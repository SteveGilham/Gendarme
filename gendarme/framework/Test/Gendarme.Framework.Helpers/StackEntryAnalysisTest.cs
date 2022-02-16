//
// Unit tests for StackEntryAnalysis
//
// Authors:
//	Andreas Noever <andreas.noever@gmail.com>
//
//  (C) 2008 Andreas Noever
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Linq;
using System.Reflection;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Gendarme.Framework;
using Gendarme.Framework.Helpers;

using NUnit.Framework;

#pragma warning disable IDE0017 // Simplify object initialization
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable IDE0060
#if !NET472
#pragma warning disable IDE0063
#pragma warning disable IDE0066
#endif

namespace Test.Framework
{
  [TestFixture]
  public class StackEntryAnalysisTest
  {
    private AssemblyDefinition assembly;
    private TypeDefinition type;

    [OneTimeSetUp]
    public void FixtureSetUp()
    {
      string unit = Assembly.GetExecutingAssembly().Location;
      assembly = AssemblyDefinition.ReadAssembly(unit);
      type = assembly.MainModule.GetType("Test.Framework.StackEntryAnalysisTest");
    }

    public MethodDefinition GetTest(string name)
    {
      foreach (MethodDefinition method in type.Methods)
      {
        if (method.Name == name)
          return method;
      }
      return null;
    }

    public Instruction GetFirstNewObj(MethodDefinition method)
    {
      foreach (Instruction ins in method.Body.Instructions)
      {
        if (ins.OpCode.Code == Code.Newobj)
          return ins;
      }
      return null;
    }

    public object SimpleReturn()
    {
      return new object();
    }

    [Test]
    public void TestSimpleReturn()
    {
      MethodDefinition m = GetTest("SimpleReturn");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(1, result.Length, "result-Length-1");
      Assert.AreEqual(OpCodes.Ret, result[0].Instruction.OpCode, "result-Opcode-Ret");
    }

    public object SimpleLoc()
    {
      var a = new object();
      return a;
    }

    [Test]
    public void TestSimpleLoc()
    {
      MethodDefinition m = GetTest("SimpleLoc");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(1, result.Length, "result-Length-1");
      Assert.AreEqual(OpCodes.Ret, result[0].Instruction.OpCode, "result-Opcode-Ret");
    }

    public object Loc()
    {
      var a = new object();
      object b = a;
      return b;
    }

    [Test]
    public void TestLoc()
    {
      MethodDefinition m = GetTest("Loc");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(1, result.Length, "result-Length-1");
      Assert.AreEqual(OpCodes.Ret, result[0].Instruction.OpCode, "result-Opcode-Ret");
    }

    public object Loc2()
    {
      int a = 0;
      int b = 1;
      int c = 2;
      int d = 3;
      var e = new object();
      switch (new Random().Next())
      {
        case 0:
          return a;

        case 1:
          return b;

        case 2:
          return c;

        case 3:
          return d;

        default:
          return e;
      }
    }

    [Test]
    public void TestLoc2()
    {
      MethodDefinition m = GetTest("Loc2");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(1, result.Length, "result-Length-1");
      Assert.AreEqual(OpCodes.Ret, result[0].Instruction.OpCode, "result-Opcode-Ret");
    }

    public object Pop()
    {
      new object();
      return null;
    }

    [Test]
    public void TestPop()
    {
      MethodDefinition m = GetTest("Pop");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(0, result.Length);
    }

    public object Branch()
    {
      var a = new object();
      object b;
      if (new Random().Next() == 0)
        return a;
      else
        b = a;
      return b.ToString();
    }

    [Test]
    public void TestBranch()
    {
      MethodDefinition m = GetTest("Branch");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(2, result.Length, "result-Length-2");
      Assert.AreEqual(OpCodes.Ret, result[0].Instruction.OpCode, "result[0]-Opcode-Ret");
      Assert.AreEqual(OpCodes.Callvirt, result[1].Instruction.OpCode, "result[1]-Opcode-Callvirt");
    }

    public void Branch2()
    {
      var a = new Exception();
      bool cond = new Random().Next() == 0;
      a.Source = cond ? "a" : "b"; //tricks the compiler to load a onto the stack before doing a branch (to get more coverage)
    }

