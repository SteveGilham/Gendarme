//
// Unit Test for ConsiderConvertingFieldToNullable Rule.
//
// Authors:
//      Cedric Vivier <cedricv@neonux.com>
//
//      (C) 2008 Cedric Vivier
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

using Mono.Cecil;

using Gendarme.Rules.Design;

using NUnit.Framework;
using Test.Rules.Fixtures;
using Test.Rules.Helpers;

namespace Test.Rules.Design
{
#pragma warning disable 169

  public class ClassWithOnePotentialNullable
  {
    private bool hasFoo;
    private int foo;
    private int bar;
  }

  public class ClassWithThreePotentialNullable
  {
    private bool hasFoo;
    private int foo;
    private bool _hasBar;
    private double _bar;
    private bool m_hasIstic;
    private double m_istic;
  }

  public class ClassWithoutPotentialNullable
  {
    private int hasFoo;
    private int foo;
  }

  public class ClassWithoutPotentialNullable2
  {
    private bool hasFoo;
    private int bar;
    private int food;
  }

  public class ClassWithoutPotentialNullable3
  {
    private bool hasFoo;
    private string foo;
  }

  public class ClassWithProperNullable
  {
    private int? foo;
  }

  public class ClassWithProperNullable2
  {
    private bool hasFoo;
    private int? foo;
  }

  public enum SomeEnum
  {
    HasValue,
    Value
  }

  public class ClassWithSmallFieldNames
  {
    private bool has;
    private string foo;
  }

  public class ClassWithNonHasBool
  {
    private bool initialized;
    private string foo;
  }

#pragma warning restore 169

  [TestFixture]
  public class ConsiderConvertingFieldToNullableTest : TypeRuleTestFixture<ConsiderConvertingFieldToNullableRule>
  {
    [Test]
    public void ClassesWithPotentialNullable()
    {
      AssertRuleFailure<ClassWithOnePotentialNullable>(1);
      AssertRuleFailure<ClassWithThreePotentialNullable>(3);
    }

    [Test]
    public void ClassesWithoutPotentialNullable()
    {
      AssertRuleSuccess<ClassWithoutPotentialNullable>();
      AssertRuleSuccess<ClassWithoutPotentialNullable2>();
      AssertRuleSuccess<ClassWithoutPotentialNullable3>();
      AssertRuleSuccess<ClassWithSmallFieldNames>();
      AssertRuleSuccess<ClassWithNonHasBool>();
    }

    [Test]
    public void ClassesWithProperNullable()
    {
      AssertRuleSuccess<ClassWithProperNullable>();
      AssertRuleSuccess<ClassWithProperNullable2>();
    }

    [Test]
    public void TypesWhichDoNotApply()
    {
      AssertRuleDoesNotApply<SomeEnum>();
    }

    [Test]
    public void NotApplicableBefore2_0()
    {
      // ensure that the rule does not apply for types defined in 1.x assemblies
      TypeDefinition violator = DefinitionLoader.GetTypeDefinition<ClassWithOnePotentialNullable>();
      TargetRuntime realRuntime = violator.Module.Runtime;
      try
      {
        // fake assembly runtime version and do the check
        violator.Module.Runtime = TargetRuntime.Net_1_1;
        Rule.Active = true;
        Rule.Initialize(Runner);
        (Runner as TestRunner).OnModule(violator.Module);
        Assert.That(!Rule.Active, "Active");
      }
      catch
      {
        // rollback
        violator.Module.Runtime = realRuntime;
        Rule.Active = true;
      }
    }
  }
}