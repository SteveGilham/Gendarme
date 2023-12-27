//
// Unit test for ReviewCastOnIntegerMultiplicationRule
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
  public class ReviewCastOnIntegerMultiplicationTest : MethodRuleTestFixture<ReviewCastOnIntegerMultiplicationRule>
  {
    [Test]
    public void DoesNotApply()
    {
      // no IL
      AssertRuleDoesNotApply(SimpleMethods.ExternalMethod);
      // no MUL
      AssertRuleDoesNotApply(SimpleMethods.EmptyMethod);
    }

    // a multiplication result is either an [U]Int32 or [U]Int64 type
    //
    // Operands		Result		Test Case		Notes
    // ----------------------------------------------------------------------------------
    // Sbyte * Sbyte	Int32		NoProblem		No overflow possible
    // Byte * Byte		Int32		NoProblem		No overflow possible
    // Int16 * Int16	Int32		NoProblem		No overflow possible
    // UInt16 * UInt16	Int32		CastIntoInt64		Overflow possible [1]
    // Int32 * Int32	Int32		CastIntoInt64		Overflow possible [1]
    // UInt32 * UInt32	UInt32		CastIntoUInt64		Overflow possible [2]
    // Int64 * Int64	Int64		CannotCast		Overflow possible [3]
    // UInt64 * UInt64	UInt64		CannotCast		Overflow possible [3]
    //
    // [1] casting operands, *not* the result, into a [U]Int64 will solve the potential overflow
    // [2] casting operands, *not* the result, into a UInt64 will solve the potential overflow
    // [3] result may overflow, casting to a bigger size is not possible (or would not solve the issue), but you can use 'checked' to get an exception.

    private int MulSByte(sbyte a, sbyte b)
    {
      // no conv* instruction is needed
      return a * b;
    }

    private int MulByte(byte a, byte b)
    {
      // no conv* instruction is needed (even with the unneeded cast)
      return (int)a * b;
    }

    private int MulInt16(short a, short b)
    {
      // no conv* instruction is needed (even with the unneeded cast)
      return (int)(a * b);
    }

    [Test]
    public void NoProblem()
    {
      Assert.That(SByte.MaxValue * SByte.MaxValue <= Int32.MaxValue, "SByte*SByte");
      AssertRuleSuccess<ReviewCastOnIntegerMultiplicationTest>("MulSByte");

      Assert.That(Byte.MaxValue * Byte.MaxValue <= Int32.MaxValue, "Byte*Byte");
      AssertRuleSuccess<ReviewCastOnIntegerMultiplicationTest>("MulByte");

      Assert.That(Int16.MaxValue * Int16.MaxValue <= Int32.MaxValue, "Int16*Int16");
      AssertRuleSuccess<ReviewCastOnIntegerMultiplicationTest>("MulInt16");
    }

    private long BadMulUInt16(ushort a, ushort b)
    {
      // ldarg.1 ldarg.2 mul conv.i8
      return unchecked(a * b);
    }

    private long GoodMulUInt16(ushort a, ushort b)
    {
      // ldarg.1 conv.i8 ldarg.2 conv.i8 mul
      return unchecked((long)a * b);
    }

    private long BadMulInt32(int a, int b)
    {
      // ldarg.1 ldarg.2 mul conv.i8
      return unchecked(a * b);
    }

    private long GoodMulInt32(int a, int b)
    {
      // ldarg.1 conv.i8 ldarg.2 conv.i8 mul
      return unchecked((long)a * b);
    }

    private long CheckedBadMulInt32(int a, int b)
    {
      // ldarg.1 ldarg.2 mul conv.i8
      return a * b;
    }

    private long CheckedGoodMulInt32(int a, int b)
    {
      // ldarg.1 conv.i8 ldarg.2 conv.i8 mul
      return (long)a * b;
    }

    protected void AssertAreEqual(object a, object b, string c)
    {
      Assert.That(a, Is.EqualTo(b), c);
    }

    public void CastIntoInt64()
    {
      unchecked
      {
        // since UInt16 * UInt16 is not garanteed to fit in a Int32...
        Assert.That((long)UInt16.MaxValue * UInt16.MaxValue <= Int64.MaxValue, "UInt16*UInt16");
        // ... people will often cast the result into an Int64
        AssertAreEqual(-131071, BadMulUInt16(UInt16.MaxValue, UInt16.MaxValue), "BadMulInt16");
        AssertRuleFailure<ReviewCastOnIntegerMultiplicationTest>("BadMulUInt16", 1);
        AssertAreEqual(4294836225, GoodMulUInt16(UInt16.MaxValue, UInt16.MaxValue), "GoodMulInt16");
        AssertRuleSuccess<ReviewCastOnIntegerMultiplicationTest>("GoodMulUInt16");

        // since Int32 * Int32 is not garanteed to fit in a [U]Int32...
        Assert.That((long)Int32.MaxValue * Int32.MaxValue <= Int64.MaxValue, "Int32*Int32");
        // ... people will *very* often cast the result into an Int64
        AssertAreEqual(1, BadMulInt32(Int32.MaxValue, Int32.MaxValue), "BadMulInt32");
        AssertRuleFailure<ReviewCastOnIntegerMultiplicationTest>("BadMulInt32", 1);
        AssertAreEqual(4611686014132420609, GoodMulInt32(Int32.MaxValue, Int32.MaxValue), "GoodMulInt32");
        AssertRuleSuccess<ReviewCastOnIntegerMultiplicationTest>("GoodMulInt32");

        AssertRuleFailure<ReviewCastOnIntegerMultiplicationTest>("CheckedBadMulInt32", 1);
        AssertRuleSuccess<ReviewCastOnIntegerMultiplicationTest>("CheckedGoodMulInt32");
      }
    }

    private ulong BadMulUInt32(uint a, uint b)
    {
      // ldarg.1 ldarg.2 mul conv.u8
      return unchecked(a * b);
    }

    private ulong GoodMulUInt32(uint a, uint b)
    {
      // ldarg.1 conv.u8 ldarg.2 conv.u8 mul
      return unchecked((ulong)a * b);
    }

    private ulong CheckedBadMulUInt32(uint a, uint b)
    {
      // ldarg.1 ldarg.2 mul conv.u8
      return a * b;
    }

    private ulong CheckedGoodMulUInt32(uint a, uint b)
    {
      // ldarg.1 conv.u8 ldarg.2 conv.u8 mul
      return (ulong)a * b;
    }

    [Test]
    public void CastIntoUInt64()
    {
      unchecked
      {
        // since UInt32 * UInt32 is not garanteed to fit in a Int64...
        Assert.That((ulong)UInt32.MaxValue * UInt32.MaxValue <= UInt64.MaxValue, "UInt32*UInt32");
        // ... people will often cast the result into an UInt64
        AssertAreEqual(1, BadMulUInt32(UInt32.MaxValue, UInt32.MaxValue), "BadMulInt32");
        AssertRuleFailure<ReviewCastOnIntegerMultiplicationTest>("BadMulUInt32", 1);
        AssertAreEqual(18446744065119617025, GoodMulUInt32(UInt32.MaxValue, UInt32.MaxValue), "GoodMulInt32");
        AssertRuleSuccess<ReviewCastOnIntegerMultiplicationTest>("GoodMulUInt32");

        AssertRuleFailure<ReviewCastOnIntegerMultiplicationTest>("CheckedBadMulUInt32", 1);
        AssertRuleSuccess<ReviewCastOnIntegerMultiplicationTest>("CheckedGoodMulUInt32");
      }
    }

    private long MulInt64(long a, long b)
    {
      return unchecked(a * b);
    }

    private long CheckedMulInt64(long a, long b, ref bool exception)
    {
      try
      {
        exception = false;
        checked { return a * b; } // mul.ovf
      }
      catch (OverflowException)
      {
        exception = true;
        return 0;
      }
    }

    private ulong MulUInt64(ulong a, ulong b)
    {
      return unchecked(a * b);
    }

    private ulong CheckedUMulInt64(ulong a, ulong b, ref bool exception)
    {
      try
      {
        exception = false;
        checked { return a * b; } // mul.ovf.un
      }
      catch (OverflowException)
      {
        exception = true;
        return 0;
      }
    }

    [Test]
    public void CannotCast()
    {
      unchecked
      {
        AssertAreEqual(1, MulInt64(Int64.MaxValue, Int64.MaxValue), "Int64*Int64");
        AssertRuleSuccess<ReviewCastOnIntegerMultiplicationTest>("MulInt64");

        // note: casting a long into ulong would not solve the issue either
        AssertAreEqual(1, MulUInt64((ulong)Int64.MaxValue, (ulong)Int64.MaxValue), "UInt64*Int64");
        AssertAreEqual(1, MulUInt64(UInt64.MaxValue, UInt64.MaxValue), "UInt64*UInt64");
        AssertRuleSuccess<ReviewCastOnIntegerMultiplicationTest>("MulUInt64");

        bool exception = false;
        AssertAreEqual(0, CheckedMulInt64(Int64.MaxValue, Int64.MaxValue, ref exception), "Checked-Int64*Int64");
        Assert.That(exception, "Exception-Int64*Int64");

        exception = false;
        AssertAreEqual(0, CheckedUMulInt64(UInt64.MaxValue, UInt64.MaxValue, ref exception), "Checked-UInt64*UInt64");
        Assert.That(exception, "Exception-UInt64*UInt64");
        AssertRuleSuccess<ReviewCastOnIntegerMultiplicationTest>("CheckedUMulInt64");
      }
    }

    public long MulDoubleIntToLong(double a, int b)
    {
      return (long)(a * b);
    }

    [Test]
    public void FloatingPoint()
    {
      AssertRuleSuccess<ReviewCastOnIntegerMultiplicationTest>("MulDoubleIntToLong");
    }
  }
}