    [Test]
    public void TestBranch2()
    {
      MethodDefinition m = GetTest("Branch2");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(1, result.Length, "result-Length-1");
      Assert.AreEqual(OpCodes.Callvirt, result[0].Instruction.OpCode, "result-Opcode-Callvirt");
    }

    public object TryFinally()
    {
      var a = new object();
      object b = null;
      object c;
      try
      {
        b = a;
      }
      finally
      {
        c = b;
        b = null;
      }
      if (new Random().Next() == 0)
        return c.ToString();
      else
        return b;
    }

    [Test]
    public void TestTryFinally()
    {
      MethodDefinition m = GetTest("TryFinally");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(1, result.Length, "result-Length-1");
      Assert.AreEqual(OpCodes.Callvirt, result[0].Instruction.OpCode, "result-Opcode-Callvirt");
    }

    public object NestedTryFinally()
    {
      var a = new object();
      object b = null;
      object c = null;
      try
      {
        b = a;
        try
        {
          c = new object();
        }
        finally
        {
          c = a;
          a = null;
        }
      }
      finally
      {
        b = null;
      }
      if (new Random().Next() == 0)
        return a;
      else if (new Random().Next() == 0)
        return b;
      else
        return c.ToString();
    }

    [Test]
    public void TestNestedTryFinally()
    {
      MethodDefinition m = GetTest("NestedTryFinally");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(1, result.Length, "result-Length-1");
      Assert.AreEqual(OpCodes.Callvirt, result[0].Instruction.OpCode, "result-Opcode-Callvirt");
    }

    public object NestedTryFinally2()
    {
      var a = new object();
      object b = null;
      object c = null;
      try
      {
        b = a;
      }
      finally
      {
        try
        {
          c = new object();
        }
        finally
        {
          c = a;
          a = null;
        }
        b = null;
      }
      if (new Random().Next() == 0)
        return a;
      else if (new Random().Next() == 0)
        return b;
      else
        return c.ToString();
    }

    [Test]
    public void TestNestedTryFinally2()
    {
      MethodDefinition m = GetTest("NestedTryFinally2");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(1, result.Length, "result-Length-1");
      Assert.AreEqual(OpCodes.Callvirt, result[0].Instruction.OpCode, "result-Opcode-Callvirt");
    }

    public object TryCatch()
    {
      var a = new object();
      object b = null;
      object c = null;
      try
      {
        if (new Random().Next() == 0)
        {
          a = null;
          return a;
        }
      }
      catch
      {
        b = a;
        if (new Random().Next() == 0)
          return a.ToString();
      }
      return b.GetHashCode();
    }

    [Test]
    public void TestTryCatch()
    {
      MethodDefinition m = GetTest("TryCatch");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(2, result.Length, "result-Length-2"); //no "return a";
      Assert.AreEqual(OpCodes.Callvirt, result[0].Instruction.OpCode, "result[0]-Opcode-Callvirt"); //return a.ToString ();
      Assert.AreEqual(OpCodes.Callvirt, result[1].Instruction.OpCode, "result[1]-Opcode-Callvirt"); //return b.GetHashCode ();
    }

