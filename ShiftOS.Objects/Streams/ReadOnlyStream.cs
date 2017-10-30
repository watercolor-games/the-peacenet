using System;
using System.IO;

namespace Plex.Objects.Streams
{
    /// <summary>
    /// This is a wrapper to make another Stream read-only.
    /// </summary>
    public class ReadOnlyStream: Stream
    {
        public Stream baseStream;
        
        public override bool CanRead { get { return true; } }
        public override bool CanWrite { get { return false; } }
        public override bool CanSeek { get { return baseStream.CanSeek; } }
        
        public override long Length { get { return baseStream.Length; } }
        public override long Position { get { return baseStream.Position; } set { baseStream.Position = value; } }
        
        public override void Flush()
        {
            throw new NotSupportedException("The stream does not support writing.");
        }
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            return baseStream.Read(buffer, offset, count);
        }
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            return baseStream.Seek(offset, origin);
        }
        
        public override void SetLength(long value)
        {
            throw new NotSupportedException("The stream does not support writing.");
        }
        
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("The stream does not support writing.");
        }
        
        public void Dispose()
        {
            baseStream.Dispose();
        }
        
        public ReadOnlyStream(Stream baseStream)
        {
            this.baseStream = baseStream;
        }
    }
}
