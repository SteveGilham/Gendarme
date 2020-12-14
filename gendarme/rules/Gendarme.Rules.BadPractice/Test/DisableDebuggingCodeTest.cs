//
// Unit tests for DisableDebuggingCodeRule
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
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
using System.Diagnostics;

using Mono.Cecil;

using Gendarme.Rules.BadPractice;

using NUnit.Framework;

using Test.Rules.Definitions;
using Test.Rules.Fixtures;
using Test.Rules.Helpers;

namespace Test.Rules.BadPractice
{
  [TestFixture]
  public class DisableDebuggingCodeTest : MethodRuleTestFixture<DisableDebuggingCodeRule>
  {
    [Test]
    public void DoesNotApply()
    {
      // no IL
      AssertRuleDoesNotApply(SimpleMethods.ExternalMethod);
      // no NEWOBJ
      AssertRuleDoesNotApply(SimpleMethods.EmptyMethod);
    }

    [Test]
    public void CommonCheck()
    {
      AssertRuleSuccess<Examples.Rules.BadPractice.DisableDebuggingCode>("ConditionalTrace");
      AssertRuleFailure<Examples.Rules.BadPractice.DisableDebuggingCode>("ConditionalOther", 1);
    }

    [Test]
    [Conditional("DEBUG")]
    public void DebugCheck()
    {
      AssertRuleSuccess<Examples.Rules.BadPractice.DisableDebuggingCode>("ConditionalDebug");
      AssertRuleSuccess<Examples.Rules.BadPractice.DisableDebuggingCode>("ConditionalMultiple");
    }

    [Test]
    public void NonDebug()
    {
#if DEBUG
      AssertRuleSuccess<Examples.Rules.BadPractice.DisableDebuggingCode>("UsingDebug");
#else
			AssertRuleDoesNotApply<Examples.Rules.BadPractice.DisableDebuggingCode> ("UsingDebug");	// method has no body in release
#endif
      AssertRuleSuccess<Examples.Rules.BadPractice.DisableDebuggingCode>("UsingTrace");
      AssertRuleFailure<Examples.Rules.BadPractice.DisableDebuggingCode>("UsingConsole", 1);
    }

    [Test]
    public void Initialize()
    {
      string unit = typeof(Examples.Rules.BadPractice.DisableDebuggingCode).Assembly.Location;
      AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(unit);

      Rule.Active = false;
      (Runner as TestRunner).OnAssembly(assembly);
      Assert.IsFalse(Rule.Active, "Default-Active-False");

      Rule.Active = true;
      (Runner as TestRunner).OnAssembly(assembly);
      Assert.IsTrue(Rule.Active, "Assembly-Active-True");

      (Runner as TestRunner).OnModule(assembly.MainModule);
      Assert.IsTrue(Rule.Active, "Module-Active-True");
    }
  }
}