namespace Examples.Rules.BadPractice
{
  public class ClassCallingGetEntryAssembly
  {
    public static void MainName() // fake main
    {
    }

    public void OneCall()
    {
      object o = System.Reflection.Assembly.GetEntryAssembly();
    }

    public void ThreeCalls()
    {
      string s = System.Reflection.Assembly.GetEntryAssembly().ToString();
      int x = 2 + 2;
      x = x.CompareTo(1);
      object o = System.Reflection.Assembly.GetEntryAssembly();
      System.Reflection.Assembly.GetEntryAssembly();
    }

    public void NoCalls()
    {
      int x = 42;
      int y = x * 42;
      x = x * y.CompareTo(42);
    }
  }
}