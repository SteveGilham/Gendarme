//
// Unit tests for ParameterNamesShouldMatchOverriddenMethodRule
//
// Authors:
//	Andreas Noever <andreas.noever@gmail.com>
//	Sebastien Pouliot  <sebastien@ximian.com>
//
//  (C) 2008 Andreas Noever
// Copyright (C) 2011 Novell, Inc (http://www.novell.com)
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

using Gendarme.Framework.Helpers;
using Gendarme.Rules.Naming;

using NUnit.Framework;
using Test.Rules.Fixtures;

namespace Test.Rules.Naming {

	interface ISomeInterface {
		bool InterfaceMethod (int im);
	}

	interface ISomeInterface2 {
		bool InterfaceMethod2 (int im);
	}

	abstract public class SuperBaseClass {
		protected virtual void VirtualSuperIncorrect (int vsi1, bool vsi2)
		{
		}
		protected virtual void VirtualSuperIncorrect (int vsi1, int vsi2_)
		{
		}
	}

	abstract public class BaseClass : SuperBaseClass {
		protected virtual void VirtualCorrect (int vc1, int vc2)
		{
		}

		protected virtual void VirtualIncorrect (int vi1, int vi2)
		{
		}

		protected abstract void AbstractCorrect (int ac1, int ac2);

		protected abstract void AbstractIncorrect (int ai1, int ai2);

		protected virtual void NoOverwrite (int a, int b)
		{
		}
	}

	[TestFixture]
	public class ParameterNamesShouldMatchOverriddenMethodTest : MethodRuleTestFixture<ParameterNamesShouldMatchOverriddenMethodRule> {

		class TestCase : BaseClass, ISomeInterface, ISomeInterface2, IEquatable<string> {
			protected override void VirtualCorrect (int vc1, int vc2)
			{
			}

			protected override void VirtualIncorrect (int vi1, int vi2a)
			{
			}

			protected override void VirtualSuperIncorrect (int vsi1, bool vsi2_)
			{
			}

			protected override void AbstractCorrect (int ac1, int ac2)
			{
				throw new NotImplementedException ();
			}

			protected override void AbstractIncorrect (int ai1, int ai2_)
			{
				throw new NotImplementedException ();
			}

#pragma warning disable 114
            protected virtual void NoOverwrite (int a, int bb)
			{
			}
#pragma warning restore 114

            public bool InterfaceMethod (int im_)
			{
				return false;
			}

			bool ISomeInterface2.InterfaceMethod2 (int im_)
			{
				return false;
			}

			void NoParameter ()
			{
			}

			public bool Equals (string s)
			{
				throw new NotImplementedException ();
			}
		}

		public sealed class EquatableCorrect : IEquatable<EquatableCorrect>
		{
			private readonly int value;

			public EquatableCorrect (int value)
			{
				this.value = value;
			}

			public override int GetHashCode ()
			{
				return (this.value.GetHashCode ());
			}

			public override bool Equals (object obj)
			{
				return (this.value.Equals (obj));
			}

			public bool Equals (EquatableCorrect other)
			{
				return (this.value.Equals (other.value));
			}
		}

		public sealed class EquatableIncorrect : IEquatable<EquatableIncorrect>, IEquatable<object>
		{
			private readonly int value;

			public EquatableIncorrect (int value)
			{
				this.value = value;
			}

			public bool Equals (EquatableIncorrect other)
			{
				return (this.value.Equals (other.value));
			}

			bool IEquatable<object>.Equals (object obj)
			{
				return (this.value.Equals (obj));
			}
		}

#pragma warning disable S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
		public sealed class ComparableCorrect : IComparable<ComparableCorrect>, IComparable
#pragma warning restore S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
		{
			private readonly int value;

			public ComparableCorrect (int value)
			{
				this.value = value;
			}

			public int CompareTo (object obj)
			{
				return (this.value.CompareTo (obj));
			}

			public int CompareTo (ComparableCorrect other)
			{
				return (this.value.CompareTo (other.value));
			}
		}

#pragma warning disable S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
		public sealed class ComparableCustomCorrect : IComparable<ComparableCustomCorrect>, IComparable
#pragma warning restore S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
		{
			private readonly int value;

			public ComparableCustomCorrect (int value)
			{
				this.value = value;
			}

			// not from interface
			public int CompareTo (string number)
			{
				int numValue;
				if (int.TryParse (number, out numValue))
					return (this.value.CompareTo (number));
				else
					return (this.value.CompareTo (0));
			}

			// interface
			public int CompareTo (object obj)
			{
				return (this.value.CompareTo (obj));
			}

			// interface
			public int CompareTo (ComparableCustomCorrect other)
			{
				return (this.value.CompareTo (other.value));
			}
		}

#pragma warning disable S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
		public sealed class ComparableCustomIncorrect : IComparable<ComparableCustomIncorrect>, IComparable
#pragma warning restore S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
		{
			private readonly int value;

			public ComparableCustomIncorrect (int value)
			{
				this.value = value;
			}

			// not from interface
			public int CompareTo (string other)
			{
				int numValue;
				if (int.TryParse (other, out numValue))
					return (this.value.CompareTo (other));
				else
					return (this.value.CompareTo (0));
			}

			// interface
#pragma warning disable S927 // parameter names should match base declaration and other partial definitions
			public int CompareTo (object @object)
			{
				return (this.value.CompareTo (@object));
			}

			// interface
			public int CompareTo (ComparableCustomIncorrect self)
			{
				return (this.value.CompareTo (self.value));
			}
#pragma warning restore S927 // parameter names should match base declaration and other partial definitions
		}

#pragma warning disable S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
		public sealed class ComparableGenericIncorrect<T> : IComparable<T>, IComparable
#pragma warning restore S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
		{
			private readonly int value;

			public ComparableGenericIncorrect (int value)
			{
				this.value = value;
			}

			// not from interface
			public int CompareTo (string other)
			{
				int numValue;
				if (int.TryParse (other, out numValue))
					return (this.value.CompareTo (other));
				else
					return (this.value.CompareTo (0));
			}

			// interface
			int IComparable.CompareTo (object obj)
			{
				return (this.value.CompareTo (obj));
			}

			// interface
#pragma warning disable S927 // parameter names should match base declaration and other partial definitions
			int IComparable<T>.CompareTo (T otherT)
#pragma warning restore S927 // parameter names should match base declaration and other partial definitions
			{
				return (this.value.CompareTo (otherT));
			}
		}

#pragma warning disable S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
		public sealed class ComparableGenericCorrect<T> : IComparable<T>, IComparable
#pragma warning restore S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
		{
			private readonly int value;

			public ComparableGenericCorrect (int value)
			{
				this.value = value;
			}

			// not from interface
			public int CompareTo (string number)
			{
				int numValue;
				if (int.TryParse (number, out numValue))
					return (this.value.CompareTo (number));
				else
					return (this.value.CompareTo (0));
			}

			// interface
			int IComparable.CompareTo (object obj)
			{
				return (this.value.CompareTo (obj));
			}

			// interface
			int IComparable<T>.CompareTo (T other)
			{
				return (this.value.CompareTo (other));
			}
		}

