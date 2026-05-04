//
// Unit tests for MethodRocks
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//	Andreas Noever <andreas.noever@gmail.com>
//
// Copyright (C) 2007-2008 Novell, Inc (http://www.novell.com)
// (C) 2008 Andreas Noever
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
using Gendarme.Framework.Rocks;

using Mono.Cecil;
using NUnit.Framework;

namespace Test.Framework.Rocks
{
  [TestFixture]
  public class MethodRocksTest
  {
    private class MainClassVoidVoid
    {
      private static void MainName()
      {
      }
    }

    private class MainClassIntVoid
    {
      private static int MainName()
      {
        return 42;
      }
    }

    private class MainClassVoidStrings
    {
      private static void MainName(string[] args)
      {
      }
    }

    private class MainClassIntStrings
    {
      private static int MainName(string[] args)
      {
        return 42;
      }

      ~MainClassIntStrings()
      {
      }
    }

    public int Value
    {
      get { return 42; }
      set { throw new NotSupportedException(); }
    }

    protected void EventCallback(object sender, EventArgs ea)
    {
    }

    public class FooEventArgs : EventArgs
    { }

    protected void FooEventCallback(object sender, FooEventArgs fea)
    {
    }

    private AssemblyDefinition assembly;

    [OneTimeSetUp]
    public void FixtureSetUp()
    {
      string unit = Assembly.GetExecutingAssembly().Location;
      assembly = AssemblyDefinition.ReadAssembly(unit);
    }

    private static TypeName TN(string ns, string name)
    {
      return new TypeName
      {
        Namespace = ns,
        Name = name
      };
    }

    private MethodDefinition GetMethod(string typeName, string methodName)
    {
      TypeDefinition type = assembly.MainModule.GetType(typeName);
      foreach (MethodDefinition method in type.Methods)
      {
        if (method.Name == methodName)
          return method;
      }
      Assert.Fail("Method {methodName} was not found.");
      return null;
    }

    private MethodDefinition GetMethod(string name)
    {
      return GetMethod("Test.Framework.Rocks.MethodRocksTest", name);
    }

    [Test]
    public void HasAttribute_Namespace_Null()
    {
      MethodDefinition method = GetMethod("FixtureSetUp");
      Assert.Throws<ArgumentNullException>(new Action(() =>
          method.HasAttribute(TN(null, "a"))));
    }

    [Test]
    public void HasAttribute_Name_Null()
    {
      MethodDefinition method = GetMethod("FixtureSetUp");
      Assert.Throws<ArgumentNullException>(new Action(() =>
          method.HasAttribute(TN("a", null))));
    }

    [Test]
    public void HasAttribute()
    {
      MethodDefinition method = GetMethod("FixtureSetUp");
      Assert.That(method.HasAttribute(TN("NUnit.Framework", "OneTimeSetUpAttribute")), "NUnit.Framework.OneTimeSetUpAttribute");
      Assert.That(!method.HasAttribute(TN("NUnit.Framework", "OneTimeSetUp")), "NUnit.Framework.OneTimeSetUp");
    }

    [Test]
    public void IsEntryPoint()
    {
      Assert.That(!GetMethod("FixtureSetUp").IsEntryPoint(), "FixtureSetUp");
    }

    [Test]
    public void IsFinalizer()
    {
      Assert.That(!GetMethod("FixtureSetUp").IsFinalizer(), "FixtureSetUp");
      Assert.That(GetMethod("Test.Framework.Rocks.MethodRocksTest/MainClassIntStrings", "Finalize").IsFinalizer(), "~MainClassIntStrings");
    }

    [Test]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public void IsGeneratedCode_CompilerGenerated()
    {
      Assert.That(GetMethod("IsGeneratedCode_CompilerGenerated").IsGeneratedCode(), "IsCompilerGenerated");
      Assert.That(!GetMethod("FixtureSetUp").IsGeneratedCode(), "FixtureSetUp");
    }

    [Test]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("unit test", "1.0")]
    public void IsGeneratedCode_GeneratedCode()
    {
      Assert.That(GetMethod("IsGeneratedCode_GeneratedCode").IsGeneratedCode(), "IsCompilerGenerated");
      Assert.That(!GetMethod("FixtureSetUp").IsGeneratedCode(), "FixtureSetUp");
    }

