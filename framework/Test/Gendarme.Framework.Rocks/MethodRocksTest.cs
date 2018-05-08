// 
// Unit tests for MethodRocks
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//	Andreas Noever <andreas.noever@gmail.com>
//
// Copyright (C) 2007-2008 Novell, Inc (http://www.novell.com)
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
using System.Reflection;

using Gendarme.Framework;
using Gendarme.Framework.Rocks;

using Mono.Cecil;
using NUnit.Framework;

namespace Test.Framework.Rocks {

	[TestFixture]
	public class MethodRocksTest {

		class MainClassVoidVoid {
			static void Main ()
			{
			}
		}

		class MainClassIntVoid {
			static int Main ()
			{
				return 42;
			}
		}

		class MainClassVoidStrings {
			static void Main (string[] args)
			{
			}
		}

		class MainClassIntStrings {
			static int Main (string [] args)
			{
				return 42;
			}

			~MainClassIntStrings ()
			{
			}
		}

		public class OverrideBase {
			public virtual void MethodIn (int value)
			{
				Console.WriteLine(value);
			}

			public virtual void MethodOut (out int value)
			{
				value = 10;
			}

			public virtual void MethodRef (ref int value)
			{
				Console.WriteLine(value);
				value = 10;
			}
		}

		public class Overridden : OverrideBase {
			public override void MethodIn (int value)
			{
				Console.WriteLine(value);
				Console.WriteLine(value);
			}

			public override void MethodOut (out int value)
			{
				value = 20;
			}

			public override void MethodRef (ref int value)
			{
				Console.WriteLine(value);
				Console.WriteLine(value);
				value = 20;
			}
		}

		public class NotOverridden : OverrideBase {
			new public virtual void MethodIn (int value)
			{
				Console.WriteLine(value + 1);
				Console.WriteLine(value + 2);
			}

			public virtual void MethodRef (out int value)
			{
				value = 20;
			}

			public virtual void MethodOut (ref int value)
			{
				Console.WriteLine(value);
				Console.WriteLine(value);
				value = 20;
			}
		}

		public class OverrideBaseGeneric<T> {
			public virtual void MethodIn (T value)
			{
				Console.WriteLine(value);
			}

			public virtual void MethodOut (out T value)
			{
				value = default(T);
			}

			public virtual void MethodRef (ref T value)
			{
				Console.WriteLine(value);
				value = default(T);
			}
		}

		public class NotOverriddenGeneric : OverrideBaseGeneric<string> {
			public virtual void MethodIn (int value)
			{
				Console.WriteLine(value);
				Console.WriteLine(value);
			}

			public virtual void MethodOut (out int value)
			{
				value = 20;
			}

			public virtual void MethodRef (ref int value)
			{
				Console.WriteLine(value);
				Console.WriteLine(value);
				value = 20;
			}
		}

		public class OverriddenGeneric : NotOverriddenGeneric {
			public override void MethodIn (string value)
			{
				Console.WriteLine(value + "a");
				Console.WriteLine(value + "b");
			}

			public override void MethodOut (out string value)
			{
				value = "text";
			}

			public override void MethodRef (ref string value)
			{
				Console.WriteLine("a" + value);
				Console.WriteLine("b" + value);
				value = "value";
			}
		}

		public int Value {
			get { return 42; }
			set { throw new NotSupportedException (); }
		}

		protected void EventCallback (object sender, EventArgs ea)
		{
		}

		public class FooEventArgs : EventArgs {}

		protected void FooEventCallback (object sender, FooEventArgs fea)
		{
		}

		private AssemblyDefinition assembly;
		private static readonly char[] endNameCharacters = new char[] { '.', '/' };

		[OneTimeSetUp]
		public void FixtureSetUp ()
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyDefinition.ReadAssembly (unit);
		}

		private MethodDefinition GetMethod<T> (string methodName)
		{
			string name = typeof (T).FullName.Replace ('+', '/');
			int pos = name.IndexOf ('[');
			if (pos > 0)
				name = name.Remove(pos);
			TypeDefinition type = assembly.MainModule.GetType (name);
			foreach (MethodDefinition method in type.Methods) {
				if (method.Name == methodName)
					return method;
			}
			Assert.Fail ("Method {0} was not found.", methodName);
			return null;
		}

		private MethodDefinition GetMethod (string name)
		{
			return GetMethod<MethodRocksTest> (name);
		}

		[Test]
		public void HasAttribute_Namespace_Null ()
		{
			MethodDefinition method = GetMethod ("FixtureSetUp");
            Assert.Throws<ArgumentNullException>(() => method.HasAttribute (null, "a", null));
		}

