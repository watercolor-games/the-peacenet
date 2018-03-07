using System;
using System.IO;

namespace Plex.Objects.Streams
{
    /// <summary>
    /// This is a wrapper to make another Stream write-only.
    /// </summary>
    public class WriteOnlyStream: Stream
    {
        private Stream baseStream;

        /// <inheritdoc/>
        public override bool CanRead { get { return false; } }
        /// <inheritdoc/>
        public override bool CanWrite { get { return true; } }
        /// <inheritdoc/>
        public override bool CanSeek { get { return baseStream.CanSeek; } }

        /// <inheritdoc/>
        public override long Length { get { return baseStream.Length; } }
        /// <inheritdoc/>
        public override long Position { get { return baseStream.Position; } set { baseStream.Position = value; } }

        /// <inheritdoc/>
        public override void Flush()
        {
            baseStream.Flush();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("The stream does not support reading.");
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return baseStream.Seek(offset, origin);
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            baseStream.SetLength(value);
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            baseStream.Write(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override void Close()
        {
            baseStream.Close();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WriteOnlyStream"/> object 
        /// </summary>
        /// <param name="baseStream">The underlying stream to write to</param>
        public WriteOnlyStream(Stream baseStream)
        {
            this.baseStream = baseStream;
        }
    }
}
