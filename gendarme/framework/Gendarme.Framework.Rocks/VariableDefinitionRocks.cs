//
// Gendarme.Framework.Rocks.VariableDefinitionRocks
//
// Authors:
//	Jesse Jones <jesjones@mindspring.com>
//
// 	(C) 2009 Jesse Jones
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
using System.Globalization;
using Mono.Cecil.Cil;

namespace Gendarme.Framework.Rocks {
	
	public static class VariableDefinitionRocks {
		
		/// <summary>
		/// Check if the local variable has a name which was generated by the compiler. 
		/// Note that this will return true for all variables if debugging information is not present.
		/// </summary>
		/// <param name="self">The VariableReference on which the extension method can be called.</param>
		/// <returns>True if the field name was generated by the compiler, False otherwise</returns>
        public static bool IsGeneratedName(this VariableReference self, MethodDebugInformation dbg)
		{
			if (self == null)
				return false;

			string name = self.MaybeGetName(dbg);
			if (String.IsNullOrEmpty (name))
				return true;

			return ((name [0] == '<') || (name.IndexOf ('$') != -1));
		}

        public static string GetName(this VariableReference self, MethodDebugInformation dbg)
		{
			if (self == null)
				return String.Empty;
            var name = self.MaybeGetName(dbg);
			return !string.IsNullOrEmpty (name) ? name : "V_" + self.Index.ToString (CultureInfo.InvariantCulture);
		}

        public static string MaybeGetName(this VariableReference self, MethodDebugInformation dbg)
        {
            var def = self.Resolve();
            return MaybeGetName(def, dbg);
        }

        public static string MaybeGetName(this VariableDefinition self, Mono.Cecil.MethodDefinition method)
        {
            return MaybeGetName(self, method.DebugInformation);
        }

        public static string MaybeGetName(this VariableDefinition self, MethodDebugInformation dbg)
        {
            string name = String.Empty;
            if (dbg != null)
            {
                dbg.TryGetName(self, out name);
            }
            return name;
        }
	}
}
