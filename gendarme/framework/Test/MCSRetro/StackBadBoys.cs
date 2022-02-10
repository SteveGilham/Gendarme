using System;

namespace MCSRetro
{
  public class StackEntry
  {
    public object TryCatchFinally()
    {
      object a = new object();
      object b = null;
      object c = null;
      try
      {
        if (new Random().Next() == 0)
        {
          a = null;
          return a;
        }
      }
      catch
      {
        b = a;
        if (new Random().Next() == 0)
          return a.ToString();
        try
        {
          a = null;
        }
        finally
        {
          c = b;
          b = a;
        }
      }
      finally
      {
      }
      if (new Random().Next() == 0)
        return b;
      else
        return c.GetHashCode();
    }

    public object MultipleCatch()
    {
      object a = new object();
      object b = null;
      object c = null;
      try
      {
        if (new Random().Next() == 0)
        {
          a = null;
          return a;
        }
      }
      catch (InvalidOperationException)
      {
        b = a;
      }
      catch (InvalidCastException)
      {
        c = a;
      }
      finally
      {
        a = null;
      }
      if (new Random().Next() == 0)
        return a;
      else if (new Random().Next() == 0)
        return b.GetHashCode();
      else
        return c.ToString();
    }
  }
}