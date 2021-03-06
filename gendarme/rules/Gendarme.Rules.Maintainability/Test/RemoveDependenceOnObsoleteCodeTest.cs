//
// Unit tests for RemoveDependenceOnObsoleteCodeRule
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
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
using Gendarme.Rules.Maintainability;

using NUnit.Framework;
using Test.Rules.Definitions;
using Test.Rules.Fixtures;

namespace Tests.Rules.Maintainability
{
  [Obsolete]
  internal class ObsoleteType
  {
    public void Show()
    {
    }
  }

  [Obsolete]
  internal struct ObsoleteStruct
  {
    public int Field;
  }

  [Obsolete]
  internal struct ObsoleteEnum
  {
  }

  [Obsolete]
  internal interface IObsolete
  {
  }

  internal class TypeInheritObsoleteType : ObsoleteType
  {
  }

  internal class TypeImplementObsoleteInterface : IObsolete
  {
  }

  internal interface InterfaceImplementObsoleteInterface : IObsolete
  {
  }

  internal enum EnumWithObsoleteValues
  {
    One,
    [Obsolete] Two,
    Three,
    [Obsolete] Four
  }

  internal class TypeWithEnumFields
  {
    private ObsoleteEnum a;
    private ObsoleteEnum b;
    private EnumWithObsoleteValues c; // ok, the enum is not obsolete

    public void SetEnum(ObsoleteEnum oe)
    {
      // parameter is not used so we detect it on the signature only
    }

    public void UseEnum()
    {
      c = EnumWithObsoleteValues.Two; // obsolete
    }
  }

  internal struct StructWithStuctFields
  {
    private ObsoleteStruct a;
    private DateTime b;

    public void SetStruct(ObsoleteStruct os)
    {
      // parameter is not used so we detect it on the signature only
    }

    public void UseStruct()
    {
      a = new ObsoleteStruct();
    }
  }

  internal class TypeWithObsoleteProperties
  {
    [Obsolete]
    private int Integer { get; set; }

    [Obsolete]
    public string String { get; set; }
  }

  internal class TypeWithObsoletePropertiesType
  {
    private ObsoleteEnum Enum { get; set; }
    private ObsoleteStruct Struct { get; set; }
    private ObsoleteType Type { get; set; }
  }

  internal struct StructWithObsoleteEvents
  {
    [Obsolete]
    private event EventHandler WayTooLate;

    [Obsolete]
    public event EventHandler<EventArgs> TooLate;
  }

  [Obsolete]
  public class ObsoleteEventArgs : EventArgs
  {
  }

  [Obsolete]
  public delegate void ObsoleteEventHandler(object sender, ObsoleteEventArgs e);

  internal struct StructWithObsoleteEventsType
  {
    private event ObsoleteEventHandler WayTooLate;

    public event EventHandler<ObsoleteEventArgs> TooLate;
  }

  [TestFixture]
  public class RemoveDependenceOnObsoleteCodeTypeTest : TypeRuleTestFixture<RemoveDependenceOnObsoleteCodeRule>
  {
    [Test]
    public void FSharpIgnoreCasesOfObsoleteUnion()
    {
      AssertRuleDoesNotApply<RemoveDependenceOnObsoleteCode.ToolType.Mono>();
    }

    [Test]
    public void NonObsolete()
    {
      AssertRuleSuccess(SimpleTypes.Class);
      AssertRuleSuccess(SimpleTypes.Delegate);
      AssertRuleSuccess(SimpleTypes.Enum);
      AssertRuleSuccess(SimpleTypes.GeneratedType);
      AssertRuleSuccess(SimpleTypes.Interface);
      AssertRuleSuccess(SimpleTypes.Structure);
    }

    [Test]
    public void Obsolete()
    {
      // having obsolete types / interfaces ... is not an issue
      AssertRuleDoesNotApply<ObsoleteType>();
      AssertRuleDoesNotApply<ObsoleteStruct>();
      AssertRuleDoesNotApply<ObsoleteEnum>();
      AssertRuleDoesNotApply<IObsolete>();
      AssertRuleDoesNotApply<ObsoleteEventHandler>();
      // but using them is not
      AssertRuleFailure<TypeInheritObsoleteType>(1);
      AssertRuleFailure<TypeImplementObsoleteInterface>(1);
      AssertRuleFailure<InterfaceImplementObsoleteInterface>(1);
    }

    [Test]
    public void ObsoleteMembers()
    {
      // having obsolete fields is not an issue
      AssertRuleSuccess<EnumWithObsoleteValues>();
      AssertRuleSuccess<TypeWithObsoleteProperties>();
      AssertRuleSuccess<StructWithObsoleteEvents>();

      // unless they are used (obsolete type as field)
      AssertRuleFailure<TypeWithEnumFields>(2);
      AssertRuleFailure<StructWithStuctFields>(1);
      AssertRuleFailure<TypeWithObsoletePropertiesType>(6); // 3 properties + 3 backing fields
      AssertRuleFailure<StructWithObsoleteEventsType>(2);
    }