		[Test]
		public void TestVirtual ()
		{
			AssertRuleSuccess<TestCase> ("VirtualCorrect");
			AssertRuleFailure<TestCase> ("VirtualIncorrect", 1);
			AssertRuleFailure<TestCase> ("VirtualSuperIncorrect", 1);
		}

		[Test]
		public void TestAbstract ()
		{
			AssertRuleSuccess<TestCase> ("AbstractCorrect");
			AssertRuleFailure<TestCase> ("AbstractIncorrect", 1);
		}

		[Test]
		public void TestNoOverwrite ()
		{
			AssertRuleDoesNotApply<TestCase> ("NoOverwrite");
		}

		[Test]
		public void TestInterfaceMethod ()
		{
			AssertRuleFailure<TestCase> ("InterfaceMethod", 1);
			AssertRuleFailure<TestCase> ("Test.Rules.Naming.ISomeInterface2.InterfaceMethod2", 1);
		}

		[Test]
		public void TestDoesNotApply ()
		{
			AssertRuleDoesNotApply<TestCase> ("NoParameter");
		}

		[Test]
		public void GenericInterface ()
		{
			AssertRuleSuccess<OpCodeBitmask> ("Equals", new Type [] { typeof (OpCodeBitmask) });
			AssertRuleFailure<TestCase> ("Equals", 1);
		}

		[Test]
		public void EquatableImplementation ()
		{
			AssertRuleSuccess<EquatableCorrect> ("Equals", new Type [] { typeof (EquatableCorrect) });
			AssertRuleSuccess<EquatableCorrect> ("Equals", new Type[] { typeof (object) });
			AssertRuleSuccess<EquatableIncorrect> ("Equals", new Type[] { typeof (EquatableIncorrect) });
			AssertRuleFailure<EquatableIncorrect> ("System.IEquatable<System.Object>.Equals", new Type[] { typeof (object) }, 1);
		}

		[Test]
		public void ComparableImplementationStandard ()
		{
			AssertRuleSuccess<ComparableCorrect> ("CompareTo", new Type [] { typeof (ComparableCorrect) });
			AssertRuleSuccess<ComparableCorrect> ("CompareTo", new Type[] { typeof (object) });
			AssertRuleSuccess<ComparableCustomCorrect> ("CompareTo", new Type [] { typeof (ComparableCustomCorrect) });
			AssertRuleSuccess<ComparableCustomCorrect> ("CompareTo", new Type[] { typeof (object) });
			AssertRuleDoesNotApply<ComparableCustomCorrect> ("CompareTo", new Type [] { typeof (string) });
			AssertRuleDoesNotApply<ComparableCustomIncorrect> ("CompareTo", new Type[] { typeof (string) });
			AssertRuleFailure<ComparableCustomIncorrect> ("CompareTo", new Type[] { typeof (ComparableCustomIncorrect) }, 1);
			AssertRuleFailure<ComparableCustomIncorrect> ("CompareTo", new Type[] { typeof (object) }, 1);
		}

		[Test]
		public void ComparableImplementationGeneric ()
		{
			AssertRuleDoesNotApply<ComparableGenericCorrect<DateTime>> ("CompareTo");
			AssertRuleSuccess<ComparableGenericCorrect<DateTime>> ("System.IComparable.CompareTo");
			AssertRuleSuccess<ComparableGenericCorrect<DateTime>> ("System.IComparable<T>.CompareTo");
			AssertRuleDoesNotApply<ComparableGenericIncorrect<DateTime>> ("CompareTo");
			AssertRuleSuccess<ComparableGenericIncorrect<DateTime>> ("System.IComparable.CompareTo");
			AssertRuleFailure<ComparableGenericIncorrect<DateTime>> ("System.IComparable<T>.CompareTo", 1);
		}
	}
}