    [Test]
    public void IsMain()
    {
      var save = Gendarme.Framework.Rocks.MethodRocks.MainName;
      var substitute = "MainName";
      try
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = substitute;
        Assert.That(GetMethod("Test.Framework.Rocks.MethodRocksTest/MainClassVoidVoid", substitute).IsMain(), "MainClassVoidVoid");
        Assert.That(GetMethod("Test.Framework.Rocks.MethodRocksTest/MainClassIntVoid", substitute).IsMain(), "MainClassIntVoid");
        Assert.That(GetMethod("Test.Framework.Rocks.MethodRocksTest/MainClassVoidStrings", substitute).IsMain(), "MainClassVoidStrings");
        Assert.That(GetMethod("Test.Framework.Rocks.MethodRocksTest/MainClassIntStrings", substitute).IsMain(), "MainClassIntStrings");
        Assert.That(!GetMethod("FixtureSetUp").IsMain(), "FixtureSetUp");
      }
      finally
      {
        Gendarme.Framework.Rocks.MethodRocks.MainName = save;
      }
    }

    [Test]
    public void IsProperty()
    {
      Assert.That(GetMethod("get_Value").IsProperty(), "get_Value");
      Assert.That(GetMethod("set_Value").IsProperty(), "set_Value");
      Assert.That(!GetMethod("FixtureSetUp").IsProperty(), "FixtureSetUp");
    }

    [Test]
    public void IsVisible()
    {
      TypeDefinition type = assembly.MainModule.GetType("Test.Framework.Rocks.PublicType");
      Assert.That(type.GetMethod("PublicMethod").IsVisible(), "PublicType.PublicMethod");
      Assert.That(type.GetMethod("ProtectedMethod").IsVisible(), "PublicType.ProtectedMethod");
      Assert.That(!type.GetMethod("InternalMethod").IsVisible(), "PublicType.InternalMethod");
      Assert.That(!type.GetMethod("PrivateMethod").IsVisible(), "PublicType.PrivateMethod");

      type = assembly.MainModule.GetType("Test.Framework.Rocks.PublicType/NestedPublicType");
      Assert.That(type.GetMethod("PublicMethod").IsVisible(), "NestedPublicType.PublicMethod");
      Assert.That(type.GetMethod("ProtectedMethod").IsVisible(), "NestedPublicType.ProtectedMethod");
      Assert.That(!type.GetMethod("PrivateMethod").IsVisible(), "NestedPublicType.PrivateMethod");

      type = assembly.MainModule.GetType("Test.Framework.Rocks.PublicType/NestedProtectedType");
      Assert.That(type.GetMethod("PublicMethod").IsVisible(), "NestedProtectedType.PublicMethod");

      type = assembly.MainModule.GetType("Test.Framework.Rocks.PublicType/NestedPrivateType");
      Assert.That(!type.GetMethod("PublicMethod").IsVisible(), "NestedPrivateType.PublicMethod");

      type = assembly.MainModule.GetType("Test.Framework.Rocks.InternalType");
      Assert.That(!type.GetMethod("PublicMethod").IsVisible(), "InternalType.PublicMethod");
    }

    [Test]
    public void IsEventCallback()
    {
      Assert.That(GetMethod("EventCallback").IsEventCallback(), "EventCallback");
      Assert.That(GetMethod("FooEventCallback").IsEventCallback(), "FooEventCallback");
      Assert.That(!GetMethod("IsEventCallback").IsEventCallback(), "IsEventCallback");
    }

    [Test]
    public void GetPropertyByAccessor()
    {
      Assert.That(GetMethod("get_Value").GetPropertyByAccessor().Name, Is.EqualTo("Value"), "get_Value");
      Assert.That(GetMethod("set_Value").GetPropertyByAccessor().Name, Is.EqualTo("Value"), "set_Value");
      Assert.That(GetMethod("EventCallback").GetPropertyByAccessor(), Is.Null, "EventCallback");
    }
  }
}