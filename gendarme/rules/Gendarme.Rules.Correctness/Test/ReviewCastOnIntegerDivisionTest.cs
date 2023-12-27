//
// Unit test for ReviewCastOnIntegerDivisionRule
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
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

using Gendarme.Rules.Correctness;

using NUnit.Framework;
using Test.Rules.Definitions;
using Test.Rules.Fixtures;

namespace Test.Rules.Correctness
{
  [TestFixture]
  public class ReviewCastOnIntegerDivisionTest : MethodRuleTestFixture<ReviewCastOnIntegerDivisionRule>
  {
    [Test]
    public void DoesNotApply()
    {
      // no IL
      AssertRuleDoesNotApply(SimpleMethods.ExternalMethod);
      // no DIV[_UN]
      AssertRuleDoesNotApply(SimpleMethods.EmptyMethod);
    }

    public double DoubleBadIntDivParameters(int a, int b)
    {
      // this compiles into: ldarg.1 ldarg2 div conv.r8
      // so the division is made on integers, then the result is casted into a double
      return a / b;
    }

    public double DoubleCorrectIntDivParameters(int a, int b)
    {
      // this compiles into: ldarg.1 conv.r8 ldarg2 conv.r8 div
      // so the division is made on doubles
      return (double)a / b;
    }

    public float FloatBadIntDivParameters(short a, short b)
    {
      return a / b;
    }

    public float FloatCorrectIntDivParameters(short a, short b)
    {
      return (float)a / b;
    }

    protected void AssertAreEqual(object a, object b, string c)
    {
      Assert.That(a, Is.EqualTo(b), c);
    }

    protected void AssertAreEqual(object a, object b, double delta, string c)
    {
      Assert.That(a, Is.EqualTo(b).Within(delta), c);
    }

    [Test]
    public void Parameters()
    {
      AssertAreEqual(1.0d, DoubleBadIntDivParameters(3, 2), "3/2==1d");
      AssertRuleFailure<ReviewCastOnIntegerDivisionTest>("DoubleBadIntDivParameters");

      AssertAreEqual(1.5d, DoubleCorrectIntDivParameters(3, 2), "3/2==1.5d");
      AssertRuleSuccess<ReviewCastOnIntegerDivisionTest>("DoubleCorrectIntDivParameters");

      AssertAreEqual(1.0f, FloatBadIntDivParameters(3, 2), "3/2==1f");
      AssertRuleFailure<ReviewCastOnIntegerDivisionTest>("FloatBadIntDivParameters");

      AssertAreEqual(1.5f, FloatCorrectIntDivParameters(3, 2), "3/2==1.5f");
      AssertRuleSuccess<ReviewCastOnIntegerDivisionTest>("FloatCorrectIntDivParameters");
    }

    private static byte sa = 5;
    private static byte sb = 2;
    private static double sdr;

    private static void DoubleBadIntDivStaticFields()
    {
      sdr = sa / sb;
    }

    private static void DoubleGoodIntDivStaticFields()
    {
      sdr = sa / (double)sb;
    }

    private static sbyte ssa = 5;
    private static sbyte ssb = 2;
    private static float sfr;

    private static void FloatBadIntDivStaticFields()
    {
      sfr = ssa / ssb;
    }

    private static void FloatIntDivStaticFields()
    {
      sfr = ssa / (float)ssb;
    }

    [Test]
    public void StaticFields()
    {
      DoubleBadIntDivStaticFields();
      AssertAreEqual(2.0d, sdr, "5/2==2d");
      AssertRuleFailure<ReviewCastOnIntegerDivisionTest>("DoubleBadIntDivStaticFields");

      DoubleGoodIntDivStaticFields();
      AssertAreEqual(2.5d, sdr, "3/2==1.5d");
      AssertRuleSuccess<ReviewCastOnIntegerDivisionTest>("DoubleGoodIntDivStaticFields");

      FloatBadIntDivStaticFields();
      AssertAreEqual(2.0d, sfr, "5/2==2f");
      AssertRuleFailure<ReviewCastOnIntegerDivisionTest>("FloatBadIntDivStaticFields");

      FloatIntDivStaticFields();
      AssertAreEqual(2.5d, sfr, "3/2==1.5f");
      AssertRuleSuccess<ReviewCastOnIntegerDivisionTest>("FloatIntDivStaticFields");
    }

