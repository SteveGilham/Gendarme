using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Dummy
{
  internal class Decoy : ISerializable
  {
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      throw new NotImplementedException();
    }
  }
}

namespace Octo
{
  internal class Decoy : ISerializable
  {
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      throw new NotImplementedException();
    }
  }
}

namespace Gendarme.Rules.Globalization
{
  internal class BlobReadingStream : Stream
  {
    private readonly Stream inner;

    public BlobReadingStream(Stream inner)
    {
      this.inner = inner;
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

    private static readonly string bitmap =
      "System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    public override int Read(byte[] buffer, int offset, int count)
    {
      var num = inner.Read(buffer, offset, count);
      if (num > 0)
      {
        Console.WriteLine("{0} | {1}", num,
          String.Join(":", buffer.Skip(offset).Take(num)
          .Select(b => b.ToString("x2"))));
        Console.WriteLine("{0}",
          new String(buffer.Skip(offset).Take(num)
          .Select(b => (char)b)
          .Select(c => Char.IsControl(c) ? '\u2713' : c)
          .ToArray()));
        if (num == bitmap.Length &&
          bitmap.ToCharArray().Zip(buffer.Skip(offset).Take(num), (a, b) => (a == (char)b)).All(x => x)
          )
        {
          var name1 = typeof(Dummy.Decoy).AssemblyQualifiedName;
          var name2 = typeof(Octo.Decoy).AssemblyQualifiedName;
          var name = (name1.Length == bitmap.Length) ? name1 : name2;
          var bytes = name.ToCharArray().Select(c => (byte)c).ToArray();
          Array.Copy(bytes, 0, buffer, offset, num);
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