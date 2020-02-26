using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    public override int Read(byte[] buffer, int offset, int count)
    {
      return inner.Read(buffer, offset, count);
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