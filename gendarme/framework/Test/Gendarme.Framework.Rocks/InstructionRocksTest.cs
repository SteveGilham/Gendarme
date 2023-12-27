//
// Unit tests for InstructionRocks
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using SR = System.Reflection;

using Gendarme.Framework;
using Gendarme.Framework.Rocks;

using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace Test.Framework.Rocks
{
  [TestFixture]
  public class InstructionRocksTest
  {
    private TypeDefinition type_def;
    private MethodBody body;

    [OneTimeSetUp]
    public void FixtureSetUp()
    {
      string unit = System.Reflection.Assembly.GetExecutingAssembly().Location;
      type_def = AssemblyDefinition.ReadAssembly(unit).MainModule.GetType("Test.Framework.Rocks.InstructionRocksTest");
      body = type_def.GetMethod("Fields").Body;
    }

    private static int static_field;
    private long instance_field;

    private void Fields()
    {
      static_field = 21;
      instance_field = static_field;
      Console.WriteLine(instance_field + static_field);
    }

    protected void AssertAreEqual(object a, object b, string c)
    {
      Assert.That(a, Is.EqualTo(b), c);
    }

    protected void AssertIsNull(object a, string c)
    {
      Assert.That(a, Is.Null, c);
    }

    [Test]
    public void GetField()
    {
      Instruction nins = null;
      // that's a funny thing we can do with extention methods and null
      AssertIsNull(nins.GetField(), "null");
      foreach (Instruction ins in body.Instructions)
      {
        switch (ins.OpCode.Code)
        {
          case Code.Ldfld:
          case Code.Ldflda:
          case Code.Stfld:
            AssertAreEqual("instance_field", ins.GetField().Name, ins.OpCode.ToString());
            break;

          case Code.Ldsfld:
          case Code.Ldsflda:
          case Code.Stsfld:
            AssertAreEqual("static_field", ins.GetField().Name, ins.OpCode.ToString());
            break;

          default:
            AssertIsNull(ins.GetField(), ins.OpCode.ToString());
            break;
        }
      }
    }

    private void Variables()
    {
      int a = 0;
      long b = 1;
      byte c = 2;
      short d = 3;
      uint e = 4;
      ulong f = 5;
      sbyte g = 6;
      ushort h = 7;
      Console.WriteLine((ulong)(a + b + c + d + e) + f + (ulong)(g + h));
    }

    [Test]
    public void GetVariable()
    {
      Instruction nins = null;
      // that's a funny thing we can do with extention methods and null
      AssertIsNull(nins.GetVariable(null), "all-null");
      MethodDefinition method = type_def.GetMethod("Variables");
      AssertIsNull(nins.GetVariable(method), "ins-null");
      foreach (Instruction ins in method.Body.Instructions)
      {
        switch (ins.OpCode.Code)
        {
          case Code.Ldloc_0:
          case Code.Ldloc_1:
          case Code.Ldloc_2:
          case Code.Ldloc_3:
          case Code.Ldloc:
          case Code.Ldloc_S:
          case Code.Ldloca:
          case Code.Ldloca_S:
          case Code.Stloc_0:
          case Code.Stloc_1:
          case Code.Stloc_2:
          case Code.Stloc_3:
          case Code.Stloc:
          case Code.Stloc_S:
            Assert.That(ins.GetVariable(method), Is.Not.Null, ins.OpCode.ToString());
            break;

          default:
            AssertIsNull(ins.GetVariable(method), ins.OpCode.ToString());
            break;
        }
      }
    }

    private static OpCode GetOpCode(Code code)
    {
      return (OpCode)typeof(OpCodes).GetField(
        code.ToString(),
        SR.BindingFlags.Public | SR.BindingFlags.Static).GetValue(null);
    }

    [Test]
    public void IsLoadArgument()
    {
      Instruction ins = null;
      // that's a funny thing we can do with extention methods and null
      Assert.That(ins.IsLoadArgument(), Is.False, "null");
      // work around Cecil validations
      ins = Instruction.Create(OpCodes.Nop);
      foreach (Code code in Enum.GetValues(typeof(Code)))
      {
        ins.OpCode = GetOpCode(code);
        switch (code)
        {
          case Code.Ldarg_0:
          case Code.Ldarg_1:
          case Code.Ldarg_2:
          case Code.Ldarg_3:
          case Code.Ldarg:
          case Code.Ldarg_S:
          case Code.Ldarga:
          case Code.Ldarga_S:
            Assert.That(ins.IsLoadArgument(), code.ToString());
            break;

          default:
            Assert.That(ins.IsLoadArgument(), Is.False, code.ToString());
            break;
        }
      }
    }

    [Test]
    public void IsLoadElement()
    {
      Instruction ins = null;
      // that's a funny thing we can do with extention methods and null
      Assert.That(ins.IsLoadElement(), Is.False, "null");
      // work around Cecil validations
      ins = Instruction.Create(OpCodes.Nop);
      foreach (Code code in Enum.GetValues(typeof(Code)))
      {
        ins.OpCode = GetOpCode(code);
        switch (code)
        {
          case Code.Ldelem_Any:
          case Code.Ldelem_I:
          case Code.Ldelem_I1:
          case Code.Ldelem_I2:
          case Code.Ldelem_I4:
          case Code.Ldelem_I8:
          case Code.Ldelem_R4:
          case Code.Ldelem_R8:
          case Code.Ldelem_Ref:
          case Code.Ldelem_U1:
          case Code.Ldelem_U2:
          case Code.Ldelem_U4:
          case Code.Ldelema:
            Assert.That(ins.IsLoadElement(), code.ToString());
            break;

          default:
            Assert.That(ins.IsLoadElement(), Is.False, code.ToString());
            break;
        }
      }
    }

    [Test]
    public void IsLoadIndirect()
    {
      Instruction ins = null;
      // that's a funny thing we can do with extention methods and null
      Assert.That(ins.IsLoadIndirect(), Is.False, "null");
      // work around Cecil validations
      ins = Instruction.Create(OpCodes.Nop);
      foreach (Code code in Enum.GetValues(typeof(Code)))
      {
        ins.OpCode = GetOpCode(code);
        switch (code)
        {
          case Code.Ldind_I:
          case Code.Ldind_I1:
          case Code.Ldind_I2:
          case Code.Ldind_I4:
          case Code.Ldind_I8:
          case Code.Ldind_R4:
          case Code.Ldind_R8:
          case Code.Ldind_Ref:
          case Code.Ldind_U1:
          case Code.Ldind_U2:
          case Code.Ldind_U4:
            Assert.That(ins.IsLoadIndirect(), code.ToString());
            break;

          default:
            Assert.That(ins.IsLoadIndirect(), Is.False, code.ToString());
            break;
        }
      }
    }

    [Test]
    public void IsLoadLocal()
    {
      Instruction ins = null;
      // that's a funny thing we can do with extention methods and null
      Assert.That(ins.IsLoadLocal(), Is.False, "null");
      // work around Cecil validations
      ins = Instruction.Create(OpCodes.Nop);
      foreach (Code code in Enum.GetValues(typeof(Code)))
      {
        ins.OpCode = GetOpCode(code);
        switch (code)
        {
          case Code.Ldloc_0:
          case Code.Ldloc_1:
          case Code.Ldloc_2:
          case Code.Ldloc_3:
          case Code.Ldloc:
          case Code.Ldloc_S:
          case Code.Ldloca:
          case Code.Ldloca_S:
            Assert.That(ins.IsLoadLocal(), code.ToString());
            break;

          default:
            Assert.That(ins.IsLoadLocal(), Is.False, code.ToString());
            break;
        }
      }
    }

    [Test]
    public void IsStoreArgument()
    {
      Instruction ins = null;
      // that's a funny thing we can do with extention methods and null
      Assert.That(ins.IsStoreArgument(), Is.False, "null");
      // work around Cecil validations
      ins = Instruction.Create(OpCodes.Nop);
      foreach (Code code in Enum.GetValues(typeof(Code)))
      {
        ins.OpCode = GetOpCode(code);
        switch (code)
        {
          case Code.Starg:
          case Code.Starg_S:
            Assert.That(ins.IsStoreArgument(), code.ToString());
            break;

          default:
            Assert.That(ins.IsStoreArgument(), Is.False, code.ToString());
            break;
        }
      }
    }

    [Test]
    public void IsStoreLocal()
    {
      Instruction ins = null;
      // that's a funny thing we can do with extention methods and null
      Assert.That(ins.IsStoreLocal(), Is.False, "null");
      // work around Cecil validations
      ins = Instruction.Create(OpCodes.Nop);
      foreach (Code code in Enum.GetValues(typeof(Code)))
      {
        ins.OpCode = GetOpCode(code);
        switch (code)
        {
          case Code.Stloc_0:
          case Code.Stloc_1:
          case Code.Stloc_2:
          case Code.Stloc_3:
          case Code.Stloc:
          case Code.Stloc_S:
            Assert.That(ins.IsStoreLocal(), code.ToString());
            break;

          default:
            Assert.That(ins.IsStoreLocal(), Is.False, code.ToString());
            break;
        }
      }
    }
  }
}