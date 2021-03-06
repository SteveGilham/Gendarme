//
// AssemblyResolver.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//   Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2007-2008 Novell, Inc (http://www.novell.com)
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
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Mono.Cecil;

using Gendarme.Framework.Rocks;

namespace Gendarme.Framework
{
  public class AssemblyResolver : BaseAssemblyResolver
  {
    private Dictionary<string, AssemblyDefinition> assemblies;

    private AssemblyResolver()
    {
      assemblies = new Dictionary<string, AssemblyDefinition>();
      ResolveFailure += AltCode.CecilExtensions.NetCoreResolver.ResolveHandler;
    }

    public IDictionary<string, AssemblyDefinition> AssemblyCache
    {
      get { return assemblies; }
    }

    public override AssemblyDefinition Resolve(AssemblyNameReference name)
    {
      if (name == null)
        throw new ArgumentNullException("name");

      string aname = name.FullName;
      AssemblyDefinition asm = null;
      if (!assemblies.TryGetValue(aname, out asm))
      {
        try
        {
          asm = base.Resolve(name);
          AltCode.CecilExtensions.NetCoreResolver.HookResolver(
                asm.MainModule.AssemblyResolver);
        }
        catch (FileNotFoundException)
        {
          // note: analysis will be incomplete
        }
        assemblies.Add(aname, asm);
      }
      return asm;
    }

    public void CacheAssembly(AssemblyDefinition assembly)
    {
      if (assembly == null)
        throw new ArgumentNullException("assembly");

      assemblies.Add(assembly.Name.Name, assembly);
      string location = Path.GetDirectoryName(assembly.MainModule.FileName);
      AddSearchDirectory(location);
    }

    static private AssemblyResolver resolver;

    static public AssemblyResolver Resolver
    {
      get
      {
        if (resolver == null)
        {
          resolver = new AssemblyResolver();
        }
        return resolver;
      }
    }
  }
}