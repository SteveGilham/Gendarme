﻿//
// Unit tests for PreferStringIsNullOrEmptyRule
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
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

using Gendarme.Rules.Maintainability;

using NUnit.Framework;
using Test.Rules.Definitions;
using Test.Rules.Fixtures;

namespace Test.Rules.Maintainability {

	[TestFixture]
	public class PreferStringIsNullOrEmptyTest : MethodRuleTestFixture<PreferStringIsNullOrEmptyRule> {

		[Test]
		public void DoesNotApply ()
		{
			// no IL
			AssertRuleDoesNotApply (SimpleMethods.ExternalMethod);
			// no CALL[VIRT] inside IL
			AssertRuleDoesNotApply (SimpleMethods.EmptyMethod);
		}

		public bool ArgumentNullAndLengthCheck (string s)
		{
			return ((s == null) || (s.Length == 0));
		}

		public static bool MuchoArgumentNullAndLengthCheck (int a, byte b, char c, double d, string s)
		{
			return ((s == null) || (s.Length == 0));
		}

		public bool ArgumentIsNullOrEmpty (string s)
		{
			return String.IsNullOrEmpty (s);
		}

		[Test]
		public void Arguments ()
		{
			AssertRuleFailure<PreferStringIsNullOrEmptyTest> ("ArgumentNullAndLengthCheck");
			AssertRuleFailure<PreferStringIsNullOrEmptyTest> ("MuchoArgumentNullAndLengthCheck");
			AssertRuleSuccess<PreferStringIsNullOrEmptyTest> ("ArgumentIsNullOrEmpty");
		}

		private string str;

		public bool FieldNullAndLengthCheck ()
		{
			return ((str == null) || (str.Length == 0));
		}

		public bool FieldIsNullOrEmpty ()
		{
			return String.IsNullOrEmpty (str);
		}

		[Test]
		public void Fields ()
		{
			AssertRuleFailure<PreferStringIsNullOrEmptyTest> ("FieldNullAndLengthCheck");
			AssertRuleSuccess<PreferStringIsNullOrEmptyTest> ("FieldIsNullOrEmpty");
		}

		static public bool ArgumentIsNotNullAndNotEmpty (string str)
		{
			return !((str != null) && (str.Length > 0));
		}

		public bool FieldIsNotNullAndNotEmpty ()
		{
			return !((str != null) && (str.Length > 0));
		}

		[Test]
#if __MonoCS__
		[Ignore ("is not always detected, e.g. code generated by [g]mcs")]
#endif
		public void Argument_InvertedCondition ()
		{
			Assert.IsTrue (ArgumentIsNotNullAndNotEmpty (null), "null");
			Assert.IsTrue (ArgumentIsNotNullAndNotEmpty (String.Empty), "empty");
			AssertRuleFailure<PreferStringIsNullOrEmptyTest> ("ArgumentIsNotNullAndNotEmpty");
			AssertRuleFailure<PreferStringIsNullOrEmptyTest> ("FieldIsNotNullAndNotEmpty");
		}

		static bool ArgumentVariationForCoverage (string str)
		{
			return !((str != null) && (str.Length < 0));
		}

		[Test]
		public void VariationForCoverage ()
		{
			AssertRuleSuccess<PreferStringIsNullOrEmptyTest> ("ArgumentVariationForCoverage");
		}

		private static string static_str;

		public void StaticFieldNullAndLengthCheck ()
		{
			if ((static_str == null) || (static_str.Length == 0))
				Console.WriteLine ("Empty");
		}

		public void StaticFieldIsNullOrEmpty ()
		{
			if (String.IsNullOrEmpty (static_str))
				Console.WriteLine ("Empty");
		}

		[Test]
		public void StaticFields ()
		{
			AssertRuleFailure<PreferStringIsNullOrEmptyTest> ("StaticFieldNullAndLengthCheck");
			AssertRuleSuccess<PreferStringIsNullOrEmptyTest> ("StaticFieldIsNullOrEmpty");
		}

		public bool LocalNullAndLengthCheck ()
		{
			string s = String.Format ("{0}", 1);
			return ((s == null) || (s.Length == 0));
		}

		public bool MuchoLocalNullAndLengthCheck ()
		{
			string s1 = String.Format ("{0}", 1);
			if ((s1 == null) || (s1.Length == 0))
				return true;
			string s2 = String.Format ("{0}", 1);
			if ((s2 == null) || (s2.Length == 0))
				return true;
			string s3 = String.Format ("{0}", 1);
			if ((s3 == null) || (s3.Length == 0))
				return true;
			string s4 = String.Format ("{0}", 1);
			if ((s4 == null) || (s4.Length == 0))
				return true;
			string s5 = String.Format ("{0}", 1);
			return ((s5 == null) || (s5.Length == 0));
		}

		public bool LocalIsNullOrEmpty ()
		{
			string s = String.Format ("{0}", 1);
			return String.IsNullOrEmpty (s);
		}

		[Test]
		public void Locals ()
		{
			AssertRuleFailure<PreferStringIsNullOrEmptyTest> ("LocalNullAndLengthCheck", 1);
			AssertRuleFailure<PreferStringIsNullOrEmptyTest> ("MuchoLocalNullAndLengthCheck", 5);
			AssertRuleSuccess<PreferStringIsNullOrEmptyTest> ("LocalIsNullOrEmpty");
		}

		public static void StaticLocalNullAndLengthCheck ()
		{
			string s = String.Format ("{0}", 1);
			if ((s == null) || (s.Length == 0))
				Console.WriteLine ("Empty");
		}

		public static void StaticLocalIsNullOrEmpty ()
		{
			string s = String.Format ("{0}", 1);
			if (String.IsNullOrEmpty (s))
				Console.WriteLine ("Empty");
		}

		[Test]
		public void StaticLocals ()
		{
			AssertRuleFailure<PreferStringIsNullOrEmptyTest> ("StaticLocalNullAndLengthCheck");
			AssertRuleSuccess<PreferStringIsNullOrEmptyTest> ("StaticLocalIsNullOrEmpty");
		}

		// two different checks are ok if we're doing different stuff, like throwing exceptions, based on each one
		public void CommonExceptionPattern (string s)
		{
			if (s == null)
				throw new ArgumentNullException ("s");
			if (s.Length == 0)
				throw new ArgumentException ("empty");
		}

		public void ConfusingStringInstance (string s1, string s2)
		{
			// this is bad code but it's not what the rule looks for
			if ((s1 == null) || (s2.Length == 0))
				throw new InvalidOperationException ("confusing s1 and s2!");
		}

		[Test]
		public void Others ()
		{
			AssertRuleSuccess<PreferStringIsNullOrEmptyTest> ("CommonExceptionPattern");
			AssertRuleSuccess<PreferStringIsNullOrEmptyTest> ("ConfusingStringInstance");
		}

		public static bool StringIsStrictlyEmpty (string value)
		{
			return value != null && value.Length == 0;
		}

		public void ThereShouldBeNoStringIsNullOrEmptySuggestionHere (string realm)
		{
			if (realm != null && realm.Length == 0)
				throw new ArgumentException ();
			Console.WriteLine (realm);
		}

		[Test]
		[Ignore ("FIXME: false positive")]
		public void StrictlyEmpty ()
		{
			AssertRuleSuccess<PreferStringIsNullOrEmptyTest> ("StringIsStrictlyEmpty");
			AssertRuleSuccess<PreferStringIsNullOrEmptyTest> ("ThereShouldBeNoStringIsNullOrEmptySuggestionHere");
		}
	}
}
