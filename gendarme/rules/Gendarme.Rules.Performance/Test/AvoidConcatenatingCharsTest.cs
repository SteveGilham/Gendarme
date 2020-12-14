//
// Unit tests for AvoidConcatenatingCharsRule
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2010 Novell, Inc (http://www.novell.com)
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
using Gendarme.Rules.Performance;

using NUnit.Framework;
using Test.Rules.Definitions;
using Test.Rules.Fixtures;

namespace Tests.Rules.Performance
{
  [TestFixture]
  public class AvoidConcatenatingCharsTest : MethodRuleTestFixture<AvoidConcatenatingCharsRule>
  {
    [Test]
    public void DoesNotApply()
    {
      // no IL body
      AssertRuleDoesNotApply(SimpleMethods.ExternalMethod);
      // no calls, so String.Concat is never called
      AssertRuleDoesNotApply(SimpleMethods.EmptyMethod);
    }

    private string s;
    private char c;

    private static string ss;
    private static char sc;

    private const char cc = 'a';
    private const string cs = "s";

    private string ManualConcat(char a, char b)
    {
      // compiled as String.Concat
      return a.ToString() + b.ToString();
    }

    private string Concat_String_Array(string s)
    {
      // Concat(params string[])
      return String.Concat(s);
    }

    private string Concat_String_2(string s)
    {
      return String.Concat(s, 42.ToString());
    }

    private string Concat_String_3(string s)
    {
      return String.Concat(s, "s", 'c'.ToString());
    }

    private string Concat_String_4(string s)
    {
      return String.Concat(s, "s", 'c'.ToString(), 42.ToString());
    }

    [Test]
    public void ConcatString()
    {
      AssertRuleSuccess<AvoidConcatenatingCharsTest>("ManualConcat");
      AssertRuleSuccess<AvoidConcatenatingCharsTest>("Concat_String_Array");
      AssertRuleSuccess<AvoidConcatenatingCharsTest>("Concat_String_2");
      AssertRuleSuccess<AvoidConcatenatingCharsTest>("Concat_String_Array");
      AssertRuleSuccess<AvoidConcatenatingCharsTest>("Concat_String_3");
      AssertRuleSuccess<AvoidConcatenatingCharsTest>("Concat_String_4");
    }

    private void Object_Locals()
    {
      string s = "a";
      char c = 'c';
      // IL_000c: call instance string [mscorlib]System.Char::ToString()
      // IL_0011: call string[mscorlib] System.String::Concat(string, string)
      Console.WriteLine(s + c);
    }

    private void Object_Parameters(string s, char c)
    {
      // IL_0002: call instance string [mscorlib]System.Char::ToString()
      // IL_0007: ldarg.1
      // IL_0008: call string [mscorlib]System.String::Concat(string, string)
      Console.WriteLine(c + s);
    }

    private string Object_Fields()
    {
      // IL_000c: call instance string [mscorlib]System.Char::ToString()
      // IL_0011: call string[mscorlib] System.String::Concat(string, string)
      return (s + c);
    }

    private string Object_StaticFields()
    {
      // IL_0005: call instance string [mscorlib]System.Char::ToString()
      // IL_000a: ldsfld string Tests.Rules.Performance.AvoidConcatenatingCharsTest::ss
      // IL_000f: call string[mscorlib] System.String::Concat(string, string)
      return (sc + ss);
    }

    private string Concat_String_Int_2(string s)
    {
      // IL_0003: box [mscorlib]System.Int32
      // IL_0008: call string[mscorlib] System.String::Concat(object, object)
      return String.Concat(s, 42);
    }

    private string Object_Mixed_3(char c)
    {
      // Adds the chars as ints
      // Object_Mixed_3('A') -> "162s"
      // IL_0007: call instance string[mscorlib] System.Int32::ToString()
      // IL_000c: ldstr "s"
      // IL_0011: call string[mscorlib] System.String::Concat(string, string)
      return ('a' + c + cs);
    }

    private string Concat_Object_Mixed_3(char c)
    {
      // csc compile this as 'box int' with value '61'
      // IL_0012: call string [mscorlib]System.String::Concat(object, object, object)
      return String.Concat('a', c, cs);
    }

    [Test]
    public void ConcatObject()
    {
      // Char.ToString() cases.  Performance impact???
      //AssertRuleFailure<AvoidConcatenatingCharsTest>("Object_Locals");
      //AssertRuleFailure<AvoidConcatenatingCharsTest>("Object_Parameters");
      //AssertRuleFailure<AvoidConcatenatingCharsTest>("Object_Fields");
      //AssertRuleFailure<AvoidConcatenatingCharsTest>("Object_StaticFields");

      AssertRuleFailure<AvoidConcatenatingCharsTest>("Concat_String_Int_2");
      // adds chars as ints AssertRuleFailure<AvoidConcatenatingCharsTest>("Object_Mixed_3");
      AssertRuleFailure<AvoidConcatenatingCharsTest>("Concat_Object_Mixed_3");
    }

    // all those are compiled as Concat(params object[]) by csc

    private string Concat_Object_1(char c)
    {
      // special case - should be replaced by Object.ToString() to save the boxing
      return String.Concat(c);
      // IL_0001: box [mscorlib]System.Char
      // IL_0006: call string[mscorlib] System.String::Concat(object)
    }

    private string Object_Mixed_4(string s)
    {
      return (cc + s + String.Empty + ss);
      // IL_0000: ldstr "a" -- the compiler has generated a string for us
      // IL_0010: call string [mscorlib]System.String::Concat(string, string, string, string)
    }

    private string Concat_Object_Mixed_4(string s)
    {
      return String.Concat(cc, s, String.Empty, ss);
      // IL_0024: call string [mscorlib]System.String::Concat(object[])
    }

    private string Concat_Object_NonChar_4(string s)
    {
      return String.Concat(cc, s, String.Empty, 42);
      // IL_0026: call string [mscorlib]System.String::Concat(object[])
    }

    private string Concat_ObjectArray_String()
    {
      object[] array = new object[] { "s", "s" };
      return String.Concat(array);
      // IL_0018: call string [mscorlib]System.String::Concat(object[])
    }

    private string Concat_ObjectArray_Parameter(int[] array)
    {
      return String.Concat(array);
      // IL_0001: call string[mscorlib] System.String::Concat<int32>(class [mscorlib] System.Collections.Generic.IEnumerable`1<!!0>)
    }

    [Test]
    public void ConcatObject_Array()
    {
      AssertRuleFailure<AvoidConcatenatingCharsTest>("Concat_Object_1");
      // AssertRuleFailure<AvoidConcatenatingCharsTest>("Object_Mixed_4");
      AssertRuleFailure<AvoidConcatenatingCharsTest>("Concat_Object_Mixed_4");
      AssertRuleFailure<AvoidConcatenatingCharsTest>("Concat_Object_NonChar_4");
      // there is no boxing visible in this case
      AssertRuleSuccess<AvoidConcatenatingCharsTest>("Concat_ObjectArray_String");
      AssertRuleSuccess<AvoidConcatenatingCharsTest>("Concat_ObjectArray_Parameter");
    }

    private string Concat_NoBox_Object()
    {
      // same as "this.ToString ();" but no boxing
      return String.Concat(this);
    }

    private string Concat_NoBox_String_Object()
    {
      return String.Concat("string: ", this);
    }

    [Test]
    public void ConcatObject_NoBoxing()
    {
      AssertRuleSuccess<AvoidConcatenatingCharsTest>("Concat_NoBox_Object");
      AssertRuleSuccess<AvoidConcatenatingCharsTest>("Concat_NoBox_String_Object");
    }
  }
}