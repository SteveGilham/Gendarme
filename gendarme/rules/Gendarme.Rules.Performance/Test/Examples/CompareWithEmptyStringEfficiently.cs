using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examples.Rules.Performance
{
  public class CompareWithEmptyStringEfficiently
  {
    public class UsingStringEquals
    {
      private string s = "";

      public static void Main(string[] args)
      {
        UsingStringEquals u = new UsingStringEquals();
        if (u.s.Equals(""))
        {
        }
      }
    }

    public class UsingStringEqualsEmpty
    {
      private string s = "";

      public static void Main(string[] args)
      {
        UsingStringEqualsEmpty u = new UsingStringEqualsEmpty();
        if (u.s.Equals(String.Empty))
        {
        }
      }
    }

    public class UsingStringLength
    {
      private string s = "";

      public static void Main(string[] args)
      {
        UsingStringLength u = new UsingStringLength();
        if (u.s.Length == 0)
        {
        }
      }
    }

    public class UsingEqualsWithNonStringArg
    {
      private int i = 0;

      public static void Main(string[] args)
      {
        UsingEqualsWithNonStringArg u = new UsingEqualsWithNonStringArg();
        if (u.i.Equals(1))
        {
        }
      }
    }

    public class AnotherUseOfEqualsWithEmptyString
    {
      private string s = "abc";

      public static void Main(string[] args)
      {
        AnotherUseOfEqualsWithEmptyString a = new AnotherUseOfEqualsWithEmptyString();
        bool b = a.s.Equals("");
      }
    }

    public class AnotherUseOfEqualsWithStringEmpty
    {
      private string s = "abc";

      public static void Main(string[] args)
      {
        AnotherUseOfEqualsWithStringEmpty a = new AnotherUseOfEqualsWithStringEmpty();
        bool b = a.s.Equals(String.Empty);
      }
    }

    public class OneMoreUseOfEqualsWithEmptyString
    {
      private string s = "";

      public static void Main(string[] args)
      {
        OneMoreUseOfEqualsWithEmptyString o = new OneMoreUseOfEqualsWithEmptyString();
        if (o.s.Equals(""))
        {
          bool b = o.s.Equals("");
        }
      }
    }

    public class UsingEqualsWithNonEmptyString
    {
      private string s = "";

      public static void Main(string[] args)
      {
        UsingEqualsWithNonEmptyString u = new UsingEqualsWithNonEmptyString();
        if (u.s.Equals("abc"))
        {
        }
      }
    }
  }
}