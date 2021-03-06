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

		[System.CodeDom.Compiler.GeneratedCodeAttribute ("unit test", "1.0")]
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
			public object Clone ()
			{
				throw new NotImplementedException ();
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
			public object Clone ()
			{
				throw new NotImplementedException ();
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

		private byte [] array_of_bytes;
		private Enum [] array_of_enum;
		private Flags [] array_of_flags;
		private string [] array_of_strings;
		private Deep [] array_of_classes;
		private ICloneable [] array_of_interfaces;

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
		public void FixtureSetUp ()
		{
			string unit = System.Reflection.Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyDefinition.ReadAssembly (unit);
			type = assembly.MainModule.GetType ("Test.Framework.Rocks.TypeRocksTest");
		}

		private TypeDefinition GetType (string name)
		{
			return assembly.MainModule.GetType ("Test.Framework.Rocks.TypeRocksTest" + name);
		}

		private TypeReference GetFieldType (string name)
		{
			TypeDefinition type = assembly.MainModule.GetType ("Test.Framework.Rocks.TypeRocksTest");
      foreach (FieldDefinition field in type.Fields)
      {
				if (name == field.Name)
					return field.FieldType;
			}
			Assert.Fail (name);
			return null;
		}

		private MethodDefinition GetMethod (string name)
		{
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == name)
					return method;
			Assert.Fail (name);
			return null;
		}

		[Test]
		public void GetMethod ()
		{
			Assert.AreSame (GetMethod ("MethodA"), type.GetMethod (new MethodSignature ("MethodA")), "a1");

			Assert.AreSame (GetMethod ("MethodA"), type.GetMethod ("MethodA"), "a2");
			Assert.AreSame (GetMethod ("MethodA"), type.GetMethod (x => x.Name == "MethodA"), "a3");
			Assert.AreSame (GetMethod ("MethodA"), type.GetMethod (MethodAttributes.Public, "MethodA"), "a4");
			Assert.AreSame (GetMethod ("MethodA"), type.GetMethod ("MethodA", "System.Void", new string [1]), "a5");
			Assert.AreSame (GetMethod ("MethodA"), type.GetMethod (MethodAttributes.Public, "MethodA", "System.Void", new string [1]), "a6");
			Assert.AreSame (GetMethod ("MethodA"), type.GetMethod (MethodAttributes.Public, "MethodA", "System.Void", new string [1] { "System.Boolean" }, x => x.HasBody), "a7");

			Assert.IsNull (type.GetMethod ("MethodB"), "b1");
			Assert.IsNull (type.GetMethod (new MethodSignature ("MethodB")), "b2");
			Assert.IsNull (type.GetMethod (MethodAttributes.Static, "MethodA"), "b3");
			Assert.IsNull (type.GetMethod ("MethodA", null, new string [0]), "b4");
			Assert.IsNull (type.GetMethod ("MethodA", "System.Int32", null), "b5");
			Assert.IsNull (type.GetMethod ("MethodA", null, new string [1] { "System.Int32" }), "b6");
		}

        private static TypeName TN(string ns, string name)
        {
            return new TypeName{
                Namespace = ns,
                Name = name
            };
        }

		[Test]
		public void HasAttribute_Namespace_Null ()
		{
            Assert.Throws<ArgumentNullException>(() => 
			    GetType (String.Empty).HasAttribute(TN(null, "a")));
		}

		[Test]
		public void HasAttribute_Name_Null ()
		{
			Assert.Throws<ArgumentNullException>(() => 
			    GetType (String.Empty).HasAttribute(TN ("a", null)));
		}

		[Test]
		public void HasAttribute ()
		{
			Assert.IsTrue (GetType (String.Empty).HasAttribute (TN("NUnit.Framework", "TestFixtureAttribute")), "TypeRocksTest");
			Assert.IsFalse (GetType ("/Enum").HasAttribute (TN("System", "FlagsAttribute")), "Enum/System.FlagsAttribute");
			Assert.IsTrue (GetType ("/Flags").HasAttribute (TN("System", "FlagsAttribute")), "Flags/System.FlagsAttribute");
			// fullname is required
			Assert.IsFalse (GetType ("/Flags").HasAttribute (TN("System", "Flags")), "Flags/System.Flags");
			Assert.IsFalse (GetType ("/Flags").HasAttribute (TN("", "FlagsAttribute")), "Flags/FlagsAttribute");
		}

		[Test]
		public void HasMethod ()
		{
			Assert.IsTrue (type.HasMethod (new MethodSignature ("MethodA")), "A");
			Assert.IsFalse (type.HasMethod (new MethodSignature ("MethodB")), "B");
		}

		[Test]
		public void Implements_Namespace_Null ()
		{
			Assert.Throws<ArgumentNullException>(() => 
			    GetType (String.Empty).Implements(TN (null, "a")));
		}

		[Test]
		public void Implements_Name_Null ()
		{
			Assert.Throws<ArgumentNullException>(() => 
			    GetType (String.Empty).Implements(TN  ("a", null)));
		}

		[Test]
		public void Implements ()
		{
			Assert.IsFalse (GetType (String.Empty).Implements (TN ("System", "ICloneable")), "ICloneable");
			Assert.IsTrue (GetType ("/IDeepCloneable").Implements (TN ("Test.Framework.Rocks", "TypeRocksTest/IDeepCloneable")), "itself");
			Assert.IsTrue (GetType ("/IDeepCloneable").Implements (TN ("System", "ICloneable")), "interface inheritance");
			Assert.IsTrue (GetType ("/Deep").Implements (TN ("Test.Framework.Rocks", "TypeRocksTest/IDeepCloneable")), "IDeepCloneable");
			Assert.IsTrue (GetType ("/Deep").Implements (TN ("System", "ICloneable")), "second-level ICloneable");
			Assert.IsTrue (GetType ("/Mixin").Implements (TN ("Test.Framework.Rocks", "TypeRocksTest/IDeepCloneable")), "parent interface inheritance");
		}

		[Test]
		public void Inherits_Namespace_Null ()
		{
			Assert.Throws<ArgumentNullException>(() => 
			    GetType (String.Empty).Inherits (TN (null, "a")));
		}

		[Test]
		public void Inherits_Name_Null ()
		{
			Assert.Throws<ArgumentNullException>(() => 
			    GetType (String.Empty).Inherits (TN ("a", null)));
		}

		[Test]
		public void Inherits ()
		{
			Assert.IsFalse (GetType ("/NotAttribute").Inherits (TN ("System", "Attribute")), "NotAttribute");
			Assert.IsTrue (GetType ("/AnAttribute").Inherits (TN ("System", "Attribute")), "AnAttribute");
			Assert.IsFalse (GetType ("/ClassInheritsNotAttribute").Inherits (TN ("System", "Attribute")), "ClassInheritsNotAttribute");
			Assert.IsTrue (GetType ("/AttributeInheritsAnAttribute").Inherits (TN ("System", "Attribute")), "AttributeInheritsAnAttribute");
		}

		[Test]
		public void Inherits_FromAnotherAssembly ()
		{
			// we can't be sure here so to avoid false positives return false
			Assert.IsTrue (GetType ("/AttributeInheritsOuterAttribute").Inherits (TN ("System", "Attribute")), "AttributeInheritsOuterAttribute");
			Assert.IsTrue (GetType ("/AttributeInheritsOuterAttributeDerivingAttribute").Inherits (TN ("System", "Attribute")), "AttributeInheritsOuterAttributeDerivingAttribute");
		}

		[Test]
		public void Inherits_Itself ()
		{
			TypeDefinition type = GetType (String.Empty);
			Assert.IsTrue (type.Inherits (type.GetTypeName()), "itself");
		}

		[Test]
		public void IsAttribute ()
		{
			Assert.IsFalse (GetType ("/NotAttribute").IsAttribute (), "NotAttribute");
			Assert.IsTrue (GetType ("/AnAttribute").IsAttribute (), "AnAttribute");
			Assert.IsFalse (GetType ("/ClassInheritsNotAttribute").IsAttribute (), "ClassInheritsNotAttribute");
			Assert.IsTrue (GetType ("/AttributeInheritsAnAttribute").IsAttribute (), "AttributeInheritsAnAttribute");
		}

		[Test]
		public void IsAttribute_InheritsFromAnotherAssembly ()
		{
			// we can't be sure here so to avoid false positives return false
			Assert.IsTrue (GetType ("/AttributeInheritsOuterAttribute").IsAttribute (), "AttributeInheritsOuterAttribute");
			Assert.IsTrue (GetType ("/AttributeInheritsOuterAttributeDerivingAttribute").IsAttribute (), "AttributeInheritsOuterAttributeDerivingAttribute");
		}

		[Test]
		public void IsFlags ()
		{
			Assert.IsFalse (GetType (String.Empty).IsFlags (), "Type.IsFlags");
			Assert.IsFalse (GetType ("/Enum").IsFlags (), "Enum.IsFlags");
			Assert.IsTrue (GetType ("/Flags").IsFlags (), "Flags.IsFlags");
		}

		[Test]
		public void IsFloatingPoint ()
		{
			TypeDefinition type = GetType (String.Empty);
			Assert.IsFalse (type.IsFloatingPoint (), "Type.IsFloatingPoint");
      foreach (FieldDefinition field in type.Fields)
      {
        switch (field.Name)
        {
				case "SingleValue":
				case "DoubleValue":
					Assert.IsTrue (field.FieldType.IsFloatingPoint (), field.Name);
					break;
				}
			}
		}

		[Test]
		public void IsGeneratedCode_CompilerGenerated ()
		{
			Assert.IsTrue (GetType ("/TypeCompilerGenerated").IsGeneratedCode (), "IsCompilerGenerated");
			Assert.IsFalse (GetType (String.Empty).IsGeneratedCode (), "TypeRocksTest");
		}

		[Test]
		public void IsGeneratedCode_GeneratedCode ()
		{
			Assert.IsTrue (GetType ("/TypeGeneratedCode").IsGeneratedCode (), "IsCompilerGenerated");
			Assert.IsFalse (GetType (String.Empty).IsGeneratedCode (), "TypeRocksTest");
		}

		[Test]
		public void IsNative ()
		{
			TypeDefinition type = GetType (String.Empty);
			Assert.IsFalse (type.IsNative (), "Type.IsNative");
      foreach (FieldDefinition field in type.Fields)
      {
        switch (field.Name)
        {
				case "IntPtrValue":
				case "UIntPtrValue":
				case "HandleRefValue":
					Assert.IsTrue (field.FieldType.IsNative (), field.Name);
					break;
				}
			}
		}

		[Test]
		public void IsNamed ()
		{
			string name = "Test.Framework.Rocks.PublicType";
			TypeDefinition type = assembly.MainModule.GetType (name);

			Assert.IsTrue (type.IsNamed ( "Test.Framework.Rocks.PublicType"));
			Assert.IsFalse (type.IsNamed ("Test.Framework.Rocks.P"));//Missing Text
			Assert.IsFalse (type.IsNamed ("Test.Framework.Rocks.PublicTypeExtraText"));

			Assert.IsTrue (type.IsNamed(TN  ("Test.Framework.Rocks", "PublicType")));
			Assert.IsFalse (type.IsNamed(TN  ("Test.Framework.Rocks", "P")));//Missing Text
			Assert.IsFalse (type.IsNamed(TN  ("Test.Framework.Rocks", "PublicTypeExtraText")));
		}

		[Test]
		public void IsNamedNestedType ()
		{
			string name = "Test.Framework.Rocks.PublicType/NestedPublicType";
			TypeDefinition type = assembly.MainModule.GetType (name);

			Assert.IsTrue (type.IsNamed ("Test.Framework.Rocks.PublicType/NestedPublicType"));
			Assert.IsFalse (type.IsNamed ("Test.Framework.Rocks.PublicType/N"));//Missing Text
			Assert.IsFalse (type.IsNamed ("Test.Framework.Rocks.PublicType/NestedPublicTypeExtaStuff"));

			Assert.IsTrue (type.IsNamed (TN ("Test.Framework.Rocks", "PublicType/NestedPublicType")));
			Assert.IsFalse (type.IsNamed (TN ("Test.Framework.Rocks", "PublicType/N")));//Missing Text
			Assert.IsFalse (type.IsNamed (TN ("Test.Framework.Rocks", "PublicType/NestedPublicTypeExtraText")));

			Assert.IsFalse (type.IsNamed (TN ("Test.Framework.Rocks", "NestedPublicType")));
			Assert.IsFalse (type.IsNamed (TN ("", "NestedPublicType")));
		}

		[Test]
		public void IsNamedDoubleNestedType ()
		{
			string name = "Test.Framework.Rocks.PublicType/NestedPublicType/NestedNestedPublicType";
			TypeDefinition type = assembly.MainModule.GetType (name);

			Assert.IsTrue (type.IsNamed ("Test.Framework.Rocks.PublicType/NestedPublicType/NestedNestedPublicType"));
			
			Assert.IsTrue (type.IsNamed (TN ("Test.Framework.Rocks", "PublicType/NestedPublicType/NestedNestedPublicType")));
		}

		[Test]
		public void IsVisible ()
		{
			string name = "Test.Framework.Rocks.PublicType";
			TypeDefinition type = assembly.MainModule.GetType (name);
			Assert.IsTrue (type.IsVisible (), name);

			name = "Test.Framework.Rocks.PublicType/NestedPublicType";
			Assert.IsTrue (assembly.MainModule.GetType (name).IsVisible (), name);

			name = "Test.Framework.Rocks.PublicType/NestedProtectedType";
			Assert.IsTrue (assembly.MainModule.GetType (name).IsVisible (), name);

			name = "Test.Framework.Rocks.PublicType/NestedPrivateType";
			Assert.IsFalse (assembly.MainModule.GetType (name).IsVisible (), name);

			name = "Test.Framework.Rocks.PublicType/NestedInternalType";
			Assert.IsFalse (assembly.MainModule.GetType (name).IsVisible (), name);

			name = "Test.Framework.Rocks.InternalType";
			Assert.IsFalse (assembly.MainModule.GetType (name).IsVisible (), name);
		}
	}
}
