//
// Gendarme.Framework.Rocks.CustomAttributeRocks
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2007-2008 Novell, Inc (http://www.novell.com)
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

namespace Gendarme.Framework.Rocks
{
  // add CustomAttribute[Collection], ICustomAttributeProvider extensions
  // methods here only if:
  // * you supply minimal documentation for them (xml)
  // * you supply unit tests for them
  // * they are required somewhere to simplify, even indirectly, the rules
  //   (i.e. don't bloat the framework in case of x, y or z in the future)

  /// <summary>
  /// CustomAttributeRocks contains extensions methods for CustomAttribute
  /// and the related collection classes.
  /// </summary>
	public static class CustomAttributeRocks
  {
    internal static bool HasAnyGeneratedCodeAttribute(this ICustomAttributeProvider self)
    {
      if ((self == null) || !self.HasCustomAttributes)
        return false;

      foreach (CustomAttribute ca in self.CustomAttributes)
      {
        TypeReference cat = ca.AttributeType;
        if (cat.IsNamed(generatedCode) ||
          cat.IsNamed(compilerGenerated))
        {
          return true;
        }
      }
      return false;
    }

    private readonly static TypeName generatedCode = new TypeName
    {
      Namespace = "System.CodeDom.Compiler",
      Name = "GeneratedCodeAttribute"
    };

    private readonly static TypeName compilerGenerated = new TypeName
    {
      Namespace = "System.Runtime.CompilerServices",
      Name = "CompilerGeneratedAttribute"
    };

    /// <summary>
    /// Check if the type contains an attribute of a specified type.
    /// </summary>
    /// <param name="self">The ICustomAttributeProvider (e.g. AssemblyDefinition, TypeReference, MethodReference,
    /// FieldReference...) on which the extension method can be called.</param>
    /// <param name="nameSpace">The namespace of the attribute to be matched</param>
    /// <param name="name">The name of the attribute to be matched</param>
    /// <returns>True if the provider contains an attribute of the same name,
    /// False otherwise.</returns>
    public static bool HasAttribute(this ICustomAttributeProvider self, TypeName typename)
    {
      if (typename.Namespace == null)
        throw new ArgumentNullException("nameSpace");
      if (typename.Name == null)
        throw new ArgumentNullException("name");

      if ((self == null) || !self.HasCustomAttributes)
        return false;

      foreach (CustomAttribute ca in self.CustomAttributes)
      {
        if (ca.AttributeType.IsNamed(typename))
          return true;
      }
      return false;
    }

    public static bool HasAttribute<T>(this ICustomAttributeProvider self)
    {
      if ((self == null) || !self.HasCustomAttributes)
        return false;

      foreach (CustomAttribute ca in self.CustomAttributes)
      {
        if (ca.AttributeType.FullName == typeof(T).FullName)
          return true;
      }
      return false;
    }

    private static bool IsSumType(CustomAttribute a)
    {
      var b = a.GetBlob();
      if (b.Length != 8)
        return false;
      var v = BitConverter.ToInt64(b, 0);
      var mask = v & 0x1fffff;
      return mask == 0x10001;
    }

    public static bool IsSumType(this ICustomAttributeProvider self)
    {
      if ((self == null) || !self.HasCustomAttributes)
        return false;

      //return self.CustomAttributes.Any(a => a.AttributeType.FullName == "Microsoft.FSharp.Core.CompilationMappingAttribute" &&
      //                                      a.ConstructorArguments.Count == 1 &&
      //                                      (0x1f & (int)a.ConstructorArguments[0].Value) == 1); // Sum type
      return self.CustomAttributes.Any(a => a.AttributeType.FullName == "Microsoft.FSharp.Core.CompilationMappingAttribute" &&
                                            IsSumType(a)); // Sum type
    }

    public static bool IsRecordType(this ICustomAttributeProvider self)
    {
      if ((self == null) || !self.HasCustomAttributes)
        return false;

      return self.CustomAttributes.Any(a => a.AttributeType.FullName == "Microsoft.FSharp.Core.CompilationMappingAttribute" &&
                                            a.ConstructorArguments.Count == 1 &&
                                            (0x1f & (int)a.ConstructorArguments[0].Value) == 2); // Record type
    }

    public static bool IsObjectType(this ICustomAttributeProvider self)
    {
      if ((self == null) || !self.HasCustomAttributes)
        return false;

      return self.CustomAttributes.Any(a => a.AttributeType.FullName == "Microsoft.FSharp.Core.CompilationMappingAttribute" &&
                                            a.ConstructorArguments.Count == 1 &&
                                            (0x1f & (int)a.ConstructorArguments[0].Value) == 3); // Object type
    }

    public static bool IsFieldType(this ICustomAttributeProvider self)
    {
      if ((self == null) || !self.HasCustomAttributes)
        return false;

      return self.CustomAttributes.Any(a => a.AttributeType.FullName == "Microsoft.FSharp.Core.CompilationMappingAttribute" &&
                                            a.ConstructorArguments.Count == 2 &&
                                            (0x1f & (int)a.ConstructorArguments[0].Value) == 4); // Field type
    }

    public static bool IsClosureType(this ICustomAttributeProvider self)
    {
      if ((self == null) || !self.HasCustomAttributes)
        return false;

      return self.CustomAttributes.Any(a => a.AttributeType.FullName == "Microsoft.FSharp.Core.CompilationMappingAttribute" &&
                                            a.ConstructorArguments.Count == 1 &&
                                            (0x1f & (int)a.ConstructorArguments[0].Value) == 6); // Closure type
    }

    public static bool IsModuleType(this ICustomAttributeProvider self)
    {
      if ((self == null) || !self.HasCustomAttributes)
        return false;

      return self.CustomAttributes.Any(a => a.AttributeType.FullName == "Microsoft.FSharp.Core.CompilationMappingAttribute" &&
                                            a.ConstructorArguments.Count == 1 &&
                                            (0x1f & (int)a.ConstructorArguments[0].Value) == 7); // Module type
    }

    public static bool IsUnionCase(this ICustomAttributeProvider self)
    {
      if ((self == null) || !self.HasCustomAttributes)
        return false;

      return self.CustomAttributes.Any(a => a.AttributeType.FullName == "Microsoft.FSharp.Core.CompilationMappingAttribute" &&
                                            a.ConstructorArguments.Count == 2 &&
                                            (0x1f & (int)a.ConstructorArguments[0].Value) == 8); // Module type
    }
  }
}