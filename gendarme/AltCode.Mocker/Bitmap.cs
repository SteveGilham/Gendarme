using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace AltCode.Mocker
{
  [Serializable]
  public sealed class Bitmap : ISerializable
  {
    public Bitmap()
    { }

#pragma warning disable IDE0060 // Remove unused parameter
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", 
      Justification="Called by deserializer")]
    public Bitmap(Stream dummy)
#pragma warning restore IDE0060 // Remove unused parameter
    { }

#pragma warning disable IDE0060 // Remove unused parameter
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", 
      Justification="Meets interface")]
    public void GetObjectData(SerializationInfo info, StreamingContext context)
#pragma warning restore IDE0060 // Remove unused parameter
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