    [Test]
    public void FSharpIgnoreRecordFields()
    {
      AssertRuleDoesNotApply<RemoveDependenceOnObsoleteCode.Params>();
    }
  }

  internal class TypeWithObsoleteFields
  {
    [Obsolete]
    public static int Field;

    private void Structure()
    {
      ObsoleteStruct s = new ObsoleteStruct();
      s.Field = 0;
    }
  }

  internal class TypeWithObsoleteMethods
  {
    [Obsolete]
    public TypeWithObsoleteMethods()
    {
    }

    [Obsolete]
    public void Show()
    {
      Console.WriteLine();
    }

    public void ShowData(object o)
    {
      // variable (and use) of an obsolete type
      ObsoleteType t = (o as ObsoleteType);
      Console.WriteLine(t);
    }

    public void ShowType(object o)
    {
      (o as ObsoleteType).Show();
    }

    private ObsoleteType GetObsolete()
    {
      return null;
    }

    private void SetObsoleteField(int x)
    {
      TypeWithObsoleteFields.Field = x;
    }

    private static void MainName()
    {
      new TypeWithObsoleteMethods().Show();
    }
  }

  [TestFixture]
  public class RemoveDependenceOnObsoleteCodeMethodTest : MethodRuleTestFixture<RemoveDependenceOnObsoleteCodeRule>
  {
    [Test]
    public void DoesNotApply()
    {
      // we can define [Obsolete] if we don't use them (backward compatibility)
      AssertRuleDoesNotApply<TypeWithObsoleteMethods>("Show");
      AssertRuleDoesNotApply<TypeWithObsoleteMethods>(".ctor");
    }

    [Test]
    public void ObsoleteMethods()
    {
      // call .ctor and Show
      AssertRuleFailure<TypeWithObsoleteMethods>("MainName", 2);
      // variable (type and it's use)
      AssertRuleFailure<TypeWithObsoleteMethods>("ShowData", 1);
      // call a non-obsolete method from an obsolete type
      AssertRuleFailure<TypeWithObsoleteMethods>("ShowType", 1);
      // obsolete return value (+ implied local variable for csc, but not gmcs)
      AssertRuleFailure<TypeWithObsoleteMethods>("GetObsolete");
    }

    [Test]
    public void ObsoleteFields()
    {
      // the number of defects can vary between compiler (gmcs versus csc)
      AssertRuleFailure<StructWithStuctFields>("UseStruct");
      AssertRuleFailure<TypeWithObsoleteFields>("Structure", 3); // variable + initobj + field
      AssertRuleFailure<TypeWithObsoleteMethods>("SetObsoleteField", 1);
    }

    [Test]
    [Ignore("Enums")]
    public void ObsoleteEnums()
    {
      // enums are special since they are compiled as integers (or their base type)
      // and we "lose" the real, named, value. We need to track every case back to the type
      AssertRuleFailure<TypeWithEnumFields>("UseEnum", 1);
    }

    [Test]
    public void ObsoleteParameterTypes()
    {
      AssertRuleFailure<TypeWithEnumFields>("SetEnum", 1);
      AssertRuleFailure<StructWithStuctFields>("SetStruct", 1);
    }

    [Test]
    public void ObsoleteProperty()
    {
      // property is [Obsolete] - not the getter/setter
      AssertRuleDoesNotApply<TypeWithObsoleteProperties>("get_Integer");
      AssertRuleDoesNotApply<TypeWithObsoleteProperties>("set_Integer");
    }

    [Test]
    public void ObsoleteEvents()
    {
      // event is [Obsolete] - not the getter/setter
      AssertRuleDoesNotApply<StructWithObsoleteEvents>("add_TooLate");
      AssertRuleDoesNotApply<StructWithObsoleteEvents>("remove_TooLate");
    }

    [Test]
    public void InsideObsoleteType()
    {
      // method in obsolete type calls some obsolete code
      AssertRuleDoesNotApply<ObsoleteType>("Show");
    }

    [Test]
    public void FSharpIgnoreCaseConstructorsOfObsoleteUnion()
    {
      AssertRuleDoesNotApply<RemoveDependenceOnObsoleteCode.ToolType.Mono>(".ctor");
    }

    [Test]
    public void FSharpIgnoreRecordConstructors()
    {
      AssertRuleDoesNotApply<RemoveDependenceOnObsoleteCode.Params>(".ctor");
    }
  }
}