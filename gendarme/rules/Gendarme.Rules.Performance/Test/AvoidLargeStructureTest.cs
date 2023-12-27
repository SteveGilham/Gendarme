//
// Unit tests for AvoidLargeStructureRule
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

using Mono.Cecil;
using Mono.Cecil.Metadata;
using Gendarme.Framework;
using Gendarme.Rules.Performance;

using NUnit.Framework;
using Test.Rules.Fixtures;
using Test.Rules.Helpers;

namespace Test.Rules.Performance
{
#pragma warning disable 169, 649

  [TestFixture]
  public class AvoidLargeStructureTest : TypeRuleTestFixture<AvoidLargeStructureRule>
  {
    private int GetSize(Type type)
    {
      return (int)AvoidLargeStructureRule.SizeOfStruct(DefinitionLoader.GetTypeDefinition(type));
    }

    [Test]
    public void Class()
    {
      AssertRuleDoesNotApply<AvoidLargeStructureTest>();
    }

    // Also note that this rule deals with the managed size of a structure,
    // not it's unmanaged representation (i.e. [StructLayout] attribute is ignored).

    // note: we need unsafe to call sizeof on structs

    private struct Empty
    {
    }

    protected void AssertAreEqual(object a, object b, string c)
    {
      Assert.That(a, Is.EqualTo(b), c);
    }

    [Test]
    public unsafe void StructEmpty()
    {
      AssertRuleSuccess<Empty>();
      // note: under MS runtime sizeof(Empty) returns 1
      // while is returns 0 under Mono
      // Assert.AreEqual (sizeof (Empty), GetSize (type), "Size");
    }

    private struct Small
    {
      private byte b;
    }

    [Test]
    public unsafe void StructSmall()
    {
      AssertRuleSuccess<Small>();
      AssertAreEqual(sizeof(Small), GetSize(typeof(Small)), "Size");
    }

    private struct DualBytes
    {
      private byte b1;
      private byte b2;
    }

    [Test]
    public unsafe void StructDualBytes()
    {
      AssertRuleSuccess<DualBytes>();
      AssertAreEqual(sizeof(DualBytes), GetSize(typeof(DualBytes)), "Size");
    }

    private struct DualSmall
    {
      private Small s1;
      private Small s2;
    }

    [Test]
    public unsafe void StructDualSmall()
    {
      AssertRuleSuccess<DualSmall>();
      AssertAreEqual(sizeof(DualSmall), GetSize(typeof(DualSmall)), "Size");
    }

    private struct Misaligned
    {
      private byte b;
      private char c;
      private int i;
    }

    [Test]
    public unsafe void StructMisaligned()
    {
      AssertRuleSuccess<Misaligned>();
      AssertAreEqual(sizeof(Misaligned), GetSize(typeof(Misaligned)), "Size");
    }

    // 20 bytes - due to misalignment of bytes and chars
    private struct MisalignedLarge
    {
      private byte b1;
      private char c1;
      private byte b2;
      private char c2;
      private byte b3;
      private char c3;
      private byte b4;
      private char c4;
      private int length;
    }

    [Test]
    public unsafe void StructMisalignedLarge()
    {
      AssertRuleFailure<MisalignedLarge>();
      AssertAreEqual(sizeof(MisalignedLarge), GetSize(typeof(MisalignedLarge)), "Size");
    }

    // 16 bytes - due to "correct" alignment of bytes and chars
    private struct AlignedLarge
    {
      private byte b1;
      private byte b2;
      private byte b3;
      private byte b4;
      private char c1;
      private char c2;
      private char c3;
      private char c4;
      private int length;
    }

    [Test]
    public unsafe void StructAlignedLarge()
    {
      AssertRuleSuccess<AlignedLarge>();
      AssertAreEqual(sizeof(AlignedLarge), GetSize(typeof(AlignedLarge)), "Size");
    }

    private struct Half
    {
      private double d;
    }

    [Test]
    public unsafe void StructHalf()
    {
      AssertRuleSuccess<Half>();
      AssertAreEqual(sizeof(Half), GetSize(typeof(Half)), "Size");
    }

    private struct FiveBytes
    {
      private float d;
      private byte b;
    }

    [Test]
    public unsafe void StructFiveBytes()
    {
      AssertRuleSuccess<FiveBytes>();
      AssertAreEqual(sizeof(FiveBytes), GetSize(typeof(FiveBytes)), "Size");
    }

    private struct NineBytes
    {
      private double d;
      private byte b;
    }

    [Test]
    public unsafe void StructNineBytes()
    {
      AssertRuleSuccess<NineBytes>();
      AssertAreEqual(sizeof(NineBytes), GetSize(typeof(NineBytes)), "Size");
    }

    private struct Limit
    {
      private int a;
      private float d;
      private long l;
    }

