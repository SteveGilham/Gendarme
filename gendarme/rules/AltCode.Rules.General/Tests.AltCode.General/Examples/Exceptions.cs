using System;

namespace AltCode.Dixon.TestData
{
  public class Exceptions
  {
    private static void Fail1()
    {
      throw new InvalidOperationException("Fail1");
    }

    public static void Fail2()
    {
      try
      {
        Fail1();
      }
      catch (InvalidOperationException ioe)
      {
        Console.WriteLine(ioe);
        throw ioe;
      }
    }

    public static void FailSafe()
    {
      try
      {
        Fail1();
      }
      catch (InvalidOperationException ioe)
      {
        Console.WriteLine(ioe);
        throw;
      }
    }
  }
}