		[Test]
		public void HasAttribute_Name_Null ()
		{
			MethodDefinition method = GetMethod ("FixtureSetUp");
            Assert.Throws<ArgumentNullException>(() => method.HasAttribute ("a", null, null));
		}

		[Test]
		public void HasAttribute ()
		{
			MethodDefinition method = GetMethod ("FixtureSetUp");
			Assert.IsTrue (method.HasAttribute ("NUnit.Framework", "OneTimeSetUpAttribute", null), "NUnit.Framework.OneTimeSetUpAttribute");
			Assert.IsFalse (method.HasAttribute ("NUnit.Framework", "OneTimeSetUp", null), "NUnit.Framework.OneTimeSetUp");
		}

		[Test]
		public void IsFinalizer ()
		{
			Assert.IsFalse (GetMethod ("FixtureSetUp").IsFinalizer (), "FixtureSetUp");
			Assert.IsTrue (GetMethod<MainClassIntStrings> ("Finalize").IsFinalizer (), "~MainClassIntStrings");
		}

		[Test]
		[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
		public void IsGeneratedCode_CompilerGenerated ()
		{
			Assert.IsTrue (GetMethod ("IsGeneratedCode_CompilerGenerated").IsGeneratedMethodBody (), "IsCompilerGeneratedMethodBody");
			Assert.IsFalse (GetMethod ("IsGeneratedCode_CompilerGenerated").IsGeneratedMethodOrType (), "IsCompilerGeneratedMethodOrType");
			Assert.IsFalse (GetMethod ("FixtureSetUp").IsGeneratedMethodOrType (), "FixtureSetUp");
		}

		[Test]
		[System.CodeDom.Compiler.GeneratedCodeAttribute ("unit test", "1.0")]
		public void IsGeneratedCode_GeneratedCode ()
		{
			Assert.IsTrue (GetMethod ("IsGeneratedCode_GeneratedCode").IsGeneratedMethodOrType(), "IsCompilerGeneratedMethodOrType");
			Assert.IsTrue (GetMethod ("IsGeneratedCode_GeneratedCode").IsGeneratedMethodBody(), "IsCompilerGeneratedMethodBody");
			Assert.IsFalse (GetMethod ("FixtureSetUp").IsGeneratedMethodBody (), "FixtureSetUp");
		}

		[Test]
		public void IsMainSignature ()
		{
			Assert.IsTrue (GetMethod<MainClassVoidVoid> ("Main").IsMainSignature (), "MainClassVoidVoid");
			Assert.IsTrue (GetMethod<MainClassIntVoid> ("Main").IsMainSignature (), "MainClassIntVoid");
			Assert.IsTrue (GetMethod<MainClassVoidStrings> ("Main").IsMainSignature (), "MainClassVoidStrings");
			Assert.IsTrue (GetMethod<MainClassIntStrings> ("Main").IsMainSignature (), "MainClassIntStrings");
			Assert.IsFalse (GetMethod ("FixtureSetUp").IsMainSignature (), "FixtureSetUp");
		}

		[Test]
		public void IsProperty ()
		{
			Assert.IsTrue (GetMethod ("get_Value").IsProperty (), "get_Value");
			Assert.IsTrue (GetMethod ("set_Value").IsProperty (), "set_Value");
			Assert.IsFalse (GetMethod ("FixtureSetUp").IsProperty (), "FixtureSetUp");
		}

		[Test]
		public void IsOverride ()
		{
			Assert.IsTrue (GetMethod<Overridden> ("MethodIn").IsOverride (), "Overridden.MethodIn");
			Assert.IsTrue (GetMethod<Overridden> ("MethodOut").IsOverride (), "Overridden.MethodOut");
			Assert.IsTrue (GetMethod<Overridden> ("MethodRef").IsOverride (), "Overridden.MethodRef");
			Assert.IsFalse (GetMethod<NotOverridden> ("MethodIn").IsOverride (), "NotOverridden.MethodIn");
			Assert.IsFalse (GetMethod<NotOverridden> ("MethodOut").IsOverride (), "NotOverridden.MethodOut");
			Assert.IsFalse (GetMethod<NotOverridden> ("MethodRef").IsOverride (), "NotOverridden.MethodRef");

			Assert.IsTrue (GetMethod<OverriddenGeneric> ("MethodIn").IsOverride (), "OverriddenGeneric.MethodIn");
			Assert.IsTrue (GetMethod<OverriddenGeneric> ("MethodOut").IsOverride (), "OverriddenGeneric.MethodOut");
			Assert.IsTrue (GetMethod<OverriddenGeneric> ("MethodRef").IsOverride (), "OverriddenGeneric.MethodRef");
			Assert.IsFalse (GetMethod<NotOverriddenGeneric> ("MethodIn").IsOverride (), "NotOverriddenGeneric.MethodIn");
			Assert.IsFalse (GetMethod<NotOverriddenGeneric> ("MethodOut").IsOverride (), "NotOverriddenGeneric.MethodOut");
			Assert.IsFalse (GetMethod<NotOverriddenGeneric> ("MethodRef").IsOverride (), "NotOverriddenGeneric.MethodRef");
		}

		[Test]
		public void IsVisible ()
		{
			TypeDefinition type = assembly.MainModule.GetType (TestTypeNames.PublicType);
			Assert.IsTrue (type.GetMethod ("PublicMethod").IsVisible (), "PublicType.PublicMethod");
			Assert.IsTrue (type.GetMethod ("ProtectedMethod").IsVisible (), "PublicType.ProtectedMethod");
			Assert.IsFalse (type.GetMethod ("InternalMethod").IsVisible (), "PublicType.InternalMethod");
			Assert.IsFalse (type.GetMethod ("PrivateMethod").IsVisible (), "PublicType.PrivateMethod");

			type = assembly.MainModule.GetType (TestTypeNames.NestedPublicType);
			Assert.IsTrue (type.GetMethod ("PublicMethod").IsVisible (), "NestedPublicType.PublicMethod");
			Assert.IsTrue (type.GetMethod ("ProtectedMethod").IsVisible (), "NestedPublicType.ProtectedMethod");
			Assert.IsFalse (type.GetMethod ("PrivateMethod").IsVisible (), "NestedPublicType.PrivateMethod");

			type = assembly.MainModule.GetType (TestTypeNames.NestedProtectedType);
			Assert.IsTrue (type.GetMethod ("PublicMethod").IsVisible (), "NestedProtectedType.PublicMethod");

			type = assembly.MainModule.GetType (TestTypeNames.NestedPrivateType);
			Assert.IsFalse (type.GetMethod ("PublicMethod").IsVisible (), "NestedPrivateType.PublicMethod");

			type = assembly.MainModule.GetType (TestTypeNames.InternalType);
			Assert.IsFalse (type.GetMethod ("PublicMethod").IsVisible (), "InternalType.PublicMethod");
		}

		[Test]
		public void IsEventCallback ()
		{
			Assert.IsTrue (GetMethod ("EventCallback").IsEventCallback (), "EventCallback");
			Assert.IsTrue (GetMethod ("FooEventCallback").IsEventCallback (), "FooEventCallback");
			Assert.IsFalse (GetMethod ("IsEventCallback").IsEventCallback (), "IsEventCallback");
		}

		[Test]
		public void GetPropertyByAccessor ()
		{
			Assert.AreEqual (GetMethod ("get_Value").GetPropertyByAccessor ().Name, "Value", "get_Value");
			Assert.AreEqual (GetMethod ("set_Value").GetPropertyByAccessor ().Name, "Value", "set_Value");
			Assert.IsNull (GetMethod ("EventCallback").GetPropertyByAccessor (), "EventCallback");
		}

		[Test]
		public void SignatureEquals ()
		{
			MethodDefinition StandardIn1 = GetMethod<Overridden> ("MethodIn"); // int
			MethodDefinition StandardIn2 = GetMethod<NotOverridden> ("MethodIn"); // int
			MethodDefinition GenericIn1 = GetMethod<OverriddenGeneric> ("MethodIn"); // string
			MethodDefinition GenericIn2 = GetMethod<NotOverriddenGeneric> ("MethodIn"); // int
			MethodDefinition GenericIn3 = GetMethod<OverrideBaseGeneric<object>>("MethodIn"); // generic

			MethodDefinition StandardOut1 = GetMethod<Overridden> ("MethodOut"); // int
			MethodDefinition StandardOut2 = GetMethod<NotOverridden> ("MethodRef"); // method has a different name than parameters; see test IsOverride
			MethodDefinition GenericOut1 = GetMethod<OverriddenGeneric> ("MethodOut"); // string
			MethodDefinition GenericOut2 = GetMethod<NotOverriddenGeneric> ("MethodOut"); // int
			MethodDefinition GenericOut3 = GetMethod<OverrideBaseGeneric<object>> ("MethodOut"); // generic

			MethodDefinition StandardRef1 = GetMethod<Overridden> ("MethodRef"); // int
			MethodDefinition StandardRef2 = GetMethod<NotOverridden> ("MethodOut"); // method has a different name than parameters; see test IsOverride
			MethodDefinition GenericRef1 = GetMethod<OverriddenGeneric> ("MethodRef"); // string
			MethodDefinition GenericRef2 = GetMethod<NotOverriddenGeneric> ("MethodRef"); // int
			MethodDefinition GenericRef3 = GetMethod<OverrideBaseGeneric<object>>("MethodRef"); // generic

			SignaturesEquals (StandardIn1, StandardIn2, bothOrders: true);    // int <==> int
			SignaturesEquals (StandardIn1, GenericIn2, bothOrders: true);     // int <==> int
			SignaturesEquals (StandardIn1, GenericIn3, bothOrders: false);    // int <== T
			SignaturesEquals (GenericIn2, GenericIn3, bothOrders: false);     // int <== T
			SignaturesDiffers (GenericIn3, StandardIn1, bothOrders: false);   // T <== int
			SignaturesDiffers (GenericIn3, StandardIn2, bothOrders: false);   // T <== int
			SignaturesDiffers (GenericIn3, GenericIn1, bothOrders: false);    // T <== string
			SignaturesDiffers (StandardIn1, GenericIn1, bothOrders: true);    // int <==> string
			SignaturesDiffers (GenericIn2, GenericIn1, bothOrders: true);     // int <==> string

			SignaturesEquals (StandardOut1, StandardOut2, bothOrders: true);  // int <==> int
			SignaturesEquals (StandardOut1, GenericOut2, bothOrders: true);   // int <==> int
			SignaturesEquals (StandardOut1, GenericOut3, bothOrders: false);  // int <== T
			SignaturesEquals (GenericOut2, GenericOut3, bothOrders: false);   // int <== T
			SignaturesDiffers (GenericOut3, StandardOut1, bothOrders: false); // T <== int
			SignaturesDiffers (GenericOut3, StandardOut2, bothOrders: false); // T <== int
			SignaturesDiffers (GenericOut3, GenericOut1, bothOrders: false);  // T <== string
			SignaturesDiffers (StandardOut1, GenericOut1, bothOrders: true);  // int <==> string
			SignaturesDiffers (GenericOut2, GenericOut1, bothOrders: true);   // int <==> string

			SignaturesEquals (StandardRef1, StandardRef2, bothOrders: true);  // int <==> int
			SignaturesEquals (StandardRef1, GenericRef2, bothOrders: true);   // int <==> int
			SignaturesEquals (StandardRef1, GenericRef3, bothOrders: false);  // int <== T
			SignaturesEquals (GenericRef2, GenericRef3, bothOrders: false);   // int <== T
			SignaturesDiffers (GenericRef3, StandardRef1, bothOrders: false); // T <== int
			SignaturesDiffers (GenericRef3, StandardRef2, bothOrders: false); // T <== int
			SignaturesDiffers (GenericRef3, GenericRef1, bothOrders: false);  // T <== string
			SignaturesDiffers (StandardRef1, GenericRef1, bothOrders: true);  // int <==> string
			SignaturesDiffers (GenericRef2, GenericRef1, bothOrders: true);   // int <==> string
		}

		private void SignaturesEquals (MethodDefinition a, MethodDefinition b, bool bothOrders)
		{
			string nameA = GetShortName (a);
			string nameB = GetShortName (b);
			Assert.IsTrue (a.SignatureEquals(b), nameA + " == " + nameB);
			if (bothOrders)
			{
				Assert.IsTrue (b.SignatureEquals(a), nameB + " == " + nameA);
			}
		}

		private void SignaturesDiffers (MethodDefinition a, MethodDefinition b, bool bothOrders)
		{
			string nameA = GetShortName (a);
			string nameB = GetShortName (b);
			Assert.IsFalse (a.SignatureEquals(b), nameA + " != " + nameB);
			if (bothOrders)
			{
				Assert.IsFalse (b.SignatureEquals(a), nameB + " != " + nameA);
			}
		}

		private string GetShortName(MethodDefinition method)
		{
			string name = method.FullName;
			int pos = name.LastIndexOf (':');
			if (pos < 0)
				return name;
			pos = name.LastIndexOfAny(endNameCharacters, pos);
			if (pos < 0)
				return name;
			return name.Substring (pos + 1);
		}
	}
}
