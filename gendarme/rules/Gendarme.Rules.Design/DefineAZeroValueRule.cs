//
// Gendarme.Rules.Design.DefineAZeroValueRule
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

using Mono.Cecil;

using Gendarme.Framework;
using System.Runtime.InteropServices;

namespace Gendarme.Rules.Design
{
  abstract public class DefineAZeroValueRule : Rule
  {
    protected static FieldDefinition GetZeroValueField(TypeDefinition type)
    {
      if (type == null)
        return null;

      foreach (FieldDefinition field in type.Fields)
      {
        // the special __value field is not static like the others (user defined)
        if (!field.IsStatic)
          continue;
        object o = field.Constant;

        var value = long.MaxValue;

        // Don't be stupid if the underlying type is not int
        // That is a matter for a separate rule.
        switch (Type.GetTypeCode(o.GetType()))
        {
          case TypeCode.Byte:
            value = (byte)o;
            break;

          case TypeCode.SByte:
            value = (sbyte)o;
            break;

          case TypeCode.UInt16:
            value = (ushort)o;
            break;

          case TypeCode.UInt32:
            value = (uint)o;
            break;

          case TypeCode.Int16:
            value = (short)o;
            break;

          case TypeCode.Int32:
            value = (int)o;
            break;

          case TypeCode.UInt64:
          case TypeCode.Int64:
            value = (long)o;
            break;

          default:
            continue;
        }

        if (value == 0L)
          return field;
      }
      return null;
    }
  }
}