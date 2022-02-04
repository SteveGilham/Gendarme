//
// Unit tests for PreferIFormatProviderOverrideRule
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
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
using System.Globalization;
using System.Reflection;
using System.Resources;

using Gendarme.Rules.Globalization;
using NUnit.Framework;

using Test.Rules.Definitions;
using Test.Rules.Fixtures;

namespace Tests.Rules.Globalization
{
  internal class IFormatProviderTestCases
  {
    public void Empty()
    {
    }

#pragma warning disable IDE0060 // Remove unused parameter
    public void Empty(IFormatProvider format)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

    public void BadEmpty()
    {
      Empty();
    }

    public void CorrectEmpty()
    {
      Empty(null);
    }

#pragma warning disable IDE0060 // Remove unused parameter
    public void First(object obj)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

#pragma warning disable IDE0060 // Remove unused parameter
    public void First(IFormatProvider format, object obj)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

    public void BadFirst()
    {
      First(null);
    }

    public void CorrectFirst()
    {
      First(null, null);
    }

#pragma warning disable IDE0060 // Remove unused parameter
    public void Last(object obj)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

#pragma warning disable IDE0060 // Remove unused parameter
    public void Last(object obj, IFormatProvider format)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

    public void BadLast()
    {
      Last(null);
    }

    public void CorrectLast()
    {
      Last(null, null);
    }
  }

  internal class CultureInfoTestCases
  {
    public void Empty()
    {
    }

#pragma warning disable IDE0060 // Remove unused parameter
    public void Empty(CultureInfo info)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

    public void BadEmpty()
    {
      Empty();
    }

    public void CorrectEmpty()
    {
      Empty(null);
    }

#pragma warning disable IDE0060 // Remove unused parameter
    public void First(object obj)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

#pragma warning disable IDE0060 // Remove unused parameter
    public void First(CultureInfo info, object obj)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

    public void BadFirst()
    {
      First(null);
    }

    public void CorrectFirst()
    {
      First(null, null);
    }

#pragma warning disable IDE0060 // Remove unused parameter
    public void Last(object obj)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

#pragma warning disable IDE0060 // Remove unused parameter
    public void Last(object obj, CultureInfo info)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

    public void BadLast()
    {
      Last(null);
    }

    public void CorrectLast()
    {
      Last(null, null);
    }

    // some bad boys
    public string ImplicitIntToString(int index, int maxIndex)
    {
      var nameStart = new string[] { "{" + index + ":" };
      return maxIndex == 1 ? "VALUE" : "VALUE" + (index + 1) + nameStart;
    }

    public string ImplicitIntToString2(int index, int maxIndex)
    {
      var nameStart = new string[] { "{" + index.ToString(CultureInfo.InvariantCulture) + ":" };
      return maxIndex == 1 ? "VALUE" : "VALUE" + (index + 1) + nameStart;
    }

    public int NestedCall(int maxValueCount)
    {
      if (maxValueCount > 1)
        throw new ArgumentException(
            string.Format("Cannot provide maxValueCount of {0} for OptionValueType.None.", maxValueCount),
            nameof(maxValueCount));
      if (maxValueCount < 0)
        throw new ArgumentException(
            string.Format("Cannot provide maxValueCount of {0} for OptionValueType.None.", maxValueCount),
            nameof(maxValueCount));

      return maxValueCount;
    }

    public int NestedCall2(int maxValueCount)
    {
      if (maxValueCount > 1)
        throw new ArgumentException(
            string.Format(CultureInfo.InvariantCulture,
            "Cannot provide maxValueCount of {0} for OptionValueType.None.", maxValueCount),
            nameof(maxValueCount));
      if (maxValueCount < 0)
        throw new ArgumentException(
            string.Format("Cannot provide maxValueCount of {0} for OptionValueType.None.", maxValueCount),
            nameof(maxValueCount));

      return maxValueCount;
    }
  }

  [TestFixture]
  public class PreferIFormatProviderOverrideTest : MethodRuleTestFixture<PreferIFormatProviderOverrideRule>
  {
    [Test]
    public void DoesNotApply()
    {
      AssertRuleDoesNotApply(SimpleMethods.EmptyMethod);
      AssertRuleDoesNotApply(SimpleMethods.ExternalMethod);
    }

    [Test]
    public void Success()
    {
      AssertRuleSuccess<IFormatProviderTestCases>("CorrectEmpty");
      AssertRuleSuccess<IFormatProviderTestCases>("CorrectFirst");
      AssertRuleSuccess<IFormatProviderTestCases>("CorrectLast");

      AssertRuleSuccess<CultureInfoTestCases>("CorrectEmpty");
      AssertRuleSuccess<CultureInfoTestCases>("CorrectFirst");
      AssertRuleSuccess<CultureInfoTestCases>("CorrectLast");
    }

    [Test]
    public void Failure()
    {
      AssertRuleFailure<IFormatProviderTestCases>("BadEmpty", 1);
      AssertRuleFailure<IFormatProviderTestCases>("BadFirst", 1);
      AssertRuleFailure<IFormatProviderTestCases>("BadLast", 1);

      AssertRuleFailure<CultureInfoTestCases>("BadEmpty", 1);
      AssertRuleFailure<CultureInfoTestCases>("BadFirst", 1);
      AssertRuleFailure<CultureInfoTestCases>("BadLast", 1);
      AssertRuleFailure<CultureInfoTestCases>("ImplicitIntToString", 1);
      AssertRuleFailure<CultureInfoTestCases>("NestedCall", 1);
      AssertRuleFailure<CultureInfoTestCases>("ImplicitIntToString2", 1);
      AssertRuleFailure<CultureInfoTestCases>("NestedCall2", 1);
    }

#pragma warning disable IDE0051 // Remove unused private members
    private void Ignored(ResourceManager rm)
#pragma warning restore IDE0051 // Remove unused private members
    {
      rm.GetObject("a");
      rm.GetObject("a", CultureInfo.CurrentCulture);
      rm.GetString("b");
      rm.GetString("b", CultureInfo.InvariantCulture);
    }

#pragma warning disable IDE0051 // Remove unused private members
    private string Params()
#pragma warning restore IDE0051 // Remove unused private members
    {
      // the overload to use is: Format(IFormatProvider, string, params object []);
      return String.Format("{0} {1} {2}", 1, 2, 3);
    }

#pragma warning disable IDE0051 // Remove unused private members
    private void NoSimpleOverload(FieldInfo fi)
#pragma warning restore IDE0051 // Remove unused private members
    {
      // the overload with a CultureInfo is SetValue (object, object, BindingFlags, Binder, CultureInfo);
      // and is not simply an "extra" parameter
      fi.SetValue(new object(), 1);
    }

    [Test]
    public void SpecialCases()
    {
      AssertRuleSuccess<PreferIFormatProviderOverrideTest>("Ignored");
      AssertRuleFailure<PreferIFormatProviderOverrideTest>("Params", 1);
      AssertRuleSuccess<PreferIFormatProviderOverrideTest>("NoSimpleOverload");
    }
  }
}