    [Test]
    public unsafe void StructLimit()
    {
      AssertRuleSuccess<Limit>();
      AssertAreEqual(sizeof(Limit), GetSize(typeof(Limit)), "Size");
    }

    private struct ComposedUnderLimit
    {
      private Half empty;
      private Small small;
    }

    [Test]
    public unsafe void StructComposedUnderLimit()
    {
      AssertRuleSuccess<ComposedUnderLimit>();
      AssertAreEqual(sizeof(ComposedUnderLimit), GetSize(typeof(ComposedUnderLimit)), "Size");
    }

    private struct ComposedLimit
    {
      private Half empty;
      private Half full;
    }

    [Test]
    public unsafe void StructComposedLimit()
    {
      AssertRuleSuccess<ComposedLimit>();
      AssertAreEqual(sizeof(ComposedLimit), GetSize(typeof(ComposedLimit)), "Size");
    }

    private struct ComposedOverLimit
    {
      private Limit limit;
      private Small small;
    }

    [Test]
    public unsafe void StructComposedOverLimit()
    {
      AssertRuleFailure<ComposedOverLimit>();
      AssertAreEqual(sizeof(ComposedOverLimit), GetSize(typeof(ComposedOverLimit)), "Size");
    }

    // other types defined in TypeCode enum
    private struct LessCommonTypes
    {
      private Char c;
      private DateTime date;
      private Decimal value;
      //DBNull dbnull;
      //object o;
    }

    // note: is we add a DBNull or object inside the struct then CSC will report a CS0208
    // when we call sizeof, even with unsafe, on the type.

    [Test]
    public unsafe void StructLessCommonTypes()
    {
      AssertRuleFailure<ComposedOverLimit>();
      // DateTime is 8 bytes under MS while it has 16 bytes on Mono
      //Assert.AreEqual (8, sizeof (DateTime), "DateTime");
      //Assert.AreEqual (16, sizeof (Decimal), "Decimal");
      //Assert.AreEqual (sizeof (LessCommonTypes), GetSize (typeof (LessCommonTypes)), "Size");
    }

    /* error CS0523: Struct member 'Inner.x' of type 'Inner' causes a cycle in the struct layout
		struct Inner {
			Inner x;
		}
		*/

    /* error CS05223: two times
		struct Outer {
			Inner x;
		}

		struct Inner {
			Outer x;
		}
		*/

    // from Mono.Cecil

    private struct Elem
    {
      public bool Simple;
      public bool String;
      public bool Type;
      public bool BoxedValueType;

      public Type FieldOrPropType;
      public object Value;

      public TypeReference ElemType;
    }

    private struct FixedArg
    {
      private bool SzArray;
      private uint NumElem;
      private Elem[] Elems;
    }

    [Test]
    public unsafe void Array()
    {
      AssertRuleSuccess<FixedArg>();
      // note: sizeof (FixedArg) does not work (well compile) because of the array
      //Assert.AreEqual (sizeof (FixedArg), GetSize (typeof (FixedArg)), "Size");
      AssertAreEqual(12, GetSize(typeof(FixedArg)), "Size");
    }

    private struct BunchOfEnums
    {
      private ConsoleColor cc;
      private ConsoleKey ck;
      private ConsoleModifiers cm;
      private ConsoleSpecialKey csk;
      private DateTimeKind kind;
      private DayOfWeek dow;
    }

    [Test]
    public unsafe void Enums()
    {
      AssertRuleFailure<BunchOfEnums>(1);
      AssertAreEqual(24, GetSize(typeof(BunchOfEnums)), "Size");
    }

    private struct WithStaticField
    {
      private static bool init;
      private BunchOfEnums boe1;
      private BunchOfEnums boe2;
    }

    [Test]
    public unsafe void Static()
    {
      AssertRuleFailure<WithStaticField>(1);
      // static does not cound
      AssertAreEqual(48, GetSize(typeof(WithStaticField)), "Size");
      AssertAreEqual(Severity.Medium, Runner.Defects[0].Severity, "Severity");
    }

    private struct High
    {
      private WithStaticField wsf1;
      private WithStaticField wsf2;
    }

    [Test]
    public unsafe void HighSeverity()
    {
      AssertRuleFailure<High>(1);
      AssertAreEqual(96, GetSize(typeof(High)), "Size");
      AssertAreEqual(Severity.High, Runner.Defects[0].Severity, "Severity");
    }

    private struct Critical
    {
      private High wsf1;
      private High wsf2;
      private High wsf3;
      private High wsf4;
    }

    [Test]
    public unsafe void CriticalSeverity()
    {
      AssertRuleFailure<Critical>(1);
      AssertAreEqual(384, GetSize(typeof(Critical)), "Size");
      AssertAreEqual(Severity.Critical, Runner.Defects[0].Severity, "Severity");
    }
  }
}