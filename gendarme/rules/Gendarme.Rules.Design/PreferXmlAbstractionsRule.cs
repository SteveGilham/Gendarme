//
// Gendarme.Rules.Design.PreferXmlAbstractionsRule
//
// Authors:
//	Cedric Vivier  <cedricv@neonux.com>
//
// Copyright (C) 2009 Cedric Vivier
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

using Gendarme.Framework;
using Gendarme.Framework.Rocks;

namespace Gendarme.Rules.Design {

	/// <summary>
	/// This rule checks if visible methods/properties are using XmlDocument, XPathDocument or XmlNode in their signature.
	/// This reduces extensibility (e.g virtual XML data sources) as it ties your API with a specific in-memory implementation.
	/// Prefer using abstractions such as IXPathNavigable, XmlReader, XmlWriter, or subtypes of XmlNode.
	/// </summary>
	/// <example>
	/// Bad example (property):
	/// <code>
	/// public class Application {
	///	public XmlDocument UserData {
	///		get { return userData; }
	///	}
	/// }
	/// </code>
	/// </example>
	/// <example>
	/// Good example (property):
	/// <code>
	/// public class Application {
	///	public IXPathNavigable UserData {
	///		get { return userData; }
	///	}
	/// }
	/// </code>
	/// </example>
	/// <example>
	/// Bad example (method parameter):
	/// <code>
	/// public class Application {
	///	public bool IsValidUserData (XmlDocument userData) {
	///		/* implementation */
	///	}
	/// }
	/// </code>
	/// </example>
	/// <example>
	/// Good example (method parameter):
	/// <code>
	/// public class Application {
	///	public bool IsValidUserData (XmlReader userData) {
	///		/* implementation */
	///	}
	/// }
	/// </code>
	/// </example>

	[Problem ("This visible method uses XmlDocument, XPathDocument or XmlNode in its signature. This reduces extensibility as it ties your API with a specific implementation.")]
	[Solution ("Use an abstraction such as IXPathNavigable, XmlReader, XmlWriter, or a subtype of XmlNode instead.")]
	[FxCopCompatibility ("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes")]
	public class PreferXmlAbstractionsRule : Rule, IMethodRule {

		const string XmlDocumentClass = "System.Xml.XmlDocument";
		const string XPathDocumentClass = "System.Xml.XPath.XPathDocument";
		const string XmlNodeClass = "System.Xml.XmlNode";

		public override void Initialize (IRunner runner)
		{
			base.Initialize (runner);

			Runner.AnalyzeModule += delegate (object o, RunnerEventArgs e) {
				foreach (AssemblyNameReference name in e.CurrentModule.AssemblyReferences) {
					if (name.Name == "System.Xml") {
						Active = true;
						return;
					}
				}
				Active = false; //no System.Xml assembly reference has been found
			};
		}

		public RuleResult CheckMethod (MethodDefinition method)
		{
			if (!method.IsVisible ())
				return RuleResult.DoesNotApply;

			if (IsSpecificXmlType (method.ReturnType.ReturnType))
				Runner.Report (method.ReturnType, GetSeverity (method), Confidence.High);

			if (method.HasParameters) {
				foreach (ParameterDefinition parameter in method.Parameters) {
					if (parameter.IsOut)
						continue; //out params already have their rule

					if (IsSpecificXmlType (parameter.ParameterType))
						Runner.Report (parameter, GetSeverity (method), Confidence.High);
				}
			}

			return Runner.CurrentRuleResult;
		}

		static bool IsSpecificXmlType (TypeReference type)
		{
			return type.FullName == XmlDocumentClass || type.FullName == XPathDocumentClass || type.FullName == XmlNodeClass;
		}

		static Severity GetSeverity (MethodDefinition method)
		{
			return method.IsPublic ? Severity.Medium : Severity.Low;
		}
	}
}
