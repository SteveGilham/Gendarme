//
// Unit tests for GetEntryAssemblyMayReturnNullRule
//
// Authors:
//	Daniel Abramov <ex@vingrad.ru>
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) Daniel Abramov
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
using System.Linq;

using Mono.Cecil;

using Gendarme.Framework;
using Gendarme.Framework.Rocks;
using Gendarme.Rules.BadPractice;

using NUnit.Framework;

using Test.Rules.Definitions;
using Test.Rules.Fixtures;
using Test.Rules.Helpers;

namespace Test.Rules.BadPractice
{
  [TestFixture]
  public class GetEntryAssemblyMayReturnNullTest : MethodRuleTestFixture<GetEntryAssemblyMayReturnNullRule>
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
    public void TestMethodNotCallingGetEntryAssembly()
    {
      AssertRuleSuccess<Examples.Rules.BadPractice.ClassCallingGetEntryAssembly>("NoCalls");
    }

    private TypeDefinition GetTest<T>(AssemblyDefinition assembly)
    {
      return assembly.MainModule.GetType(typeof(T).FullName);
    }

    [Test]
    public void TestGetEntryAssemblyCallFromExecutable()
    {
      string unit = typeof(Examples.Rules.BadPractice.ClassCallingGetEntryAssembly).Assembly.Location;
      AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(unit);
      try
      {
        assembly.EntryPoint = GetTest<Examples.Rules.BadPractice.ClassCallingGetEntryAssembly>(assembly).Methods.FirstOrDefault(m => m.Name == "MainName");
        assembly.MainModule.Kind = ModuleKind.Console;
        MethodDefinition method = GetTest<Examples.Rules.BadPractice.ClassCallingGetEntryAssembly>(assembly).Methods.FirstOrDefault(m => m.Name == "ThreeCalls");
        Assert.AreEqual(RuleResult.DoesNotApply, (Runner as TestRunner).CheckMethod(method), "RuleResult");
        Assert.AreEqual(0, Runner.Defects.Count, "Count");
      }
      finally
      {
        assembly.EntryPoint = null;
        assembly.MainModule.Kind = ModuleKind.Dll;
      }
    }

    [Test]
    public void TestMethodCallingGetEntryAssemblyOnce()
    {
      AssertRuleFailure<Examples.Rules.BadPractice.ClassCallingGetEntryAssembly>("OneCall", 1);
    }

    [Test]
    public void TestMethodCallingGetEntryAssemblyThreeTimes()
    {
      AssertRuleFailure<Examples.Rules.BadPractice.ClassCallingGetEntryAssembly>("ThreeCalls", 3);
    }
  }
}