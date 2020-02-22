//
// Gendarme.Framework.Rocks.MethodRocks
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//	Adrian Tsai <adrian_tsai@hotmail.com>
//	Daniel Abramov <ex@vingrad.ru>
//	Andreas Noever <andreas.noever@gmail.com>
//	Cedric Vivier  <cedricv@neonux.com>
//
// Copyright (C) 2007-2008 Novell, Inc (http://www.novell.com)
// Copyright (c) 2007 Adrian Tsai
// Copyright (C) 2008 Daniel Abramov
// (C) 2008 Andreas Noever
// Copyright (C) 2008 Cedric Vivier
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
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;

namespace Gendarme.Framework.Rocks {

	// add Method[Reference|Definition][Collection] extensions methods here
	// only if:
	// * you supply minimal documentation for them (xml)
	// * you supply unit tests for them
	// * they are required somewhere to simplify, even indirectly, the rules
	//   (i.e. don't bloat the framework in case of x, y or z in the future)

	/// <summary>
	/// MethodRocks contains extensions methods for Method[Definition|Reference]
	/// and the related collection classes.
	/// 
	/// Note: whenever possible try to use MethodReference since it's extend the
	/// reach/usability of the code.
	/// </summary>
	public static class MethodRocks {

		public static bool IsNamed (this MemberReference self, TypeName typeName, string methodName)
		{
			if (methodName == null)
				throw new ArgumentNullException ("methodName");
			if (self == null)
				return false;
			return ((self.Name == methodName) && self.DeclaringType.IsNamed (typeName));
		}

		/// <summary>
		/// Check if the MethodReference is defined as the entry point of it's assembly.
		/// </summary>
		/// <param name="self">The MethodReference on which the extension method can be called.</param>
		/// <returns>True if the method is defined as the entry point of it's assembly, False otherwise</returns>
		public static bool IsEntryPoint (this MethodReference self)
		{
			return ((self != null) && (self == self.Module.Assembly.EntryPoint));
		}

		/// <summary>
		/// Check if the MethodReference is a finalizer.
		/// </summary>
		/// <param name="self">The MethodReference on which the extension method can be called.</param>
		/// <returns>True if the method is a finalizer, False otherwise.</returns>
		public static bool IsFinalizer (this MethodReference self)
		{
			if (self == null)
				return false;

			return (self.HasThis && !self.HasParameters && (self.Name == "Finalize") &&
				self.ReturnType.IsNamed (systemVoid));
		}

        private readonly static TypeName systemVoid = new TypeName
        {
            Namespace = "System",
            Name = "Void"
        };


        /// <summary>
        /// Check if the MethodReference is in F# code
        /// </summary>
        /// <param name="self">The MethodReference on which the extension method can be called.</param>
        /// <returns>True if the method is in F# code, False otherwise.</returns>
        public static bool IsFSharpCode(this MethodReference self)
        {
            if (self == null)
                return false;

            return self.DeclaringType.IsFSharpType();
        }


		/// <summary>
		/// Check if the method, or it's declaring type, was generated by the compiler or a tool (i.e. not by the developer).
		/// </summary>
		/// <param name="self">The MethodReference on which the extension method can be called.</param>
		/// <returns>True if the code is not generated directly by the developer, 
		/// False otherwise (e.g. compiler or tool generated)</returns>
		public static bool IsGeneratedCode (this MethodReference self)
		{
			if (self == null)
				return false;

			MethodDefinition method = self.Resolve ();
			if (method.HasAnyGeneratedCodeAttribute ())
				return true;

			return self.DeclaringType.IsGeneratedCode ();
		}

		/// <summary>
		/// Check if the signature of a method is consitent for it's use as a Main method.
		/// Note: it doesn't check that the method is the EntryPoint of it's assembly.
		/// <code>
		/// static [void|int] Main ()
		/// static [void|int] Main (string[] args)
		/// </code>
		/// </summary>gre
		/// <param name="self">The MethodReference on which the extension method can be called.</param>
		/// <returns>True if the method is a valid Main, False otherwise.</returns>
		public static bool IsMain (this MethodReference self)
		{
			if (self == null)
				return false;

			MethodDefinition method = self.Resolve ();
			// Main must be static
			if (!method.IsStatic)
				return false;

			if (method.Name != "Main")
				return false;

			// Main must return void or int
			switch (method.ReturnType.Name) {
			case "Void":
			case "Int32":
				// ok, continue checks
				break;
			default:
				return false;
			}

			// Main (void)
			if (!method.HasParameters)
				return true;

			IList<ParameterDefinition> pdc = method.Parameters;
			if (pdc.Count != 1)
				return false;

			// Main (string[] args)
			return (pdc [0].ParameterType.Name == "String[]");
		}

		/// <summary>
		/// Check if a method is an override to a virtual method of a base type.
		/// </summary>
		/// <param name="self">The MethodReference on which the extension method can be called.</param>
		/// <returns>True if the method is an override to a virtual method, False otherwise</returns>
		public static bool IsOverride (this MethodReference self)
		{
			if (self == null)
				return false;

			MethodDefinition method = self.Resolve ();
			if ((method == null) || !method.IsVirtual)
				return false;

			TypeDefinition declaring = method.DeclaringType;
			TypeDefinition parent = declaring.BaseType != null ? declaring.BaseType.Resolve () : null;
			while (parent != null) {
				string name = method.Name;
				foreach (MethodDefinition md in parent.Methods) {
					if (name != md.Name)
						continue;
					if (!method.CompareSignature (md))
						continue;

					return md.IsVirtual;
				}
				parent = parent.BaseType != null ? parent.BaseType.Resolve () : null;
			}
			return false;
		}

