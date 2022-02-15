//
// Unit tests for TypesWithNativeFieldsShouldBeDisposableRule
//
// Authors:
//	Andreas Noever <andreas.noever@gmail.com>
//	Sebastien Pouliot  <sebastien@ximian.com>
//
//  (C) 2008 Andreas Noever
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
using System.Runtime.InteropServices;

using Gendarme.Rules.Correctness;

using NUnit.Framework;
using Test.Rules.Definitions;
using Test.Rules.Fixtures;

namespace Test.Rules.Correctness
{
#pragma warning disable IDE0044
#pragma warning disable IDE0051
#pragma warning disable IDE0052

  internal class NoNativeFields
  {
    private int A;
    private object b;
  }

  internal class NativeFieldsImplementsIDisposeable : IDisposable
  {
    private object A;
    private IntPtr B;

    public void Dispose()
    {
      throw new NotImplementedException();
    }
  }

  internal class NativeFieldsExplicit : IDisposable
  {
    private object A;
    private IntPtr B;

    void IDisposable.Dispose()
    {
      throw new NotImplementedException();
    }
  }

  internal class NativeFieldsIntPtr : ICloneable
  {
    private object A;
    private IntPtr B;

    public object Clone()
    {
      throw new NotImplementedException();
    }
  }

  internal class NativeFieldsIntPtrAssigned : ICloneable
  {
    private object A;
    private IntPtr B;

    public NativeFieldsIntPtrAssigned()
    {
      B = IntPtr.Zero;
    }

    public object Clone()
    {
      throw new NotImplementedException();
    }
  }

  internal class NativeFieldsIntPtrAllocated : ICloneable
  {
    private object A;
    private IntPtr B;

    public NativeFieldsIntPtrAllocated()
    {
      B = Marshal.AllocCoTaskMem(1);
    }

    public object Clone()
    {
      throw new NotImplementedException();
    }
  }

  internal class NativeFieldsUIntPtr : ICloneable
  {
    private object A;
    private UIntPtr B;

    public object Clone()
    {
      throw new NotImplementedException();
    }
  }

  internal class NativeFieldsUIntPtrAssigned : ICloneable
  {
    private object A;
    private UIntPtr B;

    public NativeFieldsUIntPtrAssigned()
    {
      B = (UIntPtr)0x1f00;
    }

    public object Clone()
    {
      throw new NotImplementedException();
    }
  }

  internal class NativeFieldsUIntPtrAllocated : ICloneable
  {
    private object A;
    private UIntPtr B;

    [DllImport("liberty")]
    private static extern UIntPtr Alloc(int x);

    public NativeFieldsUIntPtrAllocated()
    {
      B = Alloc(1);
    }

    public object Clone()
    {
      throw new NotImplementedException();
    }
  }

  internal class NativeFieldsHandleRef : ICloneable
  {
    private object A;
    private HandleRef B;

    public object Clone()
    {
      throw new NotImplementedException();
    }
  }

  internal class NativeFieldsHandleRefAssigned : ICloneable
  {
    private object A;
    private HandleRef B;

    public NativeFieldsHandleRefAssigned()
    {
      GCHandle handle = GCHandle.Alloc(A, GCHandleType.Pinned);
      B = new HandleRef(handle, handle.AddrOfPinnedObject());
    }

    public object Clone()
    {
      throw new NotImplementedException();
    }
  }

  internal class NativeFieldsHandleRefAllocatedElsewhere : ICloneable
  {
    private object A;
    private HandleRef B;

    private HandleRef GetHandleReference()
    {
      return new HandleRef(A, IntPtr.Zero);
    }

    public NativeFieldsHandleRefAllocatedElsewhere()
    {
      // fxcop does not trigger on this (or similar cases)
      B = GetHandleReference();
    }

    public object Clone()
    {
      throw new NotImplementedException();
    }
  }

  internal abstract class AbstractNativeFields : IDisposable
  {
    private object A;
    private HandleRef B;

    public abstract void Dispose();
  }

  internal abstract class AbstractNativeFields2 : IDisposable
  {
    private object A;
    private HandleRef B;

    public abstract void Dispose();

    void IDisposable.Dispose()
    {
      throw new NotImplementedException();
    }
  }

  internal class NativeFieldsArray : ICloneable
  {
    private object A;
    private IntPtr[] B;

    public object Clone()
    {
      B = new IntPtr[1];
      // assignation (newobj+stfld) does not need to to be inside ctor
      // note: fxcop does not catch this one
      B[0] = Marshal.AllocCoTaskMem(1);
      A = B;
      return A;
    }
  }

  internal struct StructWithNativeFields
  {
    public IntPtr a;
    public UIntPtr b;
    public HandleRef c;
  }

  internal class NativeStaticFieldsArray
  {
    private object A;
    private static UIntPtr[] B;
  }

  [TestFixture]
  public class TypesWithNativeFieldsShouldBeDisposableTest : TypeRuleTestFixture<TypesWithNativeFieldsShouldBeDisposableRule>
  {
    [Test]
    public void DoesNotApply()
    {
      AssertRuleDoesNotApply(SimpleTypes.Delegate);
      AssertRuleDoesNotApply(SimpleTypes.Enum);
      AssertRuleDoesNotApply(SimpleTypes.Interface);
      AssertRuleDoesNotApply(SimpleTypes.Structure);
    }

    [Test]
    public void TestNoNativeFields()
    {
      AssertRuleSuccess<NoNativeFields>();
    }

    [Test]
    public void TestNativeFieldsImplementsIDisposeable()
    {
      AssertRuleSuccess<NativeFieldsImplementsIDisposeable>();
    }

    [Test]
    public void TestNativeFieldsExplicit()
    {
      AssertRuleSuccess<NativeFieldsExplicit>();
    }

    [Test]
    public void TestNativeFieldsIntPtr()
    {
      AssertRuleSuccess<NativeFieldsIntPtr>();
      AssertRuleSuccess<NativeFieldsIntPtrAssigned>();
      AssertRuleFailure<NativeFieldsIntPtrAllocated>(1);
    }

    [Test]
    public void TestNativeFieldsUIntPtr()
    {
      AssertRuleSuccess<NativeFieldsUIntPtr>();
      AssertRuleSuccess<NativeFieldsUIntPtrAssigned>();
      AssertRuleFailure<NativeFieldsUIntPtrAllocated>(1);
    }

    [Test]
    public void TestNativeFieldsHandleRef()
    {
      AssertRuleSuccess<NativeFieldsHandleRef>();
      AssertRuleSuccess<NativeFieldsHandleRefAssigned>();
      AssertRuleFailure<NativeFieldsHandleRefAllocatedElsewhere>(1);
    }

    [Test]
    public void TestAbstractNativeFields()
    {
      AssertRuleFailure<AbstractNativeFields>(1);
      AssertRuleFailure<AbstractNativeFields2>(1);
    }

    [Test]
    public void TestNativeFieldsArray()
    {
      AssertRuleFailure<NativeFieldsArray>(1);
    }

    [Test]
    public void TestStructWithNativeFields()
    {
      AssertRuleDoesNotApply<StructWithNativeFields>();
    }

    [Test]
    public void TestNativeStaticFieldsArray()
    {
      AssertRuleSuccess<NativeStaticFieldsArray>();
    }
  }
}