//
// Unit tests for FieldRocks
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//	Andreas Noever <andreas.noever@gmail.com>
//
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
// (C) 2008 Andreas Noever
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
using System.Linq;
using System.Reflection;

using Gendarme.Framework;
using Gendarme.Framework.Rocks;

using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace Test.Framework.Rocks
{
  [TestFixture]
  public class FieldRocksTest
  {
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    private static int cga = 1;

    [System.CodeDom.Compiler.GeneratedCodeAttribute("unit test", "1.0")]
    protected double gca = 1.0;

    internal IntPtr ptr = IntPtr.Zero;

    private AssemblyDefinition assembly;

    private TypeDefinition type;

    [OneTimeSetUp]
    public void FixtureSetUp()
    {
      string unit = Assembly.GetExecutingAssembly().Location;
      assembly = AssemblyDefinition.ReadAssembly(unit);
      type = assembly.MainModule.GetType("Test.Framework.Rocks.FieldRocksTest");
    }

    private FieldDefinition GetField(string fieldName)
    {
      foreach (FieldDefinition field in type.Fields)
      {
        if (field.Name == fieldName)
          return field;
      }
      Assert.Fail($"Field {fieldName} was not found.");
      return null;
    }

    private static TypeName TN(string ns, string name)
    {
      return new TypeName
      {
        Namespace = ns,
        Name = name
      };
    }

    [Test]
    public void HasAttribute_Namespace_Null()
    {
      Assert.Throws<ArgumentNullException>(new Action(() =>
          GetField("assembly").HasAttribute(TN(null, "a"))));
    }

    [Test]
    public void HasAttribute_Name_Null()
    {
      Assert.Throws<ArgumentNullException>(new Action(() =>
          GetField("assembly").HasAttribute(TN("a", null))));
    }

    [Test]
    public void HasAttribute()
    {
      Assert.That(GetField("cga").HasAttribute(TN("System.Runtime.CompilerServices", "CompilerGeneratedAttribute")), "CompilerGeneratedAttribute");
      Assert.That(!GetField("cga").HasAttribute(TN("NUnit.Framework", "TestFixtureAttribute")), "TestFixtureAttribute");
    }

    [Test]
    public void IsGeneratedCode_CompilerGenerated()
    {
      Assert.That(GetField("cga").IsGeneratedCode(), "IsCompilerGenerated");
      Assert.That(!GetField("assembly").IsGeneratedCode(), "FixtureSetUp");
    }

    [Test]
    public void IsGeneratedCode_GeneratedCode()
    {
      Assert.That(GetField("gca").IsGeneratedCode(), "IsCompilerGenerated");
      Assert.That(!GetField("assembly").IsGeneratedCode(), "FixtureSetUp");
    }

    private static FieldDefinition GetField(TypeDefinition type, string name)
    {
      foreach (FieldDefinition field in type.Fields)
      {
        if (field.Name == name)
          return field;
      }
      Assert.Fail("Field '{name}' not found!");
      return null;
    }

    [Test]
    public void IsVisible()
    {
      TypeDefinition type = assembly.MainModule.GetType("Test.Framework.Rocks.PublicType");
      Assert.That(GetField(type, "PublicField").IsVisible(), "PublicType.PublicField");
      Assert.That(GetField(type, "ProtectedField").IsVisible(), "PublicType.ProtectedField");
      Assert.That(!GetField(type, "InternalField").IsVisible(), "PublicType.InternalField");
      Assert.That(!GetField(type, "PrivateField").IsVisible(), "PublicType.PrivateField");

      type = assembly.MainModule.GetType("Test.Framework.Rocks.PublicType/NestedPublicType");
      Assert.That(GetField(type, "PublicField").IsVisible(), "NestedPublicType.PublicField");
      Assert.That(GetField(type, "ProtectedField").IsVisible(), "NestedPublicType.ProtectedField");
      Assert.That(!GetField(type, "PrivateField").IsVisible(), "NestedPublicType.PrivateField");

      type = assembly.MainModule.GetType("Test.Framework.Rocks.PublicType/NestedProtectedType");
      Assert.That(GetField(type, "PublicField").IsVisible(), "NestedProtectedType.PublicField");

      type = assembly.MainModule.GetType("Test.Framework.Rocks.PublicType/NestedPrivateType");
      Assert.That(!GetField(type, "PublicField").IsVisible(), "NestedPrivateType.PublicField");

      type = assembly.MainModule.GetType("Test.Framework.Rocks.InternalType");
      Assert.That(!GetField(type, "PublicField").IsVisible(), "InternalType.PublicField");
    }

    [Test]
    public void Resolve()
    {
      foreach (Instruction ins in type.Methods[0].Body.Instructions)
      {
        FieldReference field = (ins.Operand as FieldReference);
        if ((field != null) && !(field is FieldDefinition))
        {
          FieldDefinition fd = field.Resolve();
          Assert.That(field.Name, Is.EqualTo(fd.Name), "Name");
          Assert.That(field.FieldType.FullName, Is.EqualTo(fd.FieldType.FullName), "FieldType");
        }
      }
    }
  }
}