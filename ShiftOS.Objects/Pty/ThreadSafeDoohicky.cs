using System;
using System.IO;
using System.Collections.Generic;

namespace Plex.Objects.Pty
{
    class ThreadSafeFifoBuffer : Stream
    {
        /*
         * Actual FIFO structure 
         */
        readonly Queue<byte> queueBuffer = new Queue<byte>();

        public override bool CanRead
        {
            get
            {
                return true; // Yes...
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                return -1;
            }
        }

        public override long Position
        {
            get
            {
                return queueBuffer.Count;
            }
            set
            {
                throw new NotSupportedException(); // No.
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {

            lock (queueBuffer)
            {

                int bytesRead = 0;

                for (int i = offset; i < offset + count; i++)
                {

                    if (queueBuffer.Count == 0)
                    {
                        System.Threading.Thread.Sleep(10);
                        break;
                    }

                    buffer[i] = queueBuffer.Dequeue();

                    bytesRead++;
                }

                return bytesRead;
            }

        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return -1; // No.
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(); // No.
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (queueBuffer)
            {
                for (int i = offset; i < offset + count; i++)
                {
                    queueBuffer.Enqueue(buffer[i]);
                }
            }
        }
    }
}