using System;

namespace Examples.Rules.Performance
{
  public class AvoidUncalledPrivateCode
  {
    public class CalledPrivateMethod
    {
      private void display()
      {
      }

      public static void Main(string[] args)
      {
        CalledPrivateMethod c = new CalledPrivateMethod();
        c.display();
      }
    }

    public class CalledInternalMethod
    {
      internal void CalledMethod()
      {
      }
    }

    public class MethodCallingClass
    {
      public static void Main(string[] args)
      {
        CalledInternalMethod c = new CalledInternalMethod();
        c.CalledMethod();
      }
    }

    private class PublicMethodsInPrivateClass
    {
      public void PublicCalledMethod()
      {
      }

      public virtual void PublicVirtualUncalledMethod()
      {
      }

      public void PublicUncalledMethod()
      {
      }

      public static void Main(string[] args)
      {
        PublicMethodsInPrivateClass p = new PublicMethodsInPrivateClass();
        p.PublicCalledMethod();
      }
    }

    internal class PublicMethodsInInternalClass
    {
      public void PublicCalledMethod()
      {
      }

      public virtual void PublicVirtualUncalledMethod()
      {
      }

      public void PublicUncalledMethod()
      {
      }

      public static void Main(string[] args)
      {
        PublicMethodsInInternalClass p = new PublicMethodsInInternalClass();
        p.PublicCalledMethod();
      }
    }
  }
}