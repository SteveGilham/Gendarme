//
// Unit tests for TypeRocks
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//      Daniel Abramov <ex@vingrad.ru>
//	Andreas Noever <andreas.noever@gmail.com>
//
// Copyright (C) 2007-2008 Novell, Inc (http://www.novell.com)
// (C) 2007 Daniel Abramov
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
using System.Runtime.InteropServices;

using Gendarme.Framework;
using Gendarme.Framework.Helpers;
using Gendarme.Framework.Rocks;

using Mono.Cecil;
using NUnit.Framework;

namespace Test.Framework.Rocks
{
  [TestFixture]
  public class TypeRocksTest
  {
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public class TypeCompilerGenerated
    {
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("unit test", "1.0")]
    public class TypeGeneratedCode
    {
    }

    public enum Enum
    {
      Value
    }

    [Flags]
    public enum Flags
    {
      Mask
    }

    private interface IDeepCloneable : ICloneable
    {
    }

    private class Deep : IDeepCloneable
    {
      public object Clone()
      {
        throw new NotImplementedException();
      }
    }

    private interface IInterface1
    {
    }

    private interface IInterface2 : IDeepCloneable
    {
    }

    private interface IMixinInterface : IInterface1, IInterface2
    {
    }

    private class Mixin : IMixinInterface
    {
      public object Clone()
      {
        throw new NotImplementedException();
      }
    }

    private class NotAttribute
    {
    }

    private class AnAttribute : Attribute
    {
    }

    private class ClassInheritsNotAttribute : NotAttribute
    {
    }

    private class AttributeInheritsAnAttribute : AnAttribute
    {
    }

    private class AttributeInheritsOuterAttribute : ContextStaticAttribute
    {
    }

    private class AttributeInheritsOuterAttributeDerivingAttribute : AttributeInheritsOuterAttribute
    {
    }

    private byte[] array_of_bytes;
    private Enum[] array_of_enum;
    private Flags[] array_of_flags;
    private string[] array_of_strings;
    private Deep[] array_of_classes;
    private ICloneable[] array_of_interfaces;

    private float SingleValue;
    private double DoubleValue;

    private IntPtr IntPtrValue;
    private UIntPtr UIntPtrValue;
    private HandleRef HandleRefValue;

    public void MethodA(bool parameter)
    {
    }

    private AssemblyDefinition assembly;
    private TypeDefinition type;

    [OneTimeSetUp]
    public void FixtureSetUp()
    {
      string unit = System.Reflection.Assembly.GetExecutingAssembly().Location;
      assembly = AssemblyDefinition.ReadAssembly(unit);
      type = assembly.MainModule.GetType("Test.Framework.Rocks.TypeRocksTest");
    }

    private TypeDefinition GetType(string name)
    {
      return assembly.MainModule.GetType("Test.Framework.Rocks.TypeRocksTest" + name);
    }

    private TypeReference GetFieldType(string name)
    {
      TypeDefinition type = assembly.MainModule.GetType("Test.Framework.Rocks.TypeRocksTest");
      foreach (FieldDefinition field in type.Fields)
      {
        if (name == field.Name)
          return field.FieldType;
      }
      Assert.Fail(name);
      return null;
    }

    private MethodDefinition GetMethod(string name)
    {
      foreach (MethodDefinition method in type.Methods)
        if (method.Name == name)
          return method;
      Assert.Fail(name);
      return null;
    }

    private void AssertAreSame(object a, object b, string c)
    {
      Assert.That(a, Is.SameAs(b), c);
    }

