//
// Unit tests for EqualsShouldHandleNullArgRule
//
// Authors:
//	Nidhi Rawal <sonu2404@gmail.com>
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (c) <2007> Nidhi Rawal
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
using System.Reflection;

using Gendarme.Framework;
using Gendarme.Rules.BadPractice;
using Mono.Cecil;

using NUnit.Framework;
using Test.Rules.Fixtures;

namespace Test.Rules.BadPractice
{
#pragma warning disable 114, 649, 659

  [TestFixture]
  public class EqualsShouldHandleNullArgTest : TypeRuleTestFixture<EqualsShouldHandleNullArgRule>
  {
    public class EqualsChecksForNullArg
    {
      public override bool Equals(object obj)
      {
        if (obj == null)
          return false;
        else
          return this == obj;
      }
    }

    public class EqualsDoesNotReturnFalseForNullArg
    {
      public override bool Equals(object obj)
      {
        if (obj == null)
          return true;

        return this == obj;
      }
    }

    public class EqualsNotOverriddenNotCheckingNull
    {
      public bool Equals(object obj)
      {
        return this == obj;
      }
    }

    public class EqualsNotOverriddenNotReturningFalseForNull
    {
      public new bool Equals(object obj)
      {
        if (obj != null)
          return this == obj;

        return true;
      }
    }

    [Test]
    public void Basic()
    {
      AssertRuleSuccess<EqualsChecksForNullArg>();
      AssertRuleFailure<EqualsDoesNotReturnFalseForNullArg>(1);
      AssertRuleSuccess<EqualsNotOverriddenNotCheckingNull>();
      AssertRuleFailure<EqualsNotOverriddenNotReturningFalseForNull>();
    }

    public class EqualsReturnsFalse
    {
      public override bool Equals(object obj)
      {
        return false;
      }
    }

    public class EqualsReturnsTrue
    {
      public override bool Equals(object obj)
      {
        return true;
      }
    }

    [Test]
    public void Constants()
    {
      AssertRuleSuccess<EqualsReturnsFalse>();
      AssertRuleFailure<EqualsReturnsTrue>(1);
    }

#pragma warning disable CA2231 // Overload operator equals on overriding value type Equals

    public struct EqualsUsingIsReturnFalse
#pragma warning restore CA2231 // Overload operator equals on overriding value type Equals
    {
      public override bool Equals(object obj)
      {
        if (obj is EqualsUsingIsReturnFalse)
#pragma warning disable CA2013 // Do not use ReferenceEquals with value types
          return Object.ReferenceEquals(this, obj);
#pragma warning restore CA2013 // Do not use ReferenceEquals with value types
        return false;
      }
    }

#pragma warning disable CA2231 // Overload operator equals on overriding value type Equals

    public struct EqualsUsingIsReturnTrue
#pragma warning restore CA2231 // Overload operator equals on overriding value type Equals
    {
      public override bool Equals(object obj)
      {
        if (obj is EqualsUsingIsReturnTrue)
#pragma warning disable CA2013 // Do not use ReferenceEquals with value types
          return Object.ReferenceEquals(this, obj);
#pragma warning restore CA2013 // Do not use ReferenceEquals with value types
        return true;
      }
    }

    // from /mcs/class/corlib/System.Reflection.Emit/SignatureToken.cs
#pragma warning disable CA2231 // Overload operator equals on overriding value type Equals

    public struct EqualsUsingIsReturnVariable
#pragma warning restore CA2231 // Overload operator equals on overriding value type Equals
    {
      internal int tokValue;

      public override bool Equals(object obj)
      {
        bool res = obj is EqualsUsingIsReturnVariable;
        if (res)
        {
          EqualsUsingIsReturnVariable that = (EqualsUsingIsReturnVariable)obj;
          res = (this.tokValue == that.tokValue);
        }
        return res;
      }
    }

    [Test]
    public void EqualsUsingIs()
    {
      AssertRuleSuccess<EqualsUsingIsReturnFalse>();
      AssertRuleFailure<EqualsUsingIsReturnTrue>(1);
      AssertRuleSuccess<EqualsUsingIsReturnVariable>();
    }

    public class EqualsCallBase : EqualsReturnsTrue
    {
      public override bool Equals(object obj)
      {
        return base.Equals(obj);
      }
    }

    public class EqualsCheckThis
    {
      // System.Object does this
      public override bool Equals(object obj)
      {
        return (this == obj);
      }
    }

    public class EqualsCheckType
    {
      // common pattern in corlib
      public override bool Equals(object obj)
      {
        if (obj == null || GetType() != obj.GetType())
          return false;
        return true;
      }
    }

    // from /mcs/class/System/System.ComponentModel/DisplayNameAttribute.cs
    public class CheckThisFirst
    {
#pragma warning disable IDE0044 // Add readonly modifier
      private string DisplayName;
#pragma warning restore IDE0044 // Add readonly modifier

      public override bool Equals(object obj)
      {
        if (obj == this)
          return true;

#pragma warning disable IDE0019 // Use pattern matching
        CheckThisFirst dna = obj as CheckThisFirst;
#pragma warning restore IDE0019 // Use pattern matching

        if (dna == null)
          return false;
        return dna.DisplayName == DisplayName;
      }
    }

    [Test]
    public void CommonPatterns()
    {
      AssertRuleSuccess<EqualsCheckThis>();
      AssertRuleSuccess<EqualsCheckType>();
      AssertRuleSuccess<CheckThisFirst>();
    }

    public class StaticEquals
    {
#pragma warning disable IDE0060 // Remove unused parameter

      public static bool Equals(object obj)
#pragma warning restore IDE0060 // Remove unused parameter
      {
        return false;
      }
    }

    public class EqualsTwoParameters
    {
#pragma warning disable CA1822 // Mark members as static

      public new bool Equals(object left, object right)
#pragma warning restore CA1822 // Mark members as static
      {
        return (left == right);
      }
    }

    public class EqualsReference
    {
      public bool Equals(EqualsReference obj)
      {
        return this == obj;
      }
    }

    [Test]
    public void NotApplicable()
    {
      AssertRuleDoesNotApply<StaticEquals>();
      AssertRuleDoesNotApply<EqualsTwoParameters>();
      AssertRuleDoesNotApply<EqualsReference>();
    }

    public class Throw
    {
#pragma warning disable IDE0060 // Remove unused parameter

      public bool Equals(object obj)
#pragma warning restore IDE0060 // Remove unused parameter
      {
        throw new NotSupportedException();
      }
    }

    [Test]
    public void Special()
    {
      AssertRuleSuccess<Throw>();
    }
  }
}