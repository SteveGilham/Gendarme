using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

[assembly: SuppressMessage("AltCode.Rules.General",
                           "AvoidAssemblySemanticVersionMismatchRule",
                           Justification = "Assembly version needed for mocking purposes")]

namespace AltCode.Mocker
{
  [Serializable]
  public sealed class Bitmap : ISerializable
  {
    public Bitmap()
    { }

#pragma warning disable IDE0060 // Remove unused parameter
    public Bitmap(Stream dummy)
#pragma warning restore IDE0060 // Remove unused parameter
    { }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    { }

    private Bitmap(SerializationInfo info, StreamingContext context)
    {
      SerializationInfoEnumerator enumerator = info.GetEnumerator();
      if (enumerator == null)
      {
        return;
      }
      while (enumerator.MoveNext())
      {
        if (!string.Equals(enumerator.Name, "Data", StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }
        try
        {
          byte[] array = (byte[])enumerator.Value;
        }
        catch (ExternalException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (OutOfMemoryException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (FileNotFoundException)
        {
        }
      }
    }
  }
}