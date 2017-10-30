using System;
using System.IO;

namespace Plex.Objects.Streams
{
    /// <summary>
    /// This is a wrapper to make another Stream write-only.
    /// </summary>
    public class WriteOnlyStream: Stream
    {
        public Stream baseStream;
        
        public override bool CanRead { get { return false; } }
        public override bool CanWrite { get { return true; } }
        public override bool CanSeek { get { return baseStream.CanSeek; } }
        
        public override long Length { get { return baseStream.Length; } }
        public override long Position { get { return baseStream.Position; } set { baseStream.Position = value; } }
        
        public override void Flush()
        {
            baseStream.Flush();
        }
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("The stream does not support reading.");
        }
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            return baseStream.Seek(offset, origin);
        }
        
        public override void SetLength(long value)
        {
            baseStream.SetLength(value);
        }
        
        public override void Write(byte[] buffer, int offset, int count)
        {
            baseStream.Write(buffer, offset, count);
        }
        
        public void Dispose()
        {
            baseStream.Dispose();
        }
        
        public WriteOnlyStream(Stream baseStream)
        {
            this.baseStream = baseStream;
        }
    }
}