    [Test]
    public void GetMethod()
    {
      AssertAreSame(GetMethod("MethodA"), type.GetMethod(new MethodSignature("MethodA")), "a1");

      AssertAreSame(GetMethod("MethodA"), type.GetMethod("MethodA"), "a2");
      AssertAreSame(GetMethod("MethodA"), type.GetMethod(x => x.Name == "MethodA"), "a3");
      AssertAreSame(GetMethod("MethodA"), type.GetMethod(MethodAttributes.Public, "MethodA"), "a4");
      AssertAreSame(GetMethod("MethodA"), type.GetMethod("MethodA", "System.Void", new string[1]), "a5");
      AssertAreSame(GetMethod("MethodA"), type.GetMethod(MethodAttributes.Public, "MethodA", "System.Void", new string[1]), "a6");
      AssertAreSame(GetMethod("MethodA"), type.GetMethod(MethodAttributes.Public, "MethodA", "System.Void", new string[1] { "System.Boolean" }, x => x.HasBody), "a7");

      Assert.That(type.GetMethod("MethodB"), Is.Null, "b1");
      Assert.That(type.GetMethod(new MethodSignature("MethodB")), Is.Null, "b2");
      Assert.That(type.GetMethod(MethodAttributes.Static, "MethodA"), Is.Null, "b3");
      Assert.That(type.GetMethod("MethodA", null, new string[0]), Is.Null, "b4");
      Assert.That(type.GetMethod("MethodA", "System.Int32", null), Is.Null, "b5");
      Assert.That(type.GetMethod("MethodA", null, new string[1] { "System.Int32" }), Is.Null, "b6");
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
    GetType(String.Empty).HasAttribute(TN(null, "a"))));
    }

    [Test]
    public void HasAttribute_Name_Null()
    {
      Assert.Throws<ArgumentNullException>(new Action(() =>
          GetType(String.Empty).HasAttribute(TN("a", null))));
    }

    [Test]
    public void HasAttribute()
    {
      Assert.That(GetType(String.Empty).HasAttribute(TN("NUnit.Framework", "TestFixtureAttribute")), "TypeRocksTest");
      Assert.That(GetType("/Enum").HasAttribute(TN("System", "FlagsAttribute")), Is.False, "Enum/System.FlagsAttribute");
      Assert.That(GetType("/Flags").HasAttribute(TN("System", "FlagsAttribute")), "Flags/System.FlagsAttribute");
      // fullname is required
      Assert.That(GetType("/Flags").HasAttribute(TN("System", "Flags")), Is.False, "Flags/System.Flags");
      Assert.That(GetType("/Flags").HasAttribute(TN("", "FlagsAttribute")), Is.False, "Flags/FlagsAttribute");
    }

    [Test]
    public void HasMethod()
    {
      Assert.That(type.HasMethod(new MethodSignature("MethodA")), "A");
      Assert.That(type.HasMethod(new MethodSignature("MethodB")), Is.False, "B");
    }

    [Test]
    public void Implements_Namespace_Null()
    {
      Assert.Throws<ArgumentNullException>(new Action(() =>
          GetType(String.Empty).Implements(TN(null, "a"))));
    }

    [Test]
    public void Implements_Name_Null()
    {
      Assert.Throws<ArgumentNullException>(new Action(() =>
          GetType(String.Empty).Implements(TN("a", null))));
    }

    [Test]
    public void Implements()
    {
      Assert.That(GetType(String.Empty).Implements(TN("System", "ICloneable")), Is.False, "ICloneable");
      Assert.That(GetType("/IDeepCloneable").Implements(TN("Test.Framework.Rocks", "TypeRocksTest/IDeepCloneable")), "itself");
      Assert.That(GetType("/IDeepCloneable").Implements(TN("System", "ICloneable")), "interface inheritance");
      Assert.That(GetType("/Deep").Implements(TN("Test.Framework.Rocks", "TypeRocksTest/IDeepCloneable")), "IDeepCloneable");
      Assert.That(GetType("/Deep").Implements(TN("System", "ICloneable")), "second-level ICloneable");
      Assert.That(GetType("/Mixin").Implements(TN("Test.Framework.Rocks", "TypeRocksTest/IDeepCloneable")), "parent interface inheritance");
    }

    [Test]
    public void Inherits_Namespace_Null()
    {
      Assert.Throws<ArgumentNullException>(new Action(() =>
          GetType(String.Empty).Inherits(TN(null, "a"))));
    }

    [Test]
    public void Inherits_Name_Null()
    {
      Assert.Throws<ArgumentNullException>(new Action(() =>
          GetType(String.Empty).Inherits(TN("a", null))));
    }

    [Test]
    public void Inherits()
    {
      Assert.That(GetType("/NotAttribute").Inherits(TN("System", "Attribute")), Is.False, "NotAttribute");
      Assert.That(GetType("/AnAttribute").Inherits(TN("System", "Attribute")), "AnAttribute");
      Assert.That(GetType("/ClassInheritsNotAttribute").Inherits(TN("System", "Attribute")), Is.False, "ClassInheritsNotAttribute");
      Assert.That(GetType("/AttributeInheritsAnAttribute").Inherits(TN("System", "Attribute")), "AttributeInheritsAnAttribute");
    }

    [Test]
    public void Inherits_FromAnotherAssembly()
    {
      // we can't be sure here so to avoid false positives return false
      Assert.That(GetType("/AttributeInheritsOuterAttribute").Inherits(TN("System", "Attribute")), "AttributeInheritsOuterAttribute");
      Assert.That(GetType("/AttributeInheritsOuterAttributeDerivingAttribute").Inherits(TN("System", "Attribute")), "AttributeInheritsOuterAttributeDerivingAttribute");
    }

    [Test]
    public void Inherits_Itself()
    {
      TypeDefinition type = GetType(String.Empty);
      Assert.That(type.Inherits(type.GetTypeName()), "itself");
    }

    [Test]
    public void IsAttribute()
    {
      Assert.That(GetType("/NotAttribute").IsAttribute(), Is.False, "NotAttribute");
      Assert.That(GetType("/AnAttribute").IsAttribute(), "AnAttribute");
      Assert.That(GetType("/ClassInheritsNotAttribute").IsAttribute(), Is.False, "ClassInheritsNotAttribute");
      Assert.That(GetType("/AttributeInheritsAnAttribute").IsAttribute(), "AttributeInheritsAnAttribute");
    }

    [Test]
    public void IsAttribute_InheritsFromAnotherAssembly()
    {
      // we can't be sure here so to avoid false positives return false
      Assert.That(GetType("/AttributeInheritsOuterAttribute").IsAttribute(), "AttributeInheritsOuterAttribute");
      Assert.That(GetType("/AttributeInheritsOuterAttributeDerivingAttribute").IsAttribute(), "AttributeInheritsOuterAttributeDerivingAttribute");
    }

    [Test]
    public void IsFlags()
    {
      Assert.That(GetType(String.Empty).IsFlags(), Is.False, "Type.IsFlags");
      Assert.That(GetType("/Enum").IsFlags(), Is.False, "Enum.IsFlags");
      Assert.That(GetType("/Flags").IsFlags(), "Flags.IsFlags");
    }

    [Test]
    public void IsFloatingPoint()
    {
      TypeDefinition type = GetType(String.Empty);
      Assert.That(type.IsFloatingPoint(), Is.False, "Type.IsFloatingPoint");
      foreach (FieldDefinition field in type.Fields)
      {
        switch (field.Name)
        {
          case "SingleValue":
          case "DoubleValue":
            Assert.That(field.FieldType.IsFloatingPoint(), field.Name);
            break;
        }
      }
    }

    [Test]
    public void IsGeneratedCode_CompilerGenerated()
    {
      Assert.That(GetType("/TypeCompilerGenerated").IsGeneratedCode(), "IsCompilerGenerated");
      Assert.That(GetType(String.Empty).IsGeneratedCode(), Is.False, "TypeRocksTest");
    }

    [Test]
    public void IsGeneratedCode_GeneratedCode()
    {
      Assert.That(GetType("/TypeGeneratedCode").IsGeneratedCode(), "IsCompilerGenerated");
      Assert.That(GetType(String.Empty).IsGeneratedCode(), Is.False, "TypeRocksTest");
    }

    [Test]
    public void IsNative()
    {
      TypeDefinition type = GetType(String.Empty);
      Assert.That(type.IsNative(), Is.False, "Type.IsNative");
      foreach (FieldDefinition field in type.Fields)
      {
        switch (field.Name)
        {
          case "IntPtrValue":
          case "UIntPtrValue":
          case "HandleRefValue":
            Assert.That(field.FieldType.IsNative(), field.Name);
            break;
        }
      }
    }

    [Test]
    public void IsNamed()
    {
      string name = "Test.Framework.Rocks.PublicType";
      TypeDefinition type = assembly.MainModule.GetType(name);

      Assert.That(type.IsNamed("Test.Framework.Rocks.PublicType"));
      Assert.That(type.IsNamed("Test.Framework.Rocks.P"), Is.False);//Missing Text
      Assert.That(type.IsNamed("Test.Framework.Rocks.PublicTypeExtraText"), Is.False);

      Assert.That(type.IsNamed(TN("Test.Framework.Rocks", "PublicType")));
      Assert.That(type.IsNamed(TN("Test.Framework.Rocks", "P")), Is.False);//Missing Text
      Assert.That(type.IsNamed(TN("Test.Framework.Rocks", "PublicTypeExtraText")), Is.False);
    }

    [Test]
    public void IsNamedNestedType()
    {
      string name = "Test.Framework.Rocks.PublicType/NestedPublicType";
      TypeDefinition type = assembly.MainModule.GetType(name);

      Assert.That(type.IsNamed("Test.Framework.Rocks.PublicType/NestedPublicType"));
      Assert.That(type.IsNamed("Test.Framework.Rocks.PublicType/N"), Is.False);//Missing Text
      Assert.That(type.IsNamed("Test.Framework.Rocks.PublicType/NestedPublicTypeExtraStuff"), Is.False);

      Assert.That(type.IsNamed(TN("Test.Framework.Rocks", "PublicType/NestedPublicType")));
      Assert.That(type.IsNamed(TN("Test.Framework.Rocks", "PublicType/N")), Is.False);//Missing Text
      Assert.That(type.IsNamed(TN("Test.Framework.Rocks", "PublicType/NestedPublicTypeExtraText")), Is.False);

      Assert.That(type.IsNamed(TN("Test.Framework.Rocks", "NestedPublicType")), Is.False);
      Assert.That(type.IsNamed(TN("", "NestedPublicType")), Is.False);
    }

    [Test]
    public void IsNamedDoubleNestedType()
    {
      string name = "Test.Framework.Rocks.PublicType/NestedPublicType/NestedNestedPublicType";
      TypeDefinition type = assembly.MainModule.GetType(name);

      Assert.That(type.IsNamed("Test.Framework.Rocks.PublicType/NestedPublicType/NestedNestedPublicType"));

      Assert.That(type.IsNamed(TN("Test.Framework.Rocks", "PublicType/NestedPublicType/NestedNestedPublicType")));
    }

    [Test]
    public void IsVisible()
    {
      string name = "Test.Framework.Rocks.PublicType";
      TypeDefinition type = assembly.MainModule.GetType(name);
      Assert.That(type.IsVisible(), name);

      name = "Test.Framework.Rocks.PublicType/NestedPublicType";
      Assert.That(assembly.MainModule.GetType(name).IsVisible(), name);

      name = "Test.Framework.Rocks.PublicType/NestedProtectedType";
      Assert.That(assembly.MainModule.GetType(name).IsVisible(), name);

      name = "Test.Framework.Rocks.PublicType/NestedPrivateType";
      Assert.That(assembly.MainModule.GetType(name).IsVisible(), Is.False, name);

      name = "Test.Framework.Rocks.PublicType/NestedInternalType";
      Assert.That(assembly.MainModule.GetType(name).IsVisible(), Is.False, name);

      name = "Test.Framework.Rocks.InternalType";
      Assert.That(assembly.MainModule.GetType(name).IsVisible(), Is.False, name);
    }
  }
}