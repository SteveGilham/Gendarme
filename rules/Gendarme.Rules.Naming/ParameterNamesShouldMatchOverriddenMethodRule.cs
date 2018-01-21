//
// Gendarme.Rules.Naming.ParameterNamesShouldMatchOverriddenMethodRule
//
// Authors:
//	Andreas Noever <andreas.noever@gmail.com>
//	Sebastien Pouliot  <sebastien@ximian.com>
//
//  (C) 2008 Andreas Noever
// Copyright (C) 2011 Novell, Inc (http://www.novell.com)
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
using System.Globalization;

using Mono.Cecil;
using Gendarme.Framework;
using Gendarme.Framework.Rocks;
using System.Text;

namespace Gendarme.Rules.Naming {

	/// <summary>
	/// This rule warns if an overriden method's parameter names does not match those of the 
	/// base class or those of the implemented interface. This can be confusing because it may
	/// not always be clear that it is an override or implementation of an interface method. It
	/// also makes it more difficult to use the method with languages that support named
	/// parameters (like C# 4.0).
	/// </summary>
	/// <example>
	/// Bad example:
	/// <code>
	/// public class Base {
	///	public abstract void Write (string text);
	/// }
	/// 
	/// public class SubType : Base {
	///	public override void Write (string output)
	///	{
	///		//...
	///	}
	/// }
	/// </code>
	/// </example>
	/// <example>
	/// Good example:
	/// <code>
	/// public class Base {
	///	public abstract void Write (string text);
	/// }
	/// 
	/// class SubType : Base {
	///	public override void Write (string text)
	///	{
	///		//...
	///	}
	/// }
	/// </code>
	/// </example>

	[Problem ("This method overrides (or implements) an existing method but does not use the same parameter names as the original.")]
	[Solution ("Keep parameter names consistent when overriding a class or implementing an interface.")]
	[FxCopCompatibility ("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration")]
	public class ParameterNamesShouldMatchOverriddenMethodRule : Rule, IMethodRule {

		private class ParamNamesError
		{
			private readonly int candidatesCount;
			private readonly MethodDefinition method;
			private readonly SortedDictionary<int, string> error;

			public ParamNamesError (MethodDefinition method, int candidatesCount)
			{
				this.method = method;
				this.candidatesCount = candidatesCount;
				this.error = new SortedDictionary<int, string>();
			}

			internal void Add (int index, string currentName, string parentName)
			{
				string value;
				if (error.ContainsKey (index))
				{
					value = error [index];
					value = value.Insert (value.Length - 2, " nor " + parentName);
					error [index] = value;
				}
				else
				{
					value = string.Format (CultureInfo.InvariantCulture,
						"The name of parameter #{0} ({1}) does not match the name of the parameter in the overridden method ({2}).",
						index, currentName, parentName);
					error.Add (index, value);
				}
			}

			internal void Report (IRunner runner)
			{
				if (this.error.Count > 0) {
					StringBuilder sb = new StringBuilder ();
					foreach (KeyValuePair<int, string> paramError in this.error) {
						sb.AppendLine (paramError.Value);
					}
					string message = sb.ToString ().Trim ();
					if (candidatesCount > 1)
						runner.Report (this.method, Severity.Medium, Confidence.Normal, message);
					else
						runner.Report (this.method, Severity.Medium, Confidence.High, message);
				}
			}
		}

		List<MethodDefinition> baseMethodCandidates;

		public override void Initialize (IRunner runner)
		{
			base.Initialize (runner);
			baseMethodCandidates = new List<MethodDefinition>();

			//check if this is a Boo assembly using macros
			Runner.AnalyzeModule += delegate (object o, RunnerEventArgs e) {
				IsBooAssemblyUsingMacro = (e.CurrentModule.AnyTypeReference ((TypeReference tr) => {
					return tr.IsNamed ("Boo.Lang.Compiler.Ast", "MacroStatement");
				}));
			};
		}

		private static bool SignatureMatches (MethodReference method, MethodReference baseMethod)
		{
			string name = method.Name;
			string base_name = baseMethod.Name;

			int pos = name.IndexOf ('.');
			if (pos > 0) {
				TypeReference btype = baseMethod.DeclaringType;
				string bnspace = btype.Namespace;
				if (!name.StartsWith (bnspace, StringComparison.Ordinal))
					return false;
				if (name [bnspace.Length] != '.')
					return false;

				string bname = btype.Name;
				pos = bname.IndexOf ('`');
				int length;
				if (pos > 1)
					length = pos;
				else
					length = bname.Length;
				if (String.CompareOrdinal (bname, 0, name, bnspace.Length + 1, length) != 0)
					return false;

				int end = bnspace.Length + 1 + length;
				if (pos > 0) {
					if ((name [end] != '<') && (name [end] != '`'))
						return false;
				}
				else if (name [end] != '.')
						return false;

				if (!name.EndsWith (base_name, StringComparison.Ordinal))
					return false;
			}
			else if (name != base_name)
				return false;

			return method.CompareSignature (baseMethod);
		}