    public object TryCatchFinally()
    {
      //IL_0000: nop
      var a = new object();
      //IL_0001: newobj instance void [mscorlib]System.Object::.ctor()
      //IL_0006: stloc.0
      object b = null;
      //IL_0007: ldnull
      //IL_0008: stloc.1
      object c = null;
      //IL_0009: ldnull
      //IL_000a: stloc.2
      try
      //.try
      //.try
      {
        //IL_000b: nop
        if (new Random().Next() == 0)
        {
          //IL_000c: newobj instance void [mscorlib]System.Random::.ctor()
          //IL_0011: callvirt instance int32[mscorlib]System.Random::Next()
          //IL_0016: ldc.i4.0
          //IL_0017: ceq
          //IL_0019: stloc.3
          //IL_001a: ldloc.3
          //IL_001b: brfalse.s IL_0025
          a = null;
          //IL_001d: nop
          //IL_001e: ldnull
          //IL_001f: stloc.0
          return a;
          //IL_0020: ldloc.0
          //IL_0021: stloc.s 4
          ////(no C# code)
          //IL_0023: leave.s IL_0086
        }
        //IL_0025: nop
        //IL_0026: leave.s IL_005a
      }
      catch
      //catch[mscorlib]System.Object
      {
        //IL_0028: pop
        b = a;
        //IL_0029: nop
        //IL_002a: ldloc.0
        //IL_002b: stloc.1
        if (new Random().Next() == 0)
          //IL_002c: newobj instance void [mscorlib]System.Random::.ctor()
          //IL_0031: callvirt instance int32[mscorlib]System.Random::Next()
          //IL_0036: ldc.i4.0
          //IL_0037: ceq
          //IL_0039: stloc.s 5
          //IL_003b: ldloc.s 5
          //IL_003d: brfalse.s IL_0049
          return a.ToString();
        //IL_003f: ldloc.0
        //IL_0040: callvirt instance string[mscorlib] System.Object::ToString()
        //IL_0045: stloc.s 4
        ////(no C# code)
        //IL_0047: leave.s IL_0086
        //IL_0049: nop
        try
        //.try
        {
          //IL_004a: nop
          a = null;
          //IL_004b: ldnull
          //IL_004c: stloc.0
        }
        //IL_004d: nop
        //IL_004e: leave.s IL_0057
        finally
        //finally
        {
          //IL_0050: nop
          c = b;
          //IL_0051: ldloc.1
          //IL_0052: stloc.2
          b = a;
          //IL_0053: ldloc.0
          //IL_0054: stloc.1
        }
        //IL_0055: nop
        //IL_0056: endfinally
      }
      //(no C# code)
      //IL_0057: nop
      //IL_0058: leave.s IL_005a
      finally
      {
        //IL_005a: leave.s IL_005f
        //IL_005c: nop
      }
      //IL_005d: nop
      //IL_005e: endfinally
      if (new Random().Next() == 0)
        //IL_005f: newobj instance void [mscorlib]System.Random::.ctor()
        //IL_0064: callvirt instance int32[mscorlib]System.Random::Next()
        //IL_0069: ldc.i4.0
        //IL_006a: ceq
        //IL_006c: stloc.s 6
        //IL_006e: ldloc.s 6
        //IL_0070: brfalse.s IL_0077
        return b;
      //IL_0072: ldloc.1
      //IL_0073: stloc.s 4
      else
        return c.GetHashCode();
      //IL_0075: br.s IL_0086
      //IL_0077: ldloc.2
      //IL_0078: callvirt instance int32[mscorlib]System.Object::GetHashCode()
      //IL_007d: box[mscorlib]System.Int32
      //IL_0082: stloc.s 4
      ////(no C# code)
      //IL_0084: br.s IL_0086
      //IL_0086: ldloc.s 4
      //IL_0088: ret
    }

    [Test]
    public void TestTryCatchFinally()
    {
      /*
      GetStackEntryUsage IL_0001: newobj System.Void System.Object::.ctor()
      Alt 0 is IL_0006: stloc.0=>0 //assign to 'a'
      Following IL_0006: stloc.0
      push 0 pop 1, pop limit 0
      result 0 is [IL_0006: stloc.0, 0]
      Checking IL_0006: stloc.0
      temp save remove
      Slot Local
      Maybe add IL_002b: stloc.1 //catch/b = a
      Maybe add IL_0040: callvirt System.String System.Object::ToString() //a.ToString();

      Alt 1 is IL_002b: stloc.1=>0
      Following IL_002b: stloc.1
      push 0 pop 1, pop limit 0
      result 1 is [IL_002b: stloc.1, 0]
      Alt 2 is IL_0040: callvirt System.String System.Object::ToString()=>0
      Following IL_0040: callvirt System.String System.Object::ToString()
      push 1 pop 1, pop limit 0
      result 2 is [IL_0040: callvirt System.String System.Object::ToString(), 0]
      Checking IL_002b: stloc.1
      temp save remove
      Slot Local
      Maybe add IL_0052: stloc.2 //c = b;

      Checking IL_0040: callvirt System.String System.Object::ToString()

      Alt 3 is IL_0052: stloc.2
      IL_004e: leave.s IL_0057=>0
      Following IL_0052: stloc.2
      push 0 pop 1, pop limit 0
      result 3 is [IL_0052: stloc.2
      IL_004e: leave.s IL_0057, 0]
      Checking IL_0052: stloc.2
      temp save remove
      Slot Local
      */
      Console.WriteLine("TestTryCatchFinally");
      MethodDefinition m = GetTest("TryCatchFinally");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));
      Console.WriteLine("---------------------------------------------");

