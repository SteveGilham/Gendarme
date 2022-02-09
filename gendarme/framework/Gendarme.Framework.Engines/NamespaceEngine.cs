//
// Gendarme.Framework.Engines.NamespaceEngine
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
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
//

using System;
using System.Collections.Generic;

using Mono.Cecil;

using Gendarme.Framework.Rocks;

namespace Gendarme.Framework.Engines
{
  public class NamespaceEngine : Engine
  {
    private static readonly Dictionary<AssemblyDefinition, HashSet<string>> assemblies = new Dictionary<AssemblyDefinition, HashSet<string>>();
    private static readonly Dictionary<string, HashSet<TypeDefinition>> namespaces = new Dictionary<string, HashSet<TypeDefinition>>();

    public override void Initialize(EngineController controller)
    {
      base.Initialize(controller);
      controller.BuildingType += new EventHandler<EngineEventArgs>(OnType);
      assemblies.Clear();
      namespaces.Clear();
    }

    private void OnType(object sender, EngineEventArgs e)
    {
      TypeDefinition type = (sender as TypeDefinition);
      string nspace = type.GetTypeName().Namespace;
      // we keep track of namespaces per assemblies
      AssemblyDefinition assembly = type.Module.Assembly;
      if (assemblies.TryGetValue(assembly, out HashSet<string> ns))
      {
        ns.AddIfNew(nspace);
      }
      else
      {
        ns = new HashSet<string>
        {
          nspace
        };
        assemblies.Add(assembly, ns);
      }

      // and types per namespaces
      if (!namespaces.TryGetValue(nspace, out HashSet<TypeDefinition> td))
      {
        td = new HashSet<TypeDefinition>();
        namespaces.Add(nspace, td);
      }
      td.Add(type);
    }

    /// <summary>
    /// Return all namespaces from all assemblies being analyzed.
    /// </summary>
    /// <returns>All namespaces defined in the assembly set</returns>
    public static IEnumerable<string> AllNamespaces()
    {
      foreach (string ns in namespaces.Keys)
      {
        yield return ns;
      }
    }

    /// <summary>
    /// Return if a namespace exist inside the assembly set
    /// </summary>
    /// <param name="nameSpace">Namespace to confirm existance</param>
    /// <returns>True if the namespace exists, False otherwise</returns>
    public static bool Exists(string nameSpace)
    {
      if (nameSpace == null)
        throw new ArgumentNullException(nameof(nameSpace));

      return namespaces.ContainsKey(nameSpace);
    }

    /// <summary>
    /// Return all namespaces defined inside the specified assembly.
    /// </summary>
    /// <param name="assembly">Assembly to search into</param>
    /// <returns>All namespaces defined in the specified assembly</returns>
    public static IEnumerable<string> NamespacesInside(AssemblyDefinition assembly)
    {
      if (assembly == null)
        throw new ArgumentNullException(nameof(assembly));

      if (!assemblies.TryGetValue(assembly, out HashSet<string> namespaces))
        yield return null;

      foreach (string ns in namespaces)
      {
        yield return ns;
      }
    }

    /// <summary>
    /// Return all types defined inside a namespace across all assemblies.
    /// </summary>
    /// <param name="nameSpace">Namespace to search into</param>
    /// <returns>All TypeDefinition defined the the specified namespace</returns>
    public static IEnumerable<TypeDefinition> TypesInside(string nameSpace)
    {
      if (nameSpace == null)
        throw new ArgumentNullException(nameof(nameSpace));

      if (!namespaces.TryGetValue(nameSpace, out HashSet<TypeDefinition> types))
        yield return null;

      foreach (TypeDefinition type in types)
      {
        yield return type;
      }
    }
  }
}