//
// SecurityDeclarationRocks.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2010 Jb Evain
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
using System.Security;
using SSP = System.Security.Permissions;

using Mono.Cecil;
using Gendarme.Framework.Rocks;

namespace Gendarme.Rules.Security.Cas
{
  public static class SecurityDeclarationRocks
  {
    private static IPermission CreatePermission(SecurityDeclaration declaration, SecurityAttribute attribute)
    {
      TypeReference atype = attribute.AttributeType;
      string name = atype.FullName;

      // most of the permissions resides inside mscorlib.dll
      Type attribute_type = Type.GetType(name);
      if (attribute_type == null)
      {
        // but not all of them, so we need to try harder :-)
        TypeDefinition rtype = atype.Resolve();
        AssemblyDefinition ad = rtype == null ? atype.Module.Assembly : rtype.Module.Assembly;
        attribute_type = Type.GetType(name + ", " + ad.FullName);
      }
      if (attribute_type == null)
        throw new ArgumentException("attribute");

      var security_attribute = CreateSecurityAttribute(attribute_type, declaration);
      if (security_attribute == null)
        throw new InvalidOperationException();

      CompleteSecurityAttribute(security_attribute, attribute);

      return security_attribute.CreatePermission();
    }

    private static void CompleteSecurityAttribute(SSP.SecurityAttribute security_attribute, SecurityAttribute attribute)
    {
      if (attribute.HasFields)
        CompleteSecurityAttributeFields(security_attribute, attribute);

      if (attribute.HasProperties)
        CompleteSecurityAttributeProperties(security_attribute, attribute);
    }

    private static void CompleteSecurityAttributeFields(SSP.SecurityAttribute security_attribute, ICustomAttribute attribute)
    {
      var type = security_attribute.GetType();

      foreach (var named_argument in attribute.Fields)
        type.GetField(named_argument.Name).SetValue(security_attribute, named_argument.Argument.Value);
    }

    private static void CompleteSecurityAttributeProperties(SSP.SecurityAttribute security_attribute, ICustomAttribute attribute)
    {
      var type = security_attribute.GetType();

      foreach (var named_argument in attribute.Properties)
        type.GetProperty(named_argument.Name).SetValue(security_attribute, named_argument.Argument.Value, null);
    }

    private static SSP.SecurityAttribute CreateSecurityAttribute(Type attribute_type, SecurityDeclaration declaration)
    {
      SSP.SecurityAttribute security_attribute;
      try
      {
        security_attribute = (SSP.SecurityAttribute)Activator.CreateInstance(
          attribute_type, new object[] { (SSP.SecurityAction)declaration.Action });
      }
      catch (MissingMethodException)
      {
        security_attribute = (SSP.SecurityAttribute)Activator.CreateInstance(attribute_type, new object[0]);
      }

      return security_attribute;
    }
  }
}