using System;
using System.IO;
using System.Threading;
using Plex.Objects.Streams;

namespace Plex.Objects.Pty
{
    /// <summary>
    /// Represents a stream used for a pseudo-terminal.
    /// </summary>
    public class PseudoTerminal : Stream
    {

        ThreadSafeFifoBuffer inputStream;
        ThreadSafeFifoBuffer outputStream;

        /// <inheritdoc/>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc/>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc/>
        public override long Length
        {
            get
            {
                return -1;
            }
        }

        /// <inheritdoc/>
        public override long Position
        {
            get
            {
                return -1;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        TerminalOptions options;

        bool isMaster;

        int lineBufferPosition = 0;
        byte[] lineBuffer = new byte[1024];

        PseudoTerminal(TerminalOptions ptyOptions,
                       ThreadSafeFifoBuffer inputPipe,
                       ThreadSafeFifoBuffer outputPipe,
                       bool isMaster)
        {
            inputStream = inputPipe;
            outputStream = outputPipe;
            options = ptyOptions;
            this.isMaster = isMaster;
        }


        void WriteOutput(byte c)
        {
            if (c == '\n' && (options.OFlag & PtyConstants.ONLCR) != 0)
            {
                outputStream.WriteByte((byte)'\r');
            }

            outputStream.WriteByte(c);
        }

        void WriteInput(byte c)
        {
            lock (inputStream)
            {
                if ((options.LFlag & PtyConstants.ICANON) != 0)
                {
                    if (c == options.C_cc[PtyConstants.VERASE])
                    {
                        if (lineBufferPosition > 0)
                        {
                            lineBufferPosition--;
                        }

                        lineBuffer[lineBufferPosition] = 0;

                        WriteOutput((byte)'\b');

                        return;
                    }

                    if (c == options.C_cc[PtyConstants.VINTR])
                    {
                        WriteOutput((byte)'^');
                        WriteOutput((byte)'C');
                        WriteOutput((byte)'\n');

                        Monitor.PulseAll(inputStream);
                        FlushLineBuffer();

                        return;

                    }

                    lineBuffer[lineBufferPosition++] = c;

                    if ((options.LFlag & PtyConstants.ECHO) != 0)
                    {
                        WriteOutput(c);
                    }

                    if (c == (byte)'\n')
                    {
                        Monitor.PulseAll(inputStream);
                        FlushLineBuffer();
                    }

                    return;
                }
                inputStream.WriteByte(c);
            }
        }

        void FlushLineBuffer()
        {
            inputStream.Write(lineBuffer, 0, lineBufferPosition);
            lineBufferPosition = 0;
            Monitor.PulseAll(inputStream);

        }

        /// <inheritdoc/>
        public override void Flush()
        {
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!isMaster)
            {
                return outputStream.Read(buffer, offset, count);
            }

            lock (inputStream)
            {
                int i;
                while ((i = inputStream.Read(buffer, offset, count)) == 0)
                {
                    Monitor.Wait(inputStream);
                }
                return i;
            }
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return -1;
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (isMaster)
            {
                for (int i = offset; i < offset + count; i++)
                {
                    WriteOutput(buffer[i]);
                }
            }
            else
            {
                for (int i = offset; i < offset + count; i++)
                {
                    WriteInput(buffer[i]);
                }
            }
        }

        /// <inheritdoc/>
        public static void CreatePair(out PseudoTerminal master, out PseudoTerminal slave, TerminalOptions options)
        {
            var inputStream = new ThreadSafeFifoBuffer();
            var outputStream = new ThreadSafeFifoBuffer();

            master = new PseudoTerminal(options, inputStream, outputStream, true);
            slave = new PseudoTerminal(options, inputStream, outputStream, false);
        }
    }

    /// <summary>
    /// An exception thrown by a <see cref="PseudoTerminal"/> when encountering a termination request byte. 
    /// </summary>
    public class TerminationRequestException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TerminationRequestException"/> exception. 
        /// </summary>
        public TerminationRequestException() : base("User requested termination of the current task.")
        {

        }
    }
}