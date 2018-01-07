using System;
using System.IO;

namespace Plex.Objects.Streams
{
    /// <summary>
    /// This is a wrapper to make another Stream read-only.
    /// </summary>
    public class ReadOnlyStream: Stream
    {
        private Stream baseStream; //this should not have been public.
        
        /// <inheritdoc/>
        public override bool CanRead { get { return true; } }
        /// <inheritdoc/>
        public override bool CanWrite { get { return false; } }
        /// <inheritdoc/>
        public override bool CanSeek { get { return baseStream.CanSeek; } }

        /// <inheritdoc/>
        public override long Length { get { return baseStream.Length; } }
        /// <inheritdoc/>
        public override long Position { get { return baseStream.Position; } set { baseStream.Position = value; } }

        /// <inheritdoc/>
        public override void Flush()
        {
            throw new NotSupportedException("The stream does not support writing.");
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return baseStream.Read(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return baseStream.Seek(offset, origin);
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotSupportedException("The stream does not support writing.");
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("The stream does not support writing.");
        }

        /// <inheritdoc/>
        public override void Close()
        {
            baseStream.Close();
        }

        /// <inheritdoc/>
        public ReadOnlyStream(Stream baseStream)
        {
            this.baseStream = baseStream;
        }
    }
}