    private uint ia = 10;
    private uint ib = 3;
    private double idr;

    private void DoubleBadIntDivFields()
    {
      idr = ia / ib;
    }

    private void DoubleGoodIntDivFields()
    {
      idr = (double)ia / (double)ib;
    }

    private ushort iua = 10;
    private ushort iub = 3;
    private float ifr;

    private void FloatBadIntDivFields()
    {
      ifr = iua / iub;
    }

    private void FloatGoodIntDivFields()
    {
      ifr = (float)iua / (float)iub;
    }

    [Test]
    public void Fields()
    {
      DoubleBadIntDivFields();
      AssertAreEqual(3.0d, idr, "10/3==3d");
      AssertRuleFailure<ReviewCastOnIntegerDivisionTest>("DoubleBadIntDivFields");

      DoubleGoodIntDivFields();
      AssertAreEqual(3.33d, idr, 0.1d, "10/3==3.33d");
      AssertRuleSuccess<ReviewCastOnIntegerDivisionTest>("DoubleGoodIntDivFields");

      FloatBadIntDivFields();
      AssertAreEqual(3.0f, ifr, "10/3==3f");
      AssertRuleFailure<ReviewCastOnIntegerDivisionTest>("FloatBadIntDivFields");

      FloatGoodIntDivFields();
      AssertAreEqual(3.33f, ifr, 0.1f, "10/3==3.33f");
      AssertRuleSuccess<ReviewCastOnIntegerDivisionTest>("FloatGoodIntDivFields");
    }

    private double DoubleBadIntDivLocals()
    {
      long a = 42;
      return a / 4;
    }

    private double DoubleGoodIntDivLocals()
    {
      long b = 4;
      return 42 / (double)b;
    }

    private float FloatBadIntDivLocals()
    {
      ulong a = 42;
      return a / 4;
    }

    private float FloatGoodIntDivLocals()
    {
      ulong b = 4;
      return 42 / (float)b;
    }

    [Test]
    public void Locals()
    {
      AssertAreEqual(10.0d, DoubleBadIntDivLocals(), "42/4==10d");
      AssertRuleFailure<ReviewCastOnIntegerDivisionTest>("DoubleBadIntDivLocals");

      AssertAreEqual(10.5d, DoubleGoodIntDivLocals(), 0.1d, "42/4==10.5d");
      AssertRuleSuccess<ReviewCastOnIntegerDivisionTest>("DoubleGoodIntDivLocals");

      AssertAreEqual(10.0f, FloatBadIntDivLocals(), "42/4==10f");
      AssertRuleFailure<ReviewCastOnIntegerDivisionTest>("FloatBadIntDivLocals");

      AssertAreEqual(10.5f, FloatGoodIntDivLocals(), 0.1d, "42/4==10.5f");
      AssertRuleSuccess<ReviewCastOnIntegerDivisionTest>("FloatGoodIntDivLocals");
    }

    private double DivD(float a, float b)
    {
      return a / b;
    }

    private float DivF(double a, double b)
    {
      return (float)(a / b);
    }

    [Test]
    public void DoubleFloatCasting()
    {
      AssertRuleSuccess<ReviewCastOnIntegerDivisionTest>("DivD");
      AssertRuleSuccess<ReviewCastOnIntegerDivisionTest>("DivF");
    }

    private double DoubleBadArray(int[] array)
    {
      return array[0] / array[1];
    }

    private double DoubleGoodArray(double[] array)
    {
      return array[0] / array[1];
    }

    private float FloatBadArray(short[] array)
    {
      return array[0] / array[1];
    }

    private float FloatGoodArray(float[] array)
    {
      return array[0] / array[1];
    }

    [Test]
    public void Array()
    {
      AssertRuleFailure<ReviewCastOnIntegerDivisionTest>("DoubleBadArray", 1);
      AssertRuleSuccess<ReviewCastOnIntegerDivisionTest>("DoubleGoodArray");

      AssertRuleFailure<ReviewCastOnIntegerDivisionTest>("FloatBadArray", 1);
      AssertRuleSuccess<ReviewCastOnIntegerDivisionTest>("FloatGoodArray");
    }
  }
}