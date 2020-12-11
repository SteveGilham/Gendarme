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

#if NETSTANDARD2_0
namespace System.Security
{
    public class PermissionSet : IPermission
    {
      private bool m_Unrestricted;

      public PermissionSet(System.Security.Permissions.PermissionState state)
      {
		      switch (state)
		      {
		      case System.Security.Permissions.PermissionState.Unrestricted:
			      m_Unrestricted = true;
			      break;

		      case System.Security.Permissions.PermissionState.None:
			       m_Unrestricted = false;
			      break;

		      default:
			      throw new ArgumentException("Argument_InvalidPermissionState");
		      }
      }

      public bool IsSubsetOf(IPermission other)
      {
        var target = other as PermissionSet;
	      if (target == null || !target.m_Unrestricted)
	      {
		      return !m_Unrestricted;
	      }
	      if (m_Unrestricted && !target.m_Unrestricted)
	      {
		      return false;
	      }
		    return true;
      }

      public void AddPermission(IPermission other)
      {
        if (other == null)
          return;
        throw new NotImplementedException("AddPermission foreign " + other.GetType().FullName);
      }

      public IPermission Copy()
      {
        throw new NotImplementedException();
      }

      public void Demand()
      {
        throw new NotImplementedException();
      }

      public IPermission Intersect(IPermission x)
      {
        throw new NotImplementedException();
      }

      public IPermission Union(IPermission x)
      {
        throw new NotImplementedException();
      }

      public void FromXml(SecurityElement x)
      {
        throw new NotImplementedException();
      }

      public SecurityElement ToXml()
      {
        throw new NotImplementedException();
      }
    }
}

namespace System.Security.Permissions
{
  public enum PermissionState
  {
	  /// <summary>Full access to the resource protected by the permission.</summary>
	  Unrestricted = 1,
	  /// <summary>No access to the resource protected by the permission.</summary>
	  None = 0
  }

  class PermissionSetAttribute : Attribute
  {
    public PermissionSetAttribute(SecurityAction action)
    {
      UnicodeEncoded = false;
      Action = action;
    }

    public bool Unrestricted { get;set; }
    public bool UnicodeEncoded { get; set; }
    public string XML { get; set; }
    public string Name { get; set; }
    public string File { get; set; }
    public string Hex { get; set; }
    public SecurityAction Action { get; set; }

    public PermissionSet CreatePermissionSet()
    {
	    if (Unrestricted)
	    {
		    return new PermissionSet(PermissionState.Unrestricted);
	    }
	    if (Name != null)
	    {
        throw new NotImplementedException("Name");
//		    return PolicyLevel.GetBuiltInSet(m_name);
	    }
	    if (XML != null)
	    {
        throw new NotImplementedException("XML");
//		    return ParsePermissionSet(new Parser(m_xml.ToCharArray()));
	    }
	    if (Hex != null)
	    {
        throw new NotImplementedException("Hex");
//		    return BruteForceParseStream(new MemoryStream(System.Security.Util.Hex.DecodeHexString(m_hex)));
	    }
	    if (File != null)
	    {
        throw new NotImplementedException("File");
//		    return BruteForceParseStream(new FileStream(m_file, FileMode.Open, FileAccess.Read));
	    }
	    return new PermissionSet(PermissionState.None);
    }
  }
}
#endif

namespace Gendarme.Rules.Security.Cas
{
#if NETSTANDARD2_0
#endif

  static public class SecurityDeclarationRocks
  {
    public static PermissionSet ToPermissionSet(this SecurityDeclaration self)
    {
      if (self == null)
        throw new ArgumentNullException("self");

      PermissionSet set;
      if (TryProcessPermissionSetAttribute(self, out set))
        return set;

      return CreatePermissionSet(self);
    }

    private static bool TryProcessPermissionSetAttribute(SecurityDeclaration declaration, out PermissionSet set)
    {
      set = null;

      if (!declaration.HasSecurityAttributes)
        return false;

      var attributes = declaration.SecurityAttributes;
      if (attributes.Count != 1)
        return false;

      var security_attribute = attributes[0];
      var attribute_type = security_attribute.AttributeType;
      if (attribute_type.Name != "PermissionSetAttribute" || attribute_type.Namespace != "System.Security.Permissions") // OK
        return false;

      var attribute = new SSP.PermissionSetAttribute((System.Security.Permissions.SecurityAction)declaration.Action);

      foreach (var named_argument in security_attribute.Properties)
      {
        object value = named_argument.Argument.Value;
        switch (named_argument.Name)
        {
          case "Unrestricted":
            attribute.Unrestricted = (bool)value;
            break;

          case "UnicodeEncoded":
            attribute.UnicodeEncoded = (bool)value;
            break;

          case "XML":
            attribute.XML = (string)value;
            break;

          case "Name":
            attribute.Name = (string)value;
            break;

          case "File":
            attribute.File = (string)value;
            break;

          case "Hex":
            attribute.Hex = (string)value;
            break;

          default:
            throw new NotImplementedException(named_argument.Name);
        }
      }

      set = attribute.CreatePermissionSet();
      return true;
    }

    private static PermissionSet CreatePermissionSet(SecurityDeclaration declaration)
    {
      var set = new PermissionSet(SSP.PermissionState.None);

      foreach (var attribute in declaration.SecurityAttributes)
      {
        var permission = CreatePermission(declaration, attribute);
        set.AddPermission(permission);
      }

      return set;
    }

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

    private static void CompleteSecurityAttribute(System.Security.Permissions.SecurityAttribute security_attribute, SecurityAttribute attribute)
    {
      if (attribute.HasFields)
        CompleteSecurityAttributeFields(security_attribute, attribute);

      if (attribute.HasProperties)
        CompleteSecurityAttributeProperties(security_attribute, attribute);
    }

    private static void CompleteSecurityAttributeFields(System.Security.Permissions.SecurityAttribute security_attribute, ICustomAttribute attribute)
    {
      var type = security_attribute.GetType();

      foreach (var named_argument in attribute.Fields)
        type.GetField(named_argument.Name).SetValue(security_attribute, named_argument.Argument.Value);
    }

    private static void CompleteSecurityAttributeProperties(System.Security.Permissions.SecurityAttribute security_attribute, ICustomAttribute attribute)
    {
      var type = security_attribute.GetType();

      foreach (var named_argument in attribute.Properties)
        type.GetProperty(named_argument.Name).SetValue(security_attribute, named_argument.Argument.Value, null);
    }

    private static System.Security.Permissions.SecurityAttribute CreateSecurityAttribute(Type attribute_type, SecurityDeclaration declaration)
    {
      System.Security.Permissions.SecurityAttribute security_attribute;
      try
      {
        security_attribute = (System.Security.Permissions.SecurityAttribute)Activator.CreateInstance(
          attribute_type, new object[] { (SSP.SecurityAction)declaration.Action });
      }
      catch (MissingMethodException)
      {
        security_attribute = (System.Security.Permissions.SecurityAttribute)Activator.CreateInstance(attribute_type, new object[0]);
      }

      return security_attribute;
    }
  }
}