//
// Unit Test for AvoidUnusedParameters Rule.
//
// Authors:
//      Néstor Salceda <nestor.salceda@gmail.com>
//	Sebastien Pouliot <sebastien@ximian.com>
//
//      (C) 2007 Néstor Salceda
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
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;
using Gendarme.Rules.Performance;

using NUnit.Framework;
using Test.Rules.Definitions;
using Test.Rules.Fixtures;
using Test.Rules.Helpers;

namespace Test.Rules.Performance
{
  public abstract class AbstractClass
  {
    public abstract void AbstractMethod(int x);
  }

  public class VirtualClass
  {
    public virtual void VirtualMethod(int x)
    {
    }
  }

  public class OverrideClass : VirtualClass
  {
    public override void VirtualMethod(int x)
    {
    }
  }

  [TestFixture]
  public class AvoidUnusedParametersTest : MethodRuleTestFixture<AvoidUnusedParametersRule>
  {
    [OneTimeSetUp]
    public void SetUp()
    {
      Runner.Engines.Subscribe("Gendarme.Framework.Engines.SuppressMessageEngine");
    }

#pragma warning disable CA1822 // Mark members as static

    public void PrintBannerUsingParameter(Version version)
#pragma warning restore CA1822 // Mark members as static
    {
      Console.WriteLine("Welcome to the foo program {0}", version);
    }

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter

    public void PrintBannerUsingAssembly(Version version)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static
    {
      Console.WriteLine("Welcome to the foo program {0}", Assembly.GetExecutingAssembly().GetName().Version);
    }

#pragma warning disable CA1822 // Mark members as static

    public void PrintBannerWithoutParameters()
#pragma warning restore CA1822 // Mark members as static
    {
      Console.WriteLine("Welcome to the foo program {0}", Assembly.GetExecutingAssembly().GetName().Version);
    }

    [Test]
    public void PrintBanner()
    {
      AssertRuleSuccess<AvoidUnusedParametersTest>("PrintBannerUsingParameter");
      AssertRuleFailure<AvoidUnusedParametersTest>("PrintBannerUsingAssembly", 1);
      AssertRuleDoesNotApply<AvoidUnusedParametersTest>("PrintBannerWithoutParameters");
    }

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter

    public void MethodWithUnusedParameters(IEnumerable enumerable, int x)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static
    {
      Console.WriteLine("Method with unused parameters");
    }

    [Test]
    public void MethodWithUnusedParametersTest()
    {
      AssertRuleFailure<AvoidUnusedParametersTest>("MethodWithUnusedParameters", 2);
    }

#pragma warning disable CA1822 // Mark members as static

    public void MethodWith5UsedParameters(int x, IEnumerable enumerable, string foo, char c, float f)
#pragma warning restore CA1822 // Mark members as static
    {
      Console.WriteLine(f);
      Console.WriteLine(c);
      Console.WriteLine(foo);
      Console.WriteLine(enumerable);
      Console.WriteLine(x);
    }

    [Test]
    public void MethodWith5UsedParametersTest()
    {
      AssertRuleSuccess<AvoidUnusedParametersTest>("MethodWith5UsedParameters");
    }

#pragma warning disable IDE0060 // Remove unused parameter

    public static void StaticMethodWithUnusedParameters(int x, string foo)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

    [Test]
    public void StaticMethodWithUnusedParametersTest()
    {
      AssertRuleFailure<AvoidUnusedParametersTest>("StaticMethodWithUnusedParameters", 2);
    }

    public static void StaticMethodWithUsedParameters(int x, string foo)
    {
      Console.WriteLine(x);
      Console.WriteLine(foo);
    }

    [Test]
    public void StaticMethodWithUsedParametersTest()
    {
      AssertRuleSuccess<AvoidUnusedParametersTest>("StaticMethodWithUsedParameters");
    }

    public static void StaticMethodWith5UsedParameters(int x, string foo, IEnumerable enumerable, char c, float f)
    {
      Console.WriteLine(f);
      Console.WriteLine(c);
      Console.WriteLine(foo);
      Console.WriteLine(enumerable);
      Console.WriteLine(x);
    }

    [Test]
    public void StaticMethodWith5UsedParametersTest()
    {
      AssertRuleSuccess<AvoidUnusedParametersTest>("StaticMethodWith5UsedParameters");
    }

#pragma warning disable IDE0060 // Remove unused parameter

    public static void StaticMethodWith5UnusedParameters(int x, string foo, IEnumerable enumerable, char c, float f)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

