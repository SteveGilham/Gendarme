using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Examples.Rules.BadPractice
{
  public class DisableDebuggingCode
  {
    // note: [Conditional] is usable on type from 2.0 onward but only if it inherit from Attribute
    [Conditional("DEBUG")]
    public void ConditionalDebug()
    {
      Console.WriteLine("debug");
    }

    [Conditional("TRACE")]
    public void ConditionalTrace()
    {
      Console.WriteLine("debug");
    }

    [Conditional("OTHER")]
    [Conditional("DEBUG")]
    public void ConditionalMultiple()
    {
      Console.WriteLine("debug");
    }

    [Conditional("OTHER")]
    public void ConditionalOther()
    {
      Console.WriteLine("debug");
    }

    public void UsingTrace()
    {
      Trace.WriteLine("debug");
    }

    public void UsingDebug()
    {
      Debug.WriteLine("debug");
    }

    [Category("DEBUG")] // wrong attribute
    public void UsingConsole()
    {
      Console.WriteLine("debug");
    }
  }
}