		private void GetBaseMethodCandidates (MethodDefinition method)
		{
			TypeDefinition baseType = method.DeclaringType.Resolve ();
			if (baseType == null)
				return;

			while ((baseType.BaseType != null) && (baseType != baseType.BaseType)) {
				baseType = baseType.BaseType.Resolve ();
				if (baseType == null)
					return;   // could not resolve

				if (baseType.HasMethods)
					SelectMethodCandidates (method, baseType.Methods);
			}
		}

		private void GetInterfaceMethodCandidates (MethodDefinition method)
		{
			TypeDefinition type = (method.DeclaringType as TypeDefinition);
			if (!type.HasInterfaces)
				return;

			foreach (TypeReference interfaceReference in type.Interfaces) {
				TypeDefinition interfaceCandidate = interfaceReference.Resolve ();
				if ((interfaceCandidate != null) && interfaceCandidate.HasMethods)
					SelectMethodCandidates (method, interfaceCandidate.Methods);
			}
		}

		private void GetInterfaceMethodCandidates (MethodDefinition method, string interfaceName)
		{
			TypeDefinition type = method.DeclaringType;
			if (!type.HasInterfaces)
				return;

			foreach (TypeReference interfaceReference in type.Interfaces) {
				TypeDefinition interfaceCandidate = interfaceReference.Resolve ();
				if ((interfaceCandidate != null) && interfaceCandidate.HasMethods) {
					string fullName = interfaceCandidate.FullName;
					int pos = fullName.IndexOf ('`');
					if (pos > 0) {
						fullName = fullName.Remove (pos + 1);
					}
					if (string.Equals (interfaceName, fullName, StringComparison.Ordinal))
						SelectMethodCandidates (method, interfaceCandidate.Methods);
				}
			}
		}

		private void SelectMethodCandidates (MethodDefinition method, IEnumerable<MethodDefinition> candidates)
		{
			foreach (MethodDefinition candidate in candidates) {
				if (SignatureMatches (method, candidate))
					baseMethodCandidates.Add (candidate);
			}
		}


		public RuleResult CheckMethod (MethodDefinition method)
		{
			if (!method.IsVirtual || !method.HasParameters || method.IsGeneratedMethodOrType ())
				return RuleResult.DoesNotApply;

			baseMethodCandidates.Clear ();
			if (!method.Name.Contains ("."))
			{
				if (!method.IsNewSlot)
					GetBaseMethodCandidates (method);
				GetInterfaceMethodCandidates (method);
			} else
				GetInterfaceMethodCandidates (method, GetInterfaceName (method.Name));
			if (baseMethodCandidates.Count == 0)
				return RuleResult.DoesNotApply;

			bool found = false;
			int candidatesCount = baseMethodCandidates.Count;
			ParamNamesError report = new ParamNamesError (method, candidatesCount);
			for (int j = 0; ((j < candidatesCount) && !found); j++) {
				MethodDefinition baseMethod = baseMethodCandidates[j];
				IList<ParameterDefinition> base_pdc = baseMethod.Parameters;
				//do not trigger false positives on Boo macros
				if (IsBooAssemblyUsingMacro && IsBooMacroParameter (base_pdc [0])) {
					found = true;
				} else {
					bool noDifference = true;
					IList<ParameterDefinition> pdc = method.Parameters;
					for (int i = 0; (noDifference && (i < pdc.Count)); i++) {
						if (pdc [i].Name != base_pdc [i].Name) {
							noDifference = false;
							report.Add (index: (i + 1), currentName: pdc[i].Name, parentName: base_pdc[i].Name); // add separately for each parameter
						}
					}
					found |= noDifference;
				}
			}
			if (!found)
					report.Report (Runner);
			return Runner.CurrentRuleResult;
		}

		private static string GetInterfaceName (string functionName)
		{
			int pos = functionName.LastIndexOf ('.');
			string interfaceName = functionName.Remove (pos);
			pos = functionName.IndexOf ('<');
			if (pos > 0)
				interfaceName = (interfaceName.Remove (pos) + "`");
			return (interfaceName);
		}

		public bool SkipGeneratedGuiMethods
		{
			get
			{
				return true;
			}
		}

		private bool IsBooAssemblyUsingMacro { get; set; }

		private static bool IsBooMacroParameter (ParameterReference p)
		{
			return p.Name == "macro" && p.ParameterType.IsNamed ("Boo.Lang.Compiler.Ast", "MacroStatement");
		}
	}
}
