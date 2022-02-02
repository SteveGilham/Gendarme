using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gendarme.Rules.Globalization
{
  internal class BlobReadingStream : Stream
  {
    private readonly Stream inner;

    public BlobReadingStream(Stream stream)
    {
      this.inner = stream;
    }

    public override bool CanRead => inner.CanRead;

    public override bool CanSeek => inner.CanSeek;

    public override bool CanWrite => inner.CanWrite;

    public override long Length => inner.Length;

    public override long Position
    {
      get => inner.Position;
      set { inner.Position = value; }
    }

    public override void Flush()
    {
      inner.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      var num = inner.Read(buffer, offset, count);
      if (num > 0)
      {
        var tname = "System.Drawing.Bitmap,";
        if (num > tname.Length)
        {
          var maybe = new String(buffer.Skip(offset).Take(num).Select(b => (char)b).ToArray());

          if (maybe.StartsWith(tname, StringComparison.Ordinal))
          {
            var rtype = Type.GetType(maybe, false);
            if (rtype == null)
            {
              //Console.WriteLine(maybe);

              var sub = typeof(AltCode.Mocker.Bitmap).AssemblyQualifiedName;
              var bits = System.Text.Encoding.ASCII.GetBytes(sub);
              Array.Copy(bits, 0, buffer, offset, num);
            }
          }
        }
      }
      return num;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      return inner.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      inner.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      inner.Write(buffer, offset, count);
    }
  }
}