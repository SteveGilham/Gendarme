// Unit Test for AvoidSwitchStatements Rule.
//
// Authors:
//      Néstor Salceda <nestor.salceda@gmail.com>
//
//      (C) 2008 Néstor Salceda
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
using System.Collections.Generic;

using Test.Rules.Fixtures;
using Test.Rules.Definitions;
using Test.Rules.Helpers;

using Gendarme.Framework;
using Gendarme.Rules.Smells;

using NUnit.Framework;

namespace Test.Rules.Smells
{
  [TestFixture]
  public class AvoidSwitchStatementsTest : MethodRuleTestFixture<AvoidSwitchStatementsRule>
  {
    [Test]
    public void SkipOnBodylessMethodsTest()
    {
      AssertRuleDoesNotApply(SimpleMethods.ExternalMethod);
    }

    [Test]
    public void SuccessOnEmptyMethodsTest()
    {
      AssertRuleSuccess(SimpleMethods.EmptyMethod);
    }

    private void MethodWithSwitch(Severity severity)
    {
      switch (severity)
      {
        case Severity.High:
        case Severity.Low:
        default:
          break;
      }
    }

    [Test]
    [Ignore("Roslyn turns this into a simple branch")]
    public void FailOnMethodWithSwitchTest()
    {
      AssertRuleFailure<AvoidSwitchStatementsTest>("MethodWithSwitch", 1);
    }

    private void MethodWithoutSwitch(Severity severity)
    {
      if (severity == Severity.High || severity == Severity.Low)
        return;
    }

    [Test]
    public void SuccessOnMethodWithoutSwitch()
    {
      AssertRuleSuccess<AvoidSwitchStatementsTest>("MethodWithoutSwitch");
    }

    private IEnumerable<string> MethodWithoutSwitchAndGenerator(IEnumerable<string> enumerable)
    {
      foreach (string value in enumerable)
        yield return value;

      yield return null;
    }

    [Test]
    public void SuccessOnMethodWithoutSwitchAndGeneratorTest()
    {
      // compiler generated code (mono gmcs)
      Type type = Type.GetType("Test.Rules.Smells.AvoidSwitchStatementsTest+<MethodWithoutSwitchAndGenerator>c__Iterator0");
      if (type == null)
      {
        // if not found try the output of MS csc
        type = Type.GetType("Test.Rules.Smells.AvoidSwitchStatementsTest+<MethodWithoutSwitchAndGenerator>d__0");
        if (type == null) // Roslyn
          type = Type.GetType("Test.Rules.Smells.AvoidSwitchStatementsTest+<MethodWithoutSwitchAndGenerator>d__6");
      }
      Assert.IsNotNull(type, "compiler generated type name");
      AssertRuleDoesNotApply(type, "MoveNext");
    }

    // if the number of elements in the swicth is small (like <= 6) then CSC (but not GMCS)
    // well generate code using String::op_Equality instead of a switch IL statement
    private void SwitchWithStrings(string sample)
    {
      switch (sample)
      {
        case "Foo":
        case "Bar":
          return;

        case "Baz":
        case "Bad":
          throw new ArgumentException("sample");
        case "Zoo":
        case "Yoo":
          throw new FormatException("sample");
        case "*":
          break;
      }
    }

    [Test]
    [Ignore("Roslyn uses conditional branches -- maybe test for <PrivateImplementationDetails>.ComputeStringHash(sample)")]
    public void FailOnSwitchWithStringsTest()
    {
      AssertRuleFailure<AvoidSwitchStatementsTest>("SwitchWithStrings", 1);
    }

    [Test]
    public void FSharpIgnoreGenerated()
    {
      Type[] args = new Type[] { typeof(AvoidMultidimensionalIndexer.DotNet.CLIArgs) };
      this.AssertRuleDoesNotApply<AvoidMultidimensionalIndexer.DotNet.CLIArgs>("Equals", args);
    }

    [Test]
    public void FSharpIgnoreTagMatch()
    {
      this.AssertRuleSuccess<AvoidMultidimensionalIndexer.DotNet.CLIArgs>("get_ForceDelete");
    }

    [Test]
    public void FSharpIgnoreInModules()
    {
      this.AssertRuleSuccess(typeof(AvoidSwitchStatements.Counter), "updateReport");
    }
  }
}