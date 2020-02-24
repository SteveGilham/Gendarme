//
// Unit tests for UseSTAThreadAttributeOnSWFEntryPointsTest
//
// Authors:
//	Daniel Abramov <ex@vingrad.ru>
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2008 Daniel Abramov
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
using System.Collections;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Gendarme.Framework;
using Gendarme.Rules.UI;

using NUnit.Framework;
using Test.Rules.Helpers;

namespace Test.Rules.Ui
{
  internal class CommonMainClass { }

  internal class NoAttributesMain : CommonMainClass
  {
    public static void MainName()
    {
    }
  }

  internal class STAThreadMain : CommonMainClass
  {
    [STAThread]
    public static void MainName()
    {
    }
  }

  internal class MTAThreadMain : CommonMainClass
  {
    [MTAThread]
    public static void MainName()
    {
    }
  }

  internal class BothSTAAndMTAThreadMain : CommonMainClass
  {
    [STAThread]
    [MTAThread]
    public static void MainName()
    {
    }
  }

  [TestFixture]
  public class UseSTAThreadAttributeOnSWFEntryPointsTest
  {
    private UseSTAThreadAttributeOnSWFEntryPointsRule rule;
    private TestRunner runner;
    private AssemblyDefinition assembly;

    [OneTimeSetUp]
    public void FixtureSetUp()
    {
      string unit = System.Reflection.Assembly.GetExecutingAssembly().Location;
      assembly = AssemblyDefinition.ReadAssembly(unit);
      rule = new UseSTAThreadAttributeOnSWFEntryPointsRule();
      runner = new TestRunner(rule);
    }

    private AssemblyDefinition GetAssemblyAndInject<TInjectedType>(bool SWF)
      where TInjectedType : CommonMainClass
    {
      // return executable assembly with predefined entry point - Main () of TInjectedType
      string fullClassName = typeof(TInjectedType).FullName;
      AssemblyDefinition ass = CreateAssembly(typeof(TInjectedType).Name + "Assembly", ModuleKind.Console);
      if (SWF)
      {
        ass.MainModule.Kind = ModuleKind.Windows;
        AssemblyNameReference winFormsRef = new AssemblyNameReference("System.Windows.Forms", new Version(2, 0, 0, 0));
        winFormsRef.PublicKeyToken = new byte[] { 0xb7, 0x7a, 0x5c, 0x56, 0x19, 0x34, 0xe0, 0x89 };
        ass.MainModule.AssemblyReferences.Add(winFormsRef);
      }
      TypeDefinition mainClass = Inject(assembly.MainModule.GetType(fullClassName), ass);
      ass.EntryPoint = GetMethod(mainClass, Gendarme.Framework.Rocks.MethodRocks.MainName);
      return ass;
    }

    private static AssemblyDefinition CreateAssembly(string name, ModuleKind kind)
    {
      return AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition(name, new Version()), name, kind);
    }

    // horrible hack, we're pretending to copy a full loaded type into another assembly
    private static TypeDefinition Inject(TypeDefinition type, AssemblyDefinition target)
    {
      var module = ModuleDefinition.ReadModule(
        type.Module.FullyQualifiedName,
        new ReaderParameters { ReadingMode = ReadingMode.Immediate });

      type = module.GetType(type.FullName);
      module.Types.Remove(type);
      target.MainModule.Types.Add(type);
      return type;
    }

    private MethodDefinition GetMethod(TypeDefinition type, string name)
    {
      foreach (MethodDefinition method in type.Methods)
      {
        if (method.Name == name)
          return method;
      }
      return null;
    }

    [Test]
    public void TestNoEntryPoint()
    {
      var save = Gendarme.Framework.Rocks.MethodRocks.MainName;
      var substitute = "MainName";
      try
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = substitute;
        Assert.AreEqual(RuleResult.DoesNotApply, runner.CheckAssembly(assembly));
      }
      finally
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = save;
      }
    }

    [Test]
    public void TestNoTANonSWFAssembly()
    {
      var save = Gendarme.Framework.Rocks.MethodRocks.MainName;
      var substitute = "MainName";
      try
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = substitute;
        Assert.AreEqual(RuleResult.DoesNotApply, runner.CheckAssembly(GetAssemblyAndInject<NoAttributesMain>(false)));
      }
      finally
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = save;
      }
    }

    [Test]
    public void TestNoTASWFAssembly()
    {
      var save = Gendarme.Framework.Rocks.MethodRocks.MainName;
      var substitute = "MainName";
      try
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = substitute;
        Assert.AreEqual(RuleResult.Failure, runner.CheckAssembly(GetAssemblyAndInject<NoAttributesMain>(true)));
      }
      finally
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = save;
      }
    }

    [Test]
    public void TestSTANonSWFAssembly()
    {
      var save = Gendarme.Framework.Rocks.MethodRocks.MainName;
      var substitute = "MainName";
      try
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = substitute;
        Assert.AreEqual(RuleResult.DoesNotApply, runner.CheckAssembly(GetAssemblyAndInject<STAThreadMain>(false)));
      }
      finally
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = save;
      }
    }

    [Test]
    public void TestMTANonSWFAssembly()
    {
      var save = Gendarme.Framework.Rocks.MethodRocks.MainName;
      var substitute = "MainName";
      try
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = substitute;
        Assert.AreEqual(RuleResult.DoesNotApply, runner.CheckAssembly(GetAssemblyAndInject<MTAThreadMain>(false)));
      }
      finally
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = save;
      }
    }

    [Test]
    public void TestSTAThreadSWFAssembly()
    {
      var save = Gendarme.Framework.Rocks.MethodRocks.MainName;
      var substitute = "MainName";
      try
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = substitute;
        Assert.AreEqual(RuleResult.Success, runner.CheckAssembly(GetAssemblyAndInject<STAThreadMain>(true)));
      }
      finally
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = save;
      }
    }

    [Test]
    public void TestMTAThreadSWFAssembly()
    {
      var save = Gendarme.Framework.Rocks.MethodRocks.MainName;
      var substitute = "MainName";
      try
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = substitute;
        Assert.AreEqual(RuleResult.Failure, runner.CheckAssembly(GetAssemblyAndInject<MTAThreadMain>(true)));
      }
      finally
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = save;
      }
    }

    [Test]
    public void TestSTAAndMTAThreadSWFAssembly()
    {
      var save = Gendarme.Framework.Rocks.MethodRocks.MainName;
      var substitute = "MainName";
      try
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = substitute;
        Assert.AreEqual(RuleResult.Failure, runner.CheckAssembly(GetAssemblyAndInject<BothSTAAndMTAThreadMain>(true)));
      }
      finally
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = save;
      }
    }
  }
}