		/// <summary>
		/// Check if the method corresponds to the get or set operation on a property.
		/// </summary>
		/// <param name="self">The MethodReference on which the extension method can be called.</param>
		/// <returns>True if the method is a getter or a setter, False otherwise</returns>
		public static bool IsProperty (this MethodReference self)
		{
			if (self == null)
				return false;

			MethodDefinition method = self.Resolve ();
			if (method == null)
				return false;
			return ((method.SemanticsAttributes & (MethodSemanticsAttributes.Getter | MethodSemanticsAttributes.Setter)) != 0);
		}

		/// <summary>
		/// Check if the method is visible outside of the assembly.
		/// </summary>
		/// <param name="self">The MethodReference on which the extension method can be called.</param>
		/// <returns>True if the method can be used from outside of the assembly, false otherwise.</returns>
		public static bool IsVisible (this MethodReference self)
		{
			if (self == null)
				return false;

			MethodDefinition method = self.Resolve ();
			if ((method == null) || method.IsPrivate || method.IsAssembly)
				return false;
			return self.DeclaringType.Resolve ().IsVisible ();
		}

		/// <summary>
		/// Check if the method has the signature of an Event callback.
		/// They are usually of the form: void Method (object sender, EventArgs ea), where
		/// the second parameters is either EventArgs or a subclass of it
		/// </summary>
		/// <param name="self">The MethodReference on which the extension method can be called.</param>
		/// <returns>True if the method has the signature of an event callback.</returns>
		public static bool IsEventCallback (this MethodReference self)
		{
			if (self == null)
				return false;

			MethodDefinition method = self.Resolve ();
			if ((method == null) || !method.HasParameters)
				return false;

			IList<ParameterDefinition> parameters = method.Parameters;
			if (parameters.Count != 2)
				return false;

			TypeReference type = parameters [1].ParameterType;
			GenericParameter gp = (type as GenericParameter);
			if (gp == null)
				return type.Inherits (eventArgs);

            if (gp.Owner == method.DeclaringType)
            {
                int index = 0;
                var gps = method.DeclaringType.GenericParameters;

                for (; index < gps.Count; index++)
                {
                    if (gps[index].FullName == gp.FullName)
                    {
                        var gtype = (self.DeclaringType as Mono.Cecil.GenericInstanceType);
                        if (gtype.GenericArguments[index].Inherits(eventArgs))
                            return true;
                    }
                }

            }
			if (gp.HasConstraints) {
                IList<TypeReference> cc = gp.Constraints.Select(co => co.ConstraintType).ToList();
				return ((cc.Count == 1) && cc [0].IsNamed (eventArgs));
			}

			return false;
		}

        private readonly static TypeName eventArgs = new TypeName
        {
            Namespace = "System",
            Name = "EventArgs"
        };


		/// <summary>
		/// Returns a property using supplied MethodReference of 
		/// a property accessor method (getter or setter).
		/// </summary>
		/// <param name="self">The MethodReference on which the extension method can be called.</param>
		/// <returns>PropertyDefinition which corresponds to the supplied MethodReference.</returns>
		public static PropertyDefinition GetPropertyByAccessor (this MethodReference self)
		{
			if (self == null)
				return null;

			MethodDefinition method = self.Resolve ();
			if (method == null || !method.DeclaringType.HasProperties || !method.IsProperty ())
				return null;

			string mname = method.Name;
			foreach (PropertyDefinition property in method.DeclaringType.Properties) {
				// set_ and get_ both have a length equal to 4
				string pname = property.Name;
				if (String.CompareOrdinal (pname, 0, mname, 4, pname.Length) == 0)
					return property;
			}
			return null;
		}

		private static bool AreSameElementTypes (TypeReference a, TypeReference b)
		{
			return a.IsGenericParameter || b.IsGenericParameter || b.IsNamed (a.GetTypeName());
		}

		/// <summary>
		/// Compare the IMethodSignature members with the one being specified.
		/// </summary>
		/// <param name="self">>The IMethodSignature on which the extension method can be called.</param>
		/// <param name="signature">The IMethodSignature which is being compared.</param>
		/// <returns>True if the IMethodSignature members are identical, false otherwise</returns>
		public static bool CompareSignature (this IMethodSignature self, IMethodSignature signature)
		{
			if (self == null)
				return (signature == null);

			if (self.HasThis != signature.HasThis)
				return false;
			if (self.ExplicitThis != signature.ExplicitThis)
				return false;
			if (self.CallingConvention != signature.CallingConvention)
				return false;

			if (!AreSameElementTypes (self.ReturnType, signature.ReturnType))
				return false;

			bool h1 = self.HasParameters;
			bool h2 = signature.HasParameters;
			if (h1 != h2)
				return false;
			if (!h1 && !h2)
				return true;

			IList<ParameterDefinition> pdc1 = self.Parameters;
			IList<ParameterDefinition> pdc2 = signature.Parameters;
			int count = pdc1.Count;
			if (count != pdc2.Count)
				return false;

			for (int i = 0; i < count; ++i) {
				if (!AreSameElementTypes (pdc1 [i].ParameterType, pdc2 [i].ParameterType))
					return false;
			}
			return true;
		}
	}
}
