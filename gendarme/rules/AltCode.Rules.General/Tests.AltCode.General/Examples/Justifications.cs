using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]

namespace Examples.AltCode.General
{
  [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
  public class Justifications
  {
    /// <summary>
    /// Should generate a "No Justification given" analysis warning
    /// </summary>
    /// <returns>A constant string</returns>
    [SuppressMessage("Microsoft.Naming",
                     "CA1704:IdentifiersShouldBeSpelledCorrectly",
                     Justification = "Generated code")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Naming",
                     "CA1724:TypeNamesShouldNotMatchNamespaces",
                     Justification = "That's life I'm afraid")]
    public string Token()
    {
      return "Canary";
    }

    /// <summary>
    /// Should generate a "No Justification given" analysis warning
    /// </summary>
    /// <returns>A constant string</returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
      Justification = "  ")]
    public string EmptyToken()
    {
      return "Canary";
    }

    /// <summary>
    /// Should generate a "No Justification given" analysis warning
    /// </summary>
    /// <returns>A constant string</returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
      Justification = "Shan't")]
    public string AnotherToken()
    {
      return "Canary";
    }

    /// <summary>
    /// Should not generate a "No Justification given" analysis warning
    /// </summary>
    /// <returns>A constant string</returns>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
      Justification = "Because of reasons")]
    public string YetAnotherToken()
    {
      return "Canary";
    }

    /// <summary>
    /// Should not generate a "No Justification given" analysis warning
    /// </summary>
    /// <returns>A constant string</returns>
    [ExcludeFromCodeCoverage]
    public string Token4(int x)
    {
      return "Canary" + x.ToString();
    }
  }
}