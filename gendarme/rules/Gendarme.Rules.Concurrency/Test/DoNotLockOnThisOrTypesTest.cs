//
// Unit tests for DoNotLockOnThisOrTypesRule
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
using System.Collections.Generic;

using Gendarme.Rules.Concurrency;

using NUnit.Framework;
using Test.Rules.Definitions;
using Test.Rules.Fixtures;

namespace Test.Rules.Concurrency
{
  [TestFixture]
  public class DoNotLockOnThisOrTypesTest : MethodRuleTestFixture<DoNotLockOnThisOrTypesRule>
  {
    [Test]
    public void DoesNotApply()
    {
      // no IL for p/invokes
      AssertRuleDoesNotApply(SimpleMethods.ExternalMethod);
      // no calls[virt]
      AssertRuleDoesNotApply(SimpleMethods.EmptyMethod);
    }

    private static Dictionary<string, Type> cache = new Dictionary<string, Type>();

    public bool LockThis(string s)
    {
      lock (this)
      {
        return cache.ContainsKey(s);
      }
    }

    [Test]
    public void This()
    {
      /* IL now looks like

  IL_0000: nop
	// lock (this)
	IL_0001: ldarg.0
	IL_0002: stloc.0 <= fail to find this
	// {
	IL_0003: ldc.i4.0
	IL_0004: stloc.1
	.try
	{
		IL_0005: ldloc.0 <= identify this
		IL_0006: ldloca.s 1
		// (no C# code)
		IL_0008: call void [mscorlib]System.Threading.Monitor::Enter(object, bool&)

       */
      AssertRuleFailure<DoNotLockOnThisOrTypesTest>("LockThis");
    }

    public bool LockType(string s)
    {
      lock (typeof(DoNotLockOnThisOrTypesTest))
      {
        return cache.ContainsKey(s);
      }
    }

    public bool LockTypes(string s)
    {
      lock (typeof(DoNotLockOnThisOrTypesTest))
      {
        lock (s.GetType())
        {
          return cache.ContainsKey(s);
        }
      }
    }

    [Test]
    public void Type()
    {
      AssertRuleFailure<DoNotLockOnThisOrTypesTest>("LockType", 1);
    }

    [Test]
    public void Types()
    {
      AssertRuleFailure<DoNotLockOnThisOrTypesTest>("LockTypes", 2);
    }

    public static bool StaticLockType(string s)
    {
      lock (typeof(DoNotLockOnThisOrTypesTest))
      {
        return cache.ContainsKey(s);
      }
    }

    public static bool StaticLockTypes(string s)
    {
      lock (typeof(DoNotLockOnThisOrTypesTest))
      {
        lock (s.GetType())
        {
          return cache.ContainsKey(s);
        }
      }
    }

    private static bool TryEnter(object obj)
    {
      lock (obj)
      {
        Console.WriteLine();
      }
      return true;
    }

    [Test]
    public void StaticType()
    {
      /*
  IL_0000: nop
	// lock (typeof(DoNotLockOnThisOrTypesTest))
	IL_0001: ldtoken Test.Rules.Concurrency.DoNotLockOnThisOrTypesTest
	IL_0006: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
	IL_000b: stloc.0
	// {
	IL_000c: ldc.i4.0
	IL_000d: stloc.1
	.try
	{
		IL_000e: ldloc.0
		IL_000f: ldloca.s 1
		// (no C# code)
		IL_0011: call void [mscorlib]System.Threading.Monitor::Enter(object, bool&)
       */

      AssertRuleFailure<DoNotLockOnThisOrTypesTest>("StaticLockType", 1);
    }

    [Test]
    public void StaticTypes()
    {
      AssertRuleFailure<DoNotLockOnThisOrTypesTest>("StaticLockTypes", 2);
    }

    [Test]
    public void TryEnterTest()
    {
      AssertRuleSuccess<DoNotLockOnThisOrTypesTest>("TryEnter");
    }

    private object instance_locker = new object();
    private static object static_locker = new object();

    public bool LockInstanceObject(string s)
    {
      lock (instance_locker)
      {
        return cache.ContainsKey(s);
      }
    }

    public bool LockStaticObject(string s)
    {
      lock (static_locker)
      {
        return cache.ContainsKey(s);
      }
    }

    public bool NoLock(string s)
    {
      return cache.ContainsKey(s);
    }

    [Test]
    public void Instance()
    {
      AssertRuleSuccess<DoNotLockOnThisOrTypesTest>("LockInstanceObject");
      AssertRuleSuccess<DoNotLockOnThisOrTypesTest>("LockStaticObject");
      AssertRuleSuccess<DoNotLockOnThisOrTypesTest>("NoLock");
    }

    public static bool StaticLockStaticObject(string s)
    {
      lock (static_locker)
      {
        return cache.ContainsKey(s);
      }
    }

    public static bool StaticNoLock(string s)
    {
      return cache.ContainsKey(s);
    }

    [Test]
    public void Static()
    {
      AssertRuleSuccess<DoNotLockOnThisOrTypesTest>("StaticLockStaticObject");
      AssertRuleSuccess<DoNotLockOnThisOrTypesTest>("StaticNoLock");
    }

    private abstract class Base
    {
      protected object locker = new object();

      public object Locker
      {
        get { return locker; }
      }
    }

    private class Concrete : Base
    {
      private void LockField(string s)
      {
        try
        {
          lock (base.locker)
          {
            Console.WriteLine(s);
          }
        }
        catch
        {
        }
      }

      private void LockProperty(string s)
      {
        try
        {
          lock (base.Locker)
          {
            Console.WriteLine(s);
          }
        }
        catch
        {
        }
      }
    }

    [Test]
    public void CallingBase()
    {
      AssertRuleSuccess<Concrete>("LockField");
      AssertRuleSuccess<Concrete>("LockProperty");
    }
  }
}