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

		[OneTimeSetUp]
		public void FixtureSetUp ()
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyDefinition.ReadAssembly (unit);
		}

		private MethodDefinition GetMethod<T> (string methodName)
		{
			string typeName = typeof (T).FullName.Replace ('+', '/');
			TypeDefinition type = assembly.MainModule.GetType (typeName);
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
			Assert.Throws<ArgumentNullException>(delegate { method.HasAttribute (null, "a"); });
		}

		[Test]
		public void HasAttribute_Name_Null ()
		{
			MethodDefinition method = GetMethod ("FixtureSetUp");
			Assert.Throws<ArgumentNullException>(delegate { method.HasAttribute ("a", null); });
		}

		[Test]
		public void HasAttribute ()
		{
			MethodDefinition method = GetMethod ("FixtureSetUp");
			Assert.IsTrue (method.HasAttribute ("NUnit.Framework", "OneTimeSetUpAttribute"), "NUnit.Framework.OneTimeSetUpAttribute");
			Assert.IsFalse (method.HasAttribute ("NUnit.Framework", "OneTimeSetUp"), "NUnit.Framework.OneTimeSetUp");
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
		public void IsOverride()
		{
			Assert.IsTrue (GetMethod<Overridden> ("MethodIn").IsOverride (), "Overridden.MethodIn");
			Assert.IsTrue (GetMethod<Overridden> ("MethodOut").IsOverride (), "Overridden.MethodOut");
			Assert.IsTrue (GetMethod<Overridden> ("MethodRef").IsOverride (), "Overridden.MethodRef");
			Assert.IsFalse (GetMethod<NotOverridden> ("MethodIn").IsOverride (), "NotOverridden.MethodIn");
			Assert.IsFalse (GetMethod<NotOverridden> ("MethodOut").IsOverride (), "NotOverridden.MethodOut");
			Assert.IsFalse (GetMethod<NotOverridden> ("MethodRef").IsOverride (), "NotOverridden.MethodRef");
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
	}
}
