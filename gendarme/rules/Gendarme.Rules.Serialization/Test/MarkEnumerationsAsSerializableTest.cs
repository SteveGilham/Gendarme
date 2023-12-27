//
// Unit tests for MarkEnumerationsAsSerializableRule
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
using Gendarme.Rules.Serialization;

using NUnit.Framework;
using Test.Rules.Definitions;
using Test.Rules.Fixtures;
using Test.Rules.Helpers;

namespace Test.Rules.Serialization
{
  [TestFixture]
  public class MarkEnumerationsAsSerializableTest : TypeRuleTestFixture<MarkEnumerationsAsSerializableRule>
  {
    [Test]
    public void DoesNotApply()
    {
      AssertRuleDoesNotApply(SimpleTypes.Class);
      AssertRuleDoesNotApply(SimpleTypes.Delegate);
      AssertRuleDoesNotApply(SimpleTypes.Interface);
      AssertRuleDoesNotApply(SimpleTypes.Structure);
    }

    private enum NonSerializableEnum
    {
      One,
      Two
    }

    [Serializable]
    private enum SerializableEnum
    {
      One,
      Two
    }

#if NET472

    [Test]
    public void Reflection()
    {
      // always report true since enums are ALWAYS serializable
      Assert.That(typeof(NonSerializableEnum).IsSerializable, "NonSerializableEnum");
      Assert.That(typeof(SerializableEnum).IsSerializable, "SerializableEnum");
    }

#endif

    //[Test]
    //public void FSharpPlaceholders()
    //{
    //  var probe = typeof(AvoidMultidimensionalIndexer.DotNet.CLIArgs);
    //  var type = probe.Assembly.GetType("System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes");
    //  var def = Helpers.DefinitionLoader.GetTypeDefinition(type);
    //  AssertRuleDoesNotApply(def);
    //}

    [Test]
    public void Cecil()
    {
      // Cecil reports what being set on the type, not how the runtime treats it
      TypeDefinition nse = DefinitionLoader.GetTypeDefinition<NonSerializableEnum>();
      Assert.That(nse.IsSerializable, Is.False, "NonSerializableEnum");
      TypeDefinition se = DefinitionLoader.GetTypeDefinition<SerializableEnum>();
      Assert.That(se.IsSerializable, Is.True, "SerializableEnum");
    }

    [Test]
    public void Good()
    {
      AssertRuleSuccess<SerializableEnum>();
    }

    [Test]
    public void Bad()
    {
      AssertRuleFailure<NonSerializableEnum>(1);
    }
  }
}