﻿// 
// Gendarme.Rules.NUnit.NUnitRule
//
// Authors:
//	Yuri Stuken <stuken.yuri@gmail.com>
//
// Copyright (C) 2010 Yuri Stuken
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

using Mono.Cecil;
using Mono.Cecil.Cil;

using Gendarme.Framework;
using Gendarme.Framework.Engines;
using Gendarme.Framework.Helpers;
using Gendarme.Framework.Rocks;

namespace Gendarme.Rules.NUnit {

    /// <summary>
    /// 
    /// </summary>
	abstract public class NUnitRule : Rule {

        /// <summary>
        /// 
        /// </summary>
		public Version NUnitVersion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="runner"></param>
		public override void Initialize (IRunner runner)
		{
			base.Initialize (runner);

			// If the assembly doesn't references nunit.framework then it 
			// obviously doesn't use any of its types
			Runner.AnalyzeModule += (object o, RunnerEventArgs e) => 
			{
				Active = false;
				foreach (AssemblyNameReference assembly in e.CurrentModule.AssemblyReferences) {
					if (assembly.Name == "nunit.framework") {
						Active = true;
						NUnitVersion = assembly.Version;
						return;
					}
				}

			};
		}
	}
}