using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Examples.AltCode.PowerShell
{
  public struct StructsAreNotCmdlets
  {
    public string Name;
  }

  internal class InternalsAreNotCmdlets
  {
  }

  public static class UndecoratedClassesAreNotCmdlets
  {
    public static Type Internal()
    {
      return typeof(InternalsAreNotCmdlets);
    }
  }

  public class UnattributedTypesAreNotCmdlets : Cmdlet
  {
  }
}