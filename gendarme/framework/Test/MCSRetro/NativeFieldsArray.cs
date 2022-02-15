using System;
using System.Runtime.InteropServices;

namespace Test.Rules.Design
{  
  internal class NativeFieldsArray : ICloneable
  {
    private object A;
    private IntPtr[] B;

    public object Clone()
    {
      B = new IntPtr[1];
      // assignation (newobj+stfld) does not need to to be inside ctor
      // note: fxcop does not catch this one
      B[0] = Marshal.AllocCoTaskMem(1);
      A = B;
      return A;
    }
  }
}