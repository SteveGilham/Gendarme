namespace AltCode.CecilExtensions

open System.Diagnostics.CodeAnalysis

[<AutoOpen>]
module internal Augment =

  type System.Object with
    member self.IsNotNull = self |> isNull |> not

  type Microsoft.FSharp.Core.Option<'T> with
    static member DefaultValue (fallback : 'T) (x : option<'T>) = defaultArg x fallback

[<assembly: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope="member", Target="<StartupCode$CecilExtensions>.$NetCoreResolver.#.cctor()", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope="member", Target="<StartupCode$CecilExtensions>.$ProgramDatabase.#.cctor()", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope="member", Target="AltCode.CecilExtensions.Augment.#Option`1.DefaultValue.Static`1(!!0,Microsoft.FSharp.Core.FSharpOption`1<!!0>)", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Scope="member", Target="AltCode.CecilExtensions.NetCoreResolver.#resolveFromNugetCache`1(!!0,Mono.Cecil.AssemblyNameReference)", MessageId="_arg1", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Usage", "CA2235:MarkAllNonSerializableFields", Scope="member", Target="AltCode.CecilExtensions.NetCoreResolver+candidate@69.#name", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Usage", "CA2235:MarkAllNonSerializableFields", Scope="member", Target="AltCode.CecilExtensions.NetCoreResolver+candidate@69.#y", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Usage", "CA2235:MarkAllNonSerializableFields", Scope="member", Target="AltCode.CecilExtensions.NetCoreResolver+handleResolved@87.#name", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Scope="member", Target="AltCode.CecilExtensions.NetCoreResolver+Pipe #1 stage #1 at line 119@119.#Invoke(System.WeakReference)", MessageId="0", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Usage", "CA2235:MarkAllNonSerializableFields", Scope="member", Target="AltCode.CecilExtensions.NetCoreResolver+Pipe #1 stage #4 at line 73@74.#y", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Usage", "CA2235:MarkAllNonSerializableFields", Scope="member", Target="AltCode.CecilExtensions.NetCoreResolver+Pipe #1 stage #7 at line 83@83.#name", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase.#GetPdbFromImage(Mono.Cecil.AssemblyDefinition)", MessageId="0", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase.#GetPdbWithFallback(Mono.Cecil.AssemblyDefinition)", MessageId="Fallback", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase.#GetPdbWithFallback(Mono.Cecil.AssemblyDefinition)", MessageId="0", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase.#optionFilter`1(Microsoft.FSharp.Core.FSharpFunc`2<!!0,System.Boolean>,Microsoft.FSharp.Core.FSharpOption`1<!!0>)", MessageId="a", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase.#optionFilter`1(Microsoft.FSharp.Core.FSharpFunc`2<!!0,System.Boolean>,Microsoft.FSharp.Core.FSharpOption`1<!!0>)", MessageId="option", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase.#optionFilter`1(Microsoft.FSharp.Core.FSharpFunc`2<!!0,System.Boolean>,Microsoft.FSharp.Core.FSharpOption`1<!!0>)", MessageId="a", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase.#optionFilter`1(Microsoft.FSharp.Core.FSharpFunc`2<!!0,System.Boolean>,Microsoft.FSharp.Core.FSharpOption`1<!!0>)", MessageId="T", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase.#optionFilter`1(Microsoft.FSharp.Core.FSharpFunc`2<!!0,System.Boolean>,Microsoft.FSharp.Core.FSharpOption`1<!!0>)", MessageId="0", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Usage", "CA2235:MarkAllNonSerializableFields", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase+GetPdbFromImage@54.#assembly", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase+Pipe #1 stage #1 at line 29@29.#Invoke(System.Type)", MessageId="0", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase+Pipe #1 stage #1 at line 39@39.#Invoke(Mono.Cecil.ModuleDefinition)", MessageId="0", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase+Pipe #1 stage #2 at line 40@40.#Invoke(Mono.Cecil.ModuleDefinition)", MessageId="0", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase+Pipe #1 stage #3 at line 41@41.#Invoke(Mono.Cecil.Cil.ImageDebugHeader)", MessageId="0", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase+Pipe #1 stage #4 at line 42@42.#Invoke(Mono.Cecil.Cil.ImageDebugHeader)", MessageId="0", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase+Pipe #1 stage #5 at line 43@43.#Invoke(Mono.Cecil.Cil.ImageDebugHeaderEntry)", MessageId="0", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase+Pipe #1 stage #6 at line 44@44.#Invoke(System.Byte[])", MessageId="0", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase+Pipe #1 stage #8 at line 52@52.#Invoke(System.String)", MessageId="0", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Usage", "CA2235:MarkAllNonSerializableFields", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase+ReadSymbols@96.#assembly", Justification = "work in progress")>]
[<assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Scope="member", Target="AltCode.CecilExtensions.ProgramDatabase+ReadSymbols@96.#Invoke(System.String)", MessageId="0", Justification = "work in progress")>]
()