    [Test]
    public void StaticMethodWith5UnusedParametersTest()
    {
      AssertRuleFailure<AvoidUnusedParametersTest>("StaticMethodWith5UnusedParameters", 5);
    }

    public delegate void SimpleCallback(int x);

#pragma warning disable CA1822 // Mark members as static

    public void SimpleCallbackImpl(int x)
#pragma warning restore CA1822 // Mark members as static
    {
    }

#pragma warning disable CA1822 // Mark members as static

    public void SimpleCallbackImpl2(int x)
#pragma warning restore CA1822 // Mark members as static
    {
    }

    [Test]
    public void DelegateMethodTest()
    {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
      var callback = new SimpleCallback(SimpleCallbackImpl);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
      AssertRuleDoesNotApply<AvoidUnusedParametersTest>("SimpleCallbackImpl");
    }

    [Test]
    public void DelegateMethodTestWithMultipleDelegates()
    {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
      var callback = new SimpleCallback(SimpleCallbackImpl);
      var callback2 = new SimpleCallback(SimpleCallbackImpl2);
#pragma warning restore IDE0059 // Unnecessary assignment of a value

      AssertRuleDoesNotApply<AvoidUnusedParametersTest>("SimpleCallbackImpl");
      AssertRuleDoesNotApply<AvoidUnusedParametersTest>("SimpleCallbackImpl2");
    }

#pragma warning disable CA1822 // Mark members as static

    public void AnonymousMethodWithUnusedParameters()
#pragma warning restore CA1822 // Mark members as static
    {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable IDE0039 // Use local function
      SimpleCallback callback = delegate (int x)
#pragma warning restore IDE0039 // Use local function
#pragma warning restore IDE0059 // Unnecessary assignment of a value
      {
        //Empty
      };
    }

    [Test]
    public void AnonymousMethodTest()
    {
      MethodDefinition method = null;
      // compiler generated code is compiler dependant, check for [g]mcs (inner type)
      TypeDefinition type = DefinitionLoader.GetTypeDefinition(typeof(AvoidUnusedParametersTest).Assembly, "Test.Rules.Performance.AvoidUnusedParametersTest/<>c");
      if (type != null)
        method = DefinitionLoader.GetMethodDefinition(type, "<AnonymousMethodWithUnusedParameters>b__22_0", null);
      // otherwise try for csc (inside same class)
      // would have to handle argument exception, not jjust null return, to get into this case
      //if (method == null)
      //{
      //  type = DefinitionLoader.GetTypeDefinition<AvoidUnusedParametersTest>();
      //  foreach (MethodDefinition md in type.Methods)
      //  {
      //    if (md.Name.StartsWith("<AnonymousMethodWithUnusedParameters>"))
      //    {
      //      method = md;
      //      break;
      //    }
      //  }
      //}
      Assert.IsNotNull(method, "method not found!");
      AssertRuleDoesNotApply(method);
    }

    public delegate void SimpleEventHandler(int x);

    public event SimpleEventHandler SimpleEvent;

#pragma warning disable CA1822 // Mark members as static
    public void OnSimpleEvent(int x)
#pragma warning restore CA1822 // Mark members as static
    {
    }

    [Test]
    public void EventTest()
    {
      SimpleEvent += new SimpleEventHandler(OnSimpleEvent);
      AssertRuleDoesNotApply<AvoidUnusedParametersTest>("OnSimpleEvent");
    }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CA1822 // Mark members as static
    public void EmptyMethod(int x)
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

    [Test]
    public void EmptyMethodTest()
    {
      AssertRuleFailure<AvoidUnusedParametersTest>("EmptyMethod", 1);
    }

    public static bool operator ==(AvoidUnusedParametersTest t1, AvoidUnusedParametersTest t2)
    {
      return t1.Equals(t2);
    }

    public static bool operator !=(AvoidUnusedParametersTest t1, AvoidUnusedParametersTest t2)
    {
      return !t1.Equals(t2);
    }

    public struct StructureOk
    {
      public static bool operator ==(StructureOk s1, StructureOk s2)
      {
#pragma warning disable CA2013 // Do not use ReferenceEquals with value types
        return Object.ReferenceEquals(s1, s2);
#pragma warning restore CA2013 // Do not use ReferenceEquals with value types
      }

      public static bool operator !=(StructureOk s1, StructureOk s2)
      {
#pragma warning disable CA2013 // Do not use ReferenceEquals with value types
        return !Object.ReferenceEquals(s1, s2);
#pragma warning restore CA2013 // Do not use ReferenceEquals with value types
      }
    }

