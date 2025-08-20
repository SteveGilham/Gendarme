namespace AltCode.CecilExtensions

open System
open System.Diagnostics.CodeAnalysis
open System.Linq

open Mono.Cecil
open Mono.Cecil.Cil

module Containing =

  let private cSharpContainingMethod (name: string) (ct: TypeDefinition) index predicate =
    let stripped =
      name.Substring(1, index).Replace('-', '.')

    let methods = ct.Methods

    let candidates =
      (methods.Concat(
        ct.DeclaringType // Hope we don't have to generalise this
        |> Option.ofObj
        |> Option.filter (fun _ -> ct.Name.StartsWith("<", StringComparison.Ordinal))
        |> Option.map (fun c -> c.Methods |> Seq.toList)
        |> Option.defaultValue []
      ))
      |> Seq.filter (fun mx -> (mx.Name == stripped) && mx.HasBody)
      |> Seq.toList

    match candidates with
    | [ x ] -> Some x
    | _ ->
      let tag = "<" + stripped + ">"

      let sibs =
        ct.DeclaringType // Hope we don't have to generalise this
        |> Option.ofObj
        |> Option.map (fun c ->
          c.NestedTypes
          |> Seq.filter (fun t -> t.Name.IndexOf(tag, StringComparison.Ordinal) >= 0)
          |> Seq.collect (fun t -> t.Methods)
          |> Seq.filter (fun m -> m.HasBody))
        |> Option.defaultValue ([] |> Seq.ofList)

      let peers =
        methods
        |> Seq.filter (fun mx ->
          (mx.Name.IndexOf(tag, StringComparison.Ordinal)
           >= 0)
          && mx.HasBody)

      let children =
        ct.NestedTypes
        |> Seq.filter (fun tx -> tx.Name.StartsWith("<", StringComparison.Ordinal))
        |> Seq.collect (fun tx -> tx.Methods)
        |> Seq.filter (fun mx ->
          mx.HasBody
          && (mx.Name.IndexOf(tag, StringComparison.Ordinal)
              >= 0
              || mx.DeclaringType.Name.IndexOf(tag, StringComparison.Ordinal)
                 >= 0))

      candidates.Concat(sibs).Concat(peers).Concat(children)
      |> Seq.filter predicate
      |> Seq.sortBy (fun mx -> mx.DeclaringType.FullName.Split('/').Length) // strive upwards
      |> Seq.tryHead

  let internal sameType (target: TypeReference) (candidate: TypeReference) =
    if target = candidate then
      true
    else if target.HasGenericParameters then
      let cname = candidate.FullName
      let last = cname.LastIndexOf('<')

      if last < 0 then
        false
      else
        let stripped = cname.Substring(0, last)
        let tname = target.FullName
        stripped == tname
    else
      false

  let internal sameFunction (target: MethodReference) (candidate: MethodReference) =
    if target = candidate then
      true
    else if sameType target.DeclaringType candidate.DeclaringType then
      let cname = candidate.Name
      let tname = target.Name
      tname == cname
    else
      false

  let internal methodConstructsType (t: TypeReference) (m: MethodDefinition) =
    m.Body.Instructions
    |> Seq.filter (fun i -> i.OpCode = OpCodes.Newobj)
    |> Seq.exists (fun i ->
      let tn =
        (i.Operand :?> MethodReference).DeclaringType

      sameType t tn)

  let internal methodLoadsType (t: TypeReference) (m: MethodDefinition) =
    m.Body.Instructions
    |> Seq.filter (fun i -> i.OpCode = OpCodes.Ldsfld)
    |> Seq.exists (fun i ->
      let tn =
        (i.Operand :?> FieldReference).FieldType

      sameType t tn)

  [<SuppressMessage("Gendarme.Rules.Maintainability",
                    "AvoidUnnecessarySpecializationRule",
                    Justification = "AvoidSpeculativeGenerality too")>]
  let private fSharpContainingMethod (t: TypeDefinition) (tx: TypeReference) =
    let candidates =
      t.DeclaringType.Methods.Concat(
        t.DeclaringType.NestedTypes
        |> Seq.filter (fun t2 -> (t2 :> TypeReference) <> tx)
        |> Seq.collect (fun t2 -> t2.Methods)
      )
      |> Seq.filter (fun m -> m.HasBody)

    candidates
    |> Seq.tryFind (fun c ->
      (methodConstructsType tx c)
      || (methodLoadsType tx c))

  let internal methodCallsMethod (t: MethodReference) (m: MethodDefinition) =
    m.Body.Instructions
    |> Seq.filter (fun i -> i.OpCode.FlowControl = FlowControl.Call)
    |> Seq.exists (fun i ->
      let tn = (i.Operand :?> MethodReference)
      sameFunction t tn)

  let internal methodLoadsMethod (t: MethodReference) (m: MethodDefinition) =
    m.Body.Instructions
    |> Seq.filter (fun i -> i.OpCode = OpCodes.Ldftn)
    |> Seq.exists (fun i ->
      let tn = (i.Operand :?> MethodReference)
      sameFunction t tn)

  let internal containingMethod (m: MethodDefinition) =
    let mname = m.Name
    let t = m.DeclaringType

    // like s.IndexOf('>') but need to match paired nested angle-brackets
    let indexOfMatchingClosingAngleBracket s =
      let mutable nesting = 0

      s
      |> Seq.takeWhile (fun c ->
        if c = '<' then
          nesting <- nesting + 1

        if c = '>' then
          nesting <- nesting - 1

        nesting > 0)
      |> Seq.length

    if
      mname.StartsWith("<", StringComparison.Ordinal)
      && charIndexOf mname '|' > 0
    then
      let index =
        (indexOfMatchingClosingAngleBracket mname) - 1

      cSharpContainingMethod mname t index (fun mx ->
        (mx.FullName != m.FullName)
        && (methodCallsMethod m mx))

    else
      let n = t.Name

      if t.IsNested |> not then
        None
      else if n.StartsWith("<", StringComparison.Ordinal) then
        let name =
          if n.StartsWith("<>", StringComparison.Ordinal) then
            mname
          else
            n

        // let index = name.IndexOf('>') - 1 // but need to match paired nested angle-brackets
        let index =
          (indexOfMatchingClosingAngleBracket name) - 1

        if (index < 1) then
          None
        else
          cSharpContainingMethod
            name
            t.DeclaringType
            index
            // Guard against simple recursion here (mutual will need more work!)
            (fun mx ->
              (mx.FullName != m.FullName)
              && (methodCallsMethod m mx
                  || methodConstructsType t mx
                  || methodLoadsMethod m mx))
      else if charIndexOf n '@' >= 0 then
        let tx =
          if n.EndsWith("T", StringComparison.Ordinal) then
            match
              t.Methods
              |> Seq.tryFind (fun m ->
                m.IsConstructor
                && m.HasParameters
                && (m.Parameters.Count = 1))
              |> Option.map (fun m -> m.Parameters |> Seq.head)
            with
            | None -> t :> TypeReference
            | Some other -> other.ParameterType
          else
            t :> TypeReference

        fSharpContainingMethod t tx
      else
        None

  let public Methods m =
    Seq.unfold
      (fun (state: MethodDefinition option) ->
        match state with
        | None -> None
        | Some x ->
          Some(
            x,
            let next = containingMethod x
            next
          ))
      (Some m)