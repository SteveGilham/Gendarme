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

namespace Test.Framework.Rocks {

	[TestFixture]
	public class TypeRocksTest {

		[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
		public class TypeCompilerGenerated {
		}

		[System.CodeDom.Compiler.GeneratedCodeAttribute ("unit test", "1.0")]
		public class TypeGeneratedCode {
		}

		public enum Enum {
			Value
		}

		[Flags]
		public enum Flags {
			Mask
		}

		interface IDeepCloneable : ICloneable {
		}

		class Deep : IDeepCloneable {
			public object Clone ()
			{
				throw new NotImplementedException ();
			}
		}

		interface  IInterface1 {
		}

		interface IInterface2 : IDeepCloneable {
		}

		interface IMixinInterface : IInterface1, IInterface2 {
		}

		class Mixin : IMixinInterface {
			public object Clone ()
			{
				throw new NotImplementedException ();
			}
		}

		class NotAttribute {
		}

		class AnAttribute : Attribute {
		}

		class ClassInheritsNotAttribute : NotAttribute {
		}

		class AttributeInheritsAnAttribute : AnAttribute {
		}

		class AttributeInheritsOuterAttribute : ContextStaticAttribute {
		}

		class AttributeInheritsOuterAttributeDerivingAttribute : AttributeInheritsOuterAttribute {
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

		public void MethodA (bool parameter) { }


		private AssemblyDefinition assembly;
		private TypeDefinition myType;

		[OneTimeSetUp]
		public void FixtureSetUp ()
		{
			string unit = System.Reflection.Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyDefinition.ReadAssembly (unit);
			myType = assembly.MainModule.GetType (typeof (TypeRocksTest).FullName);
		}

		private TypeDefinition GetType (string name)
		{
			return assembly.MainModule.GetType (typeof (TypeRocksTest).FullName + name);
		}

		private TypeReference GetFieldType (string name)
		{
			TypeDefinition type = assembly.MainModule.GetType (typeof (TypeRocksTest).FullName);
			foreach (FieldDefinition field in type.Fields) {
				if (name == field.Name)
					return field.FieldType;
			}
			Assert.Fail (name);
			return null;
		}

		private MethodDefinition GetMethod (string name)
		{
			foreach (MethodDefinition method in myType.Methods)
				if (method.Name == name)
					return method;
			Assert.Fail (name);
			return null;
		}

		[Test]
		public void GetMethod ()
		{
			Assert.AreSame (GetMethod ("MethodA"), myType.GetMethod (new MethodSignature ("MethodA")), "a1");

			Assert.AreSame (GetMethod ("MethodA"), myType.GetMethod ("MethodA"), "a2");
			Assert.AreSame (GetMethod ("MethodA"), myType.GetMethod (x => x.Name == "MethodA"), "a3");
			Assert.AreSame (GetMethod ("MethodA"), myType.GetMethod (MethodAttributes.Public, "MethodA"), "a4");
			Assert.AreSame (GetMethod ("MethodA"), myType.GetMethod ("MethodA", "System.Void", new string [1]), "a5");
			Assert.AreSame (GetMethod ("MethodA"), myType.GetMethod (MethodAttributes.Public, "MethodA", "System.Void", new string [1]), "a6");
			Assert.AreSame (GetMethod ("MethodA"), myType.GetMethod (MethodAttributes.Public, "MethodA", "System.Void", new string [1] { "System.Boolean" }, x => x.HasBody), "a7");

			Assert.IsNull (myType.GetMethod ("MethodB"), "b1");
			Assert.IsNull (myType.GetMethod (new MethodSignature ("MethodB")), "b2");
			Assert.IsNull (myType.GetMethod (MethodAttributes.Static, "MethodA"), "b3");
			Assert.IsNull (myType.GetMethod ("MethodA", null, new string [0]), "b4");
			Assert.IsNull (myType.GetMethod ("MethodA", "System.Int32", null), "b5");
			Assert.IsNull (myType.GetMethod ("MethodA", null, new string [1] { "System.Int32" }), "b6");
		}

		[Test]
		public void HasAttribute__NullParam ()
		{
			Assert.Throws<ArgumentNullException>(delegate { GetType (String.Empty).HasAttribute (null, "a"); }, "namespace");
			Assert.Throws<ArgumentNullException>(delegate { GetType (String.Empty).HasAttribute ("a", null); }, "name");
		}

		[Test]
		public void HasAttribute ()
		{
			Assert.IsTrue (GetType (String.Empty).HasAttribute ("NUnit.Framework", "TestFixtureAttribute"), "TypeRocksTest");
			Assert.IsFalse (GetType ("/Enum").HasAttribute ("System", "FlagsAttribute"), "Enum/System.FlagsAttribute");
			Assert.IsTrue (GetType ("/Flags").HasAttribute ("System", "FlagsAttribute"), "Flags/System.FlagsAttribute");
			// fullname is required
			Assert.IsFalse (GetType ("/Flags").HasAttribute ("System", "Flags"), "Flags/System.Flags");
			Assert.IsFalse (GetType ("/Flags").HasAttribute ("", "FlagsAttribute"), "Flags/FlagsAttribute");
		}

		[Test]
		public void HasMethod ()
		{
			Assert.IsTrue (myType.HasMethod (new MethodSignature ("MethodA")), "A");
			Assert.IsFalse (myType.HasMethod (new MethodSignature ("MethodB")), "B");
		}

		[Test]
		public void Implements_NullParam ()
		{
			Assert.Throws<ArgumentNullException>(delegate { GetType (String.Empty).Implements (null, "a"); }, "namespace");
			Assert.Throws<ArgumentNullException>(delegate { GetType (String.Empty).Implements ("a", null); }, "name");
		}

		[Test]
		public void Implements ()
		{
			Assert.IsFalse (GetType (String.Empty).Implements ("System", "ICloneable"), "ICloneable");
			Assert.IsTrue (GetType ("/IDeepCloneable").Implements (TestTypeNames.Namespace, "TypeRocksTest/IDeepCloneable"), "itself");
			Assert.IsTrue (GetType ("/IDeepCloneable").Implements ("System", "ICloneable"), "interface inheritance");
			Assert.IsTrue (GetType ("/Deep").Implements (TestTypeNames.Namespace, "TypeRocksTest/IDeepCloneable"), "IDeepCloneable");
			Assert.IsTrue (GetType ("/Deep").Implements ("System", "ICloneable"), "second-level ICloneable");
			Assert.IsTrue (GetType ("/Mixin").Implements (TestTypeNames.Namespace, "TypeRocksTest/IDeepCloneable"), "parent interface inheritance");
		}

		[Test]
		public void Inherits_NullParam ()
		{
			Assert.Throws<ArgumentNullException>(delegate { GetType (String.Empty).Inherits (null, "a"); }, "namespace");
			Assert.Throws<ArgumentNullException>(delegate { GetType (String.Empty).Inherits ("a", null); }, "name");
		}

		[Test]
		public void Inherits ()
		{
			Assert.IsFalse (GetType ("/NotAttribute").Inherits ("System", "Attribute"), "NotAttribute");
			Assert.IsTrue (GetType ("/AnAttribute").Inherits ("System", "Attribute"), "AnAttribute");
			Assert.IsFalse (GetType ("/ClassInheritsNotAttribute").Inherits ("System", "Attribute"), "ClassInheritsNotAttribute");
			Assert.IsTrue (GetType ("/AttributeInheritsAnAttribute").Inherits ("System", "Attribute"), "AttributeInheritsAnAttribute");
		}

		[Test]
		public void Inherits_FromSystemAssembly ()
		{
			// we can't be sure here so to avoid false positives return false
			Assert.IsTrue (GetType ("/AttributeInheritsOuterAttribute").Inherits ("System", "Attribute"), "AttributeInheritsOuterAttribute");
			Assert.IsTrue (GetType ("/AttributeInheritsOuterAttributeDerivingAttribute").Inherits ("System", "Attribute"), "AttributeInheritsOuterAttributeDerivingAttribute");
		}

		[Test]
		public void Inherits_Itself ()
		{
			TypeDefinition type = GetType (String.Empty);
			Assert.IsTrue (type.Inherits (type.Namespace, type.Name), "itself(namespace, name)");
			Assert.IsTrue (type.Inherits (type.FullName), "itself(full_name)");
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
		public void IsAttribute_InheritsFromSystemAssembly ()
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
			foreach (FieldDefinition field in type.Fields) {
				switch (field.Name) {
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
			foreach (FieldDefinition field in type.Fields) {
				switch (field.Name) {
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
			TypeDefinition type = assembly.MainModule.GetType (TestTypeNames.PublicType);

			Assert.IsTrue (type.IsNamed (TestTypeNames.PublicType), "full name: PublicType");
			Assert.IsFalse (type.IsNamed (TestTypeNames.Namespace + ".P"), "full name: P");//Missing Text
			Assert.IsFalse (type.IsNamed (TestTypeNames.PublicType + "ExtraText"), "full name: PublicTypeExtraText");

			Assert.IsTrue (type.IsNamed (TestTypeNames.Namespace, "PublicType"), "name: PublicType");
			Assert.IsFalse (type.IsNamed (TestTypeNames.Namespace, "P"), "name: P");//Missing Text
			Assert.IsFalse (type.IsNamed (TestTypeNames.Namespace, "PublicTypeExtraText"), "name: PublicTypeExtraText");
		}

		[Test]
		public void IsNamedNestedType ()
		{
			TypeDefinition type = assembly.MainModule.GetType (TestTypeNames.NestedPublicType);

			Assert.IsTrue (type.IsNamed (TestTypeNames.NestedPublicType), "full name: NestedPublicType");
			Assert.IsFalse (type.IsNamed (TestTypeNames.PublicType + "/N"), "full name: N");//Missing Text
			Assert.IsFalse (type.IsNamed (TestTypeNames.NestedPublicType + "TypeExtaStuff"), "full name: NestedPublicTypeExtaStuff");

			Assert.IsTrue (type.IsNamed (TestTypeNames.Namespace, "PublicType/NestedPublicType"), "name: NestedPublicType");
			Assert.IsFalse (type.IsNamed (TestTypeNames.Namespace, "PublicType/N"), "name: N");//Missing Text
			Assert.IsFalse (type.IsNamed (TestTypeNames.Namespace, "PublicType/NestedPublicTypeExtraText"), "name: NestedPublicTypeExtraText");

			Assert.IsFalse (type.IsNamed (TestTypeNames.Namespace, "NestedPublicType"), "not nested test for 'NestedPublicType'");
			// the test bellow is probably irrelevant test because of the way the empty name space is processed in 'IsNamed'
			Assert.IsFalse (type.IsNamed ("", "NestedPublicType"), "empty namespace and parent class for NestedPublicType");
		}

		[Test]
		public void IsNamedDoubleNestedType ()
		{
			TypeDefinition type = assembly.MainModule.GetType (TestTypeNames.NestedNestedPublicType);

			Assert.IsTrue (type.IsNamed (TestTypeNames.NestedNestedPublicType));
			
			Assert.IsTrue (type.IsNamed (TestTypeNames.Namespace, "PublicType/NestedPublicType/NestedNestedPublicType"));
		}

		[Test]
		public void IsVisible ()
		{
			TypeDefinition type = assembly.MainModule.GetType (TestTypeNames.PublicType);
			Assert.IsTrue (type.IsVisible (), TestTypeNames.PublicType);

			Assert.IsTrue (assembly.MainModule.GetType (TestTypeNames.NestedPublicType).IsVisible (), TestTypeNames.NestedPublicType);

			Assert.IsTrue (assembly.MainModule.GetType (TestTypeNames.NestedProtectedType).IsVisible (), TestTypeNames.NestedProtectedType);

			Assert.IsFalse (assembly.MainModule.GetType (TestTypeNames.NestedPrivateType).IsVisible (), TestTypeNames.NestedPrivateType);

			Assert.IsFalse (assembly.MainModule.GetType (TestTypeNames.NestedInternalType).IsVisible (), TestTypeNames.NestedInternalType);

			Assert.IsFalse (assembly.MainModule.GetType (TestTypeNames.InternalType).IsVisible (), TestTypeNames.InternalType);
		}
	}
}