    public struct StructureBad
    {
#pragma warning disable IDE0060 // Remove unused parameter
      public static bool operator ==(StructureBad s1, StructureBad s2)
#pragma warning restore IDE0060 // Remove unused parameter
      {
        return true;
      }

#pragma warning disable IDE0060 // Remove unused parameter
      public static bool operator !=(StructureBad s1, StructureBad s2)
#pragma warning restore IDE0060 // Remove unused parameter
      {
        return false;
      }
    }

    [Test]
    public void OperatorsClass()
    {
      AssertRuleSuccess<AvoidUnusedParametersTest>("op_Equality");
      AssertRuleSuccess<AvoidUnusedParametersTest>("op_Inequality");
    }

    [Test]
    public void OperatorsStructureOk()
    {
      AssertRuleSuccess<AvoidUnusedParametersTest.StructureOk>("op_Equality");
      AssertRuleSuccess<AvoidUnusedParametersTest.StructureOk>("op_Inequality");
    }

    [Test]
    public void OperatorsStructureBad()
    {
      AssertRuleFailure<AvoidUnusedParametersTest.StructureBad>("op_Equality", 2);
      AssertRuleFailure<AvoidUnusedParametersTest.StructureBad>("op_Inequality", 2);
    }

    [Test]
    public void OperatorsCecil()
    {
      AssertRuleSuccess<OpCode>("op_Equality");
      AssertRuleSuccess<OpCode>("op_Inequality");
    }

    public void ButtonClick_EvenArgsUnused(object o, EventArgs e)
    {
      if (o == null)
        throw new ArgumentNullException(nameof(o));
    }

    public void ButtonClick_NoParameterUnused(object o, EventArgs e)
    {
      Console.WriteLine("uho");
    }

    [Test]
    public void EventArgs()
    {
      AssertRuleDoesNotApply<AvoidUnusedParametersTest>("ButtonClick_EvenArgsUnused");
      AssertRuleDoesNotApply<AvoidUnusedParametersTest>("ButtonClick_NoParameterUnused");
    }

    [Test]
    public void AbstractMethodTest()
    {
      AssertRuleDoesNotApply<AbstractClass>("AbstractMethod");
    }

    [Test]
    public void VirtualMethodTest()
    {
      AssertRuleDoesNotApply<VirtualClass>("VirtualMethod");
    }

    [Test]
    public void OverrideMethodTest()
    {
      AssertRuleDoesNotApply<OverrideClass>("VirtualMethod");
    }

    // using our own extern helps more in coverage than SimpleMethods.External would
    [DllImport("libc.so")]
    private static extern double cos(double x);

    [Test]
    public void ExternMethodTest()
    {
      AssertRuleDoesNotApply<AvoidUnusedParametersTest>("cos");
    }

    [Conditional("DO_NOT_DEFINE")]
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Test code.")]
    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Test code.")]
    private void WriteLine(string s)
    {
      // the C.WL will not be compiled since DO_NOT_DEFINE is undefined
      // which means parameter 's' will be unused by the method
      Console.WriteLine(s);
    }

    [Test]
    public void ConditionalCode()
    {
      AssertRuleDoesNotApply<AvoidUnusedParametersTest>("WriteLine");
    }

    public class FxCopTest
    {
      // CA1801
      public class ReviewUnusedParameters
      {
#pragma warning disable IDE0060 // Remove unused parameter
        public static void Fail(int count)
#pragma warning restore IDE0060 // Remove unused parameter
        {
        }

        // manually suppressed - no MessageId
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")]
        public static void ManuallySuppressed(int count)
        {
        }

        // automatically suppressed using VS2010
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "count")]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")]
        public static void AutomaticallySuppressed(int count)
        {
        }

        // automatically suppressed using VS2010 (see GlobalSupressions.cs)
#pragma warning disable IDE0060 // Remove unused parameter
        public static void GloballySuppressed(int count)
#pragma warning restore IDE0060 // Remove unused parameter
        {
        }
      }
    }

    [Test]
    public void CA1801()
    {
      AssertRuleFailure<FxCopTest.ReviewUnusedParameters>("Fail", 1);
      AssertRuleDoesNotApply<FxCopTest.ReviewUnusedParameters>("ManuallySuppressed");
      AssertRuleDoesNotApply<FxCopTest.ReviewUnusedParameters>("AutomaticallySuppressed");
      AssertRuleDoesNotApply<FxCopTest.ReviewUnusedParameters>("GloballySuppressed");
    }
  }
}