      Assert.AreEqual(2, result.Length, "result-Length-2"); //no "return a";
      Assert.AreEqual(OpCodes.Callvirt, result[0].Instruction.OpCode, "result[0]-Opcode-Callvirt"); //return a.ToString ();
      Assert.AreEqual(OpCodes.Callvirt, result[1].Instruction.OpCode, "result[1]-Opcode-Callvirt"); //return c.GetHashCode ();
    }

    public object MultipleCatch()
    {
      var a = new object();
      object b = null;
      object c = null;
      try
      {
        if (new Random().Next() == 0)
        {
          a = null;
          return a;
        }
      }
      catch (InvalidOperationException)
      {
        b = a;
      }
      catch (InvalidCastException)
      {
        c = a;
      }
      finally
      {
        a = null;
      }
      if (new Random().Next() == 0)
        return a;
      else if (new Random().Next() == 0)
        return b.GetHashCode();
      else
        return c.ToString();
    }

    [Test]
    public void TestMultipleCatch()
    {
      MethodDefinition m = GetTest("MultipleCatch");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(2, result.Length, "result-Length-2"); //no "return a";
      Assert.AreEqual(OpCodes.Callvirt, result[0].Instruction.OpCode, "result[0]-Opcode-Callvirt"); //return b.ToString ();
      Assert.AreEqual(OpCodes.Callvirt, result[1].Instruction.OpCode, "result[1]-Opcode-Callvirt"); //return c.GetHashCode ();
    }

    public object Starg(object b)
    {
      var a = new object();
      b = a;
      return b;
    }

    [Test]
    public void TestStarg()
    {
      MethodDefinition m = GetTest("Starg");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(1, result.Length, "result-Length-1");
      Assert.AreEqual(OpCodes.Ret, result[0].Instruction.OpCode, "result-Opcode-Ret");
    }

    public object Starg2(object a, object b, object c, object d) //force ldarg.s (no macro)
    {
      d = new object();
      return d;
    }

    [Test]
    public void TestStarg2()
    {
      MethodDefinition m = GetTest("Starg2");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(1, result.Length, "result-Length-1");
      Assert.AreEqual(OpCodes.Ret, result[0].Instruction.OpCode, "result-Opcode-Ret");
    }

    public static object StargStatic(object a, object b, object c, object d, object e) //force ldarg.s (no macro) (static (no this))
    {
      e = new object();
      return e;
    }

    [Test]
    public void TestStargStatic()
    {
      MethodDefinition m = GetTest("StargStatic");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(1, result.Length, "result-Length-1");
      Assert.AreEqual(OpCodes.Ret, result[0].Instruction.OpCode, "result-Opcode-Ret");
    }

    public object OutArg(out object b)
    {
      var a = new object();
      b = a;
      return b; //this is not guaranteed to work in complex situations. The lookup for stind_ref simply checks all previous instructions for ldargs.
    }

    [Test]
    public void TestOutArg()
    {
      MethodDefinition m = GetTest("OutArg");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(2, result.Length, "result-Length-2");
      Assert.AreEqual(OpCodes.Stind_Ref, result[0].Instruction.OpCode, "result[0]-Opcode-Stind_Ref");
      Assert.AreEqual(OpCodes.Ret, result[1].Instruction.OpCode, "result[1]-Opcode-Ret");
    }

    public object OutArg2(object a, object b, object c, out object d) //force non macro version
    {
      d = new object();
      return d;
    }

    [Test]
    public void TestOutArg2()
    {
      MethodDefinition m = GetTest("OutArg2");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(2, result.Length, "result-Length-2");
      Assert.AreEqual(OpCodes.Stind_Ref, result[0].Instruction.OpCode, "result[0]-Opcode-Stind_Ref");
      Assert.AreEqual(OpCodes.Ret, result[1].Instruction.OpCode, "result[1]-Opcode-Ret");
    }

    public object Switch()
    {
      var a = new object();
      object b = null;

      switch (new Random().Next())
      {
        case 0:
          return a.ToString();

        case 1:
          b = a;
          goto case 3;
        case 2:
          a = null;
          return a;

        case 3:
          return b.ToString();
      }
      return null;
    }

    [Test]
    public void TestSwitch()
    {
      MethodDefinition m = GetTest("Switch");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(2, result.Length, "result-Length-2");
      Assert.AreEqual(OpCodes.Callvirt, result[0].Instruction.OpCode, "result[0]-Opcode-Callvirt");
      Assert.AreEqual(OpCodes.Callvirt, result[1].Instruction.OpCode, "result[1]-Opcode-Callvirt");
    }

    [Test]
    public void TestSwitch2()
    {
      //I could not find c# code that loads a reference onto the stack before executing switch
      //newobj System.Object.ctor <- reference is on the stack
      //ldc.i4.0
      //switch +2,+3
      //ret <- default
      //ret <- switch1
      //ret <- switch2

      var m = new MethodDefinition("Switch2", Mono.Cecil.MethodAttributes.Public, this.type);

      ILProcessor il = m.Body.GetILProcessor();

      Instruction switch1 = il.Create(OpCodes.Ret);
      Instruction switch2 = il.Create(OpCodes.Ret);

      il.Emit(OpCodes.Newobj, (MethodReference)GetFirstNewObj(GetTest("Switch")).Operand); //get object.ctor()
      il.Emit(OpCodes.Ldc_I4_0);
      il.Emit(OpCodes.Switch, new Instruction[] { switch1, switch2 });

      //default
      il.Emit(OpCodes.Ret);

      //switch 1 and 2
      il.Append(switch1);
      il.Append(switch2);

      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(3, result.Length, "result-Length-3");
      Assert.AreEqual(OpCodes.Ret, result[0].Instruction.OpCode, "result[0]-Opcode-Ret");
      Assert.AreEqual(OpCodes.Ret, result[1].Instruction.OpCode, "result[1]-Opcode-Ret");
      Assert.AreEqual(OpCodes.Ret, result[2].Instruction.OpCode, "result[2]-Opcode-Ret");
    }

    public void Castclass()
    {
      Exception a = (Exception)new object();
      throw a;
    }

    [Test]
    public void TestCastclass()
    {
      MethodDefinition m = GetTest("Castclass");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(1, result.Length, "result-Length-1");
      Assert.AreEqual(OpCodes.Throw, result[0].Instruction.OpCode, "result-Opcode-Throw");
    }

    public void StackOffset()
    {
      string a = (string)new object();
      a.ToString();
      a.CompareTo(a);
      Console.WriteLine(a);
    }

    [Test]
    public void TestStackOffset()
    {
      MethodDefinition m = GetTest("StackOffset");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(4, result.Length, "result-Length-4");
      Assert.AreEqual(0, result[0].StackOffset, "result[0]-StackOffset-0");
      Assert.AreEqual(1, result[1].StackOffset, "result[1]-StackOffset-1");
      Assert.AreEqual(0, result[2].StackOffset, "result[2]-StackOffset-0");
      Assert.AreEqual(0, result[3].StackOffset, "result[3]-StackOffset-0");
    }

    private object field;
    public void Field()
    {
      field = new object();
      field.ToString();
    }

    [Test]
    public void TestField()
    {
      MethodDefinition m = GetTest("Field");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(2, result.Length, "result-Length-2");
      Assert.AreEqual(Code.Stfld, result[0].Instruction.OpCode.Code, "result[0]-Opcode-Stfld");
      Assert.AreEqual(Code.Callvirt, result[1].Instruction.OpCode.Code, "result[1]-Opcode-Callvirt");
    }

    public void Field2()
    {
      var t = new StackEntryAnalysisTest();
      t.field = new object(); //do not follow assigns to other objects!
      field.ToString();
      t.field.ToString();
    }

    [Test]
    [Ignore("StackEntryAnalysis does currently not check which instance the field belongs to. This will need a reverse StackEntry test and AssemblyResolver support (for protected fields and/or static fields).")]
    public void TestField2()
    {
      MethodDefinition m = GetTest("Field2");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(1, result.Length, "result-Length-1");
      Assert.AreEqual(Code.Stfld, result[0].Instruction.OpCode.Code, "result-Opcode-Stfld");
    }

    private static object staticField;
    public void StaticField()
    {
      staticField = new object();
      staticField.ToString();
    }

    [Test]
    public void TestStaticField()
    {
      MethodDefinition m = GetTest("StaticField");
      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));

      Assert.AreEqual(2, result.Length, "result-Length-2");
      Assert.AreEqual(Code.Stsfld, result[0].Instruction.OpCode.Code, "result[0]-Opcode-Stfld");
      Assert.AreEqual(Code.Callvirt, result[1].Instruction.OpCode.Code, "result[1]-Opcode-Callvirt");
    }

    [Test]
    [Ignore("Was commented out at fork; may not yet be well formed")]
    public void TestCalli()
    {
      //ldftn Calli
      //calli void ()
      //ret

      var m = new MethodDefinition("Calli", Mono.Cecil.MethodAttributes.Public, this.type);
      var cilWorker = m.Body.GetILProcessor();
      cilWorker.Emit(OpCodes.Ldftn, m);
      cilWorker.Emit(OpCodes.Calli, new CallSite(GetTest("TestCalli").ReturnType));
      cilWorker.Emit(OpCodes.Ret);

      //foreach (var ins in m.Body.Instructions)
      //Console.WriteLine("{0}", ins);

      //IL_0000: ldftn Test.Framework.StackEntryAnalysisTest Calli()
      //IL_0000: calli System.Void()
      //IL_0000: ret

      var sea = new StackEntryAnalysis(m);
      StackEntryUsageResult[] result = sea.GetStackEntryUsage(m.Body.Instructions[0]);

      Assert.AreEqual(1, result.Length);
      Assert.AreEqual(OpCodes.Calli, result[0].Instruction.OpCode);
    }

    [Test]
    public void TestMCSRetro1()
    {
      using (var stream =
        Assembly
          .GetExecutingAssembly()
          .GetManifestResourceStream("Test.Framework.csc2008.StackBadBoys.dll"))
      using (var ad = AssemblyDefinition.ReadAssembly(stream))
      {
        Console.WriteLine("Retro1");
        MethodDefinition m = ad.MainModule.GetType("MCSRetro.StackEntry").Methods.First(m0 => m0.Name == "TryCatchFinally");
        var sea = new StackEntryAnalysis(m);
        StackEntryUsageResult[] result = sea.GetStackEntryUsage(GetFirstNewObj(m));
        Console.WriteLine("---------------------------------------------");

        Assert.AreEqual(2, result.Length, "TryCatchFinally result-Length-2"); //no "return a";
        Assert.AreEqual(OpCodes.Callvirt, result[0].Instruction.OpCode, "TryCatchFinally result[0]-Opcode-Callvirt"); //return a.ToString ();
        Assert.AreEqual(OpCodes.Callvirt, result[1].Instruction.OpCode, "TryCatchFinally result[1]-Opcode-Callvirt"); //return c.GetHashCode ();
      }
    }

    [Test]
    public void TestMCSRetro2()
    {
      using (var stream =
        Assembly
          .GetExecutingAssembly()
          .GetManifestResourceStream("Test.Framework.csc2008.StackBadBoys.dll"))
      using (var ad = AssemblyDefinition.ReadAssembly(stream))
      {
        Console.WriteLine("Retro2");
        var m = ad.MainModule.GetType("MCSRetro.StackEntry").Methods.First(m0 => m0.Name == "MultipleCatch");
        var sea = new StackEntryAnalysis(m);
        var result = sea.GetStackEntryUsage(GetFirstNewObj(m));
        Console.WriteLine("---------------------------------------------");

        Assert.AreEqual(2, result.Length, "MultipleCatch result-Length-2"); //no "return a";
        Assert.AreEqual(OpCodes.Callvirt, result[0].Instruction.OpCode, "MultipleCatch result[0]-Opcode-Callvirt"); //return b.ToString ();
        Assert.AreEqual(OpCodes.Callvirt, result[1].Instruction.OpCode, "MultipleCatch result[1]-Opcode-Callvirt"); //return c.GetHashCode ();
      }
    }
  }
}