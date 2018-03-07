using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DocoptNet;

namespace Plex.Objects
{ 
    /// <summary>
    /// Encapsulates a standard input and output stream for a Peacenet console.
    /// </summary>
    public class ConsoleContext
    {
        private StreamReader _stdin = null;
        private StreamWriter _stdout = null;
        private string workdir = null;

        /// <summary>
        /// Slowly write text to the console.
        /// </summary>
        /// <param name="text">The text to write.</param>
        public void SlowWrite(string text)
        {
            foreach(char c in text)
            {
                _stdout.Write(c.ToString());
                System.Threading.Thread.Sleep(25);

            }
            _stdout.Write("\n");
        }

        /// <summary>
        /// Retrieve the standard input stream.
        /// </summary>
        public StreamReader StandardInput
        {
            get
            {
                return _stdin;
            }
        }

        /// <summary>
        /// Retrieve the standard output stream.
        /// </summary>
        public StreamWriter StandardOutput
        {
            get
            {
                return _stdout;
            }
        }

        /// <summary>
        /// Create a console context which uses this context's standard output as the input, and the specified <see cref="Stream"/> as its output. 
        /// </summary>
        /// <param name="master">A stream to which output should be written.</param>
        /// <returns>The piped <see cref="ConsoleContext"/> object.</returns>
        public ConsoleContext Pipe(Stream master)
        {
            var writer = new StreamWriter(master);
            writer.AutoFlush = true;
            var reader = new StreamReader(_stdout.BaseStream);
            return new ConsoleContext(writer, reader);
        }

        /// <summary>
        /// Reads to the end of the standard input stream.
        /// </summary>
        /// <returns>The text read from the stream.</returns>
        public string ReadToEnd()
        {
            return _stdin.ReadToEnd();
        }

        /// <summary>
        /// Gets or sets the working directory for the command.
        /// </summary>
        public string WorkingDirectory
        {
            get
            {
                return workdir;
            }
            set
            {
                workdir = value;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConsoleContext"/> class. 
        /// </summary>
        /// <param name="stdout">The stream writer for standard output.</param>
        /// <param name="stdin">The stream reader for standard input.</param>
        public ConsoleContext(StreamWriter stdout, StreamReader stdin)
        {
            if (stdout == null || stdin == null)
                throw new ArgumentNullException();
            _stdout = stdout;
            _stdin = stdin;
        }

        /// <summary>
        /// Write a line of text to the console. This method behaves similar to <see cref="string.Format(string, object[])"/>. 
        /// </summary>
        /// <param name="format">The text to write to the console.</param>
        /// <param name="data">The data to inject into the text.</param>
        public void WriteLine(string format, params object[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                format = format.Replace($"{{{i}}}", data[i].ToString());
            }
            WriteLine(format);
        }

        /// <summary>
        /// Sets the foreground and background colors of any following text in the console.
        /// </summary>
        /// <param name="background">The color for the background.</param>
        /// <param name="foreground">The color for the foreground.</param>
        public void SetColors(ConsoleColor background, ConsoleColor foreground)
        {
            _stdout.Write((char)0x1B);
            int b = (int)background;
            int f = (int)foreground;
            _stdout.Write($"{b}{f}");
            _stdout.Write((char)0x1B);
        }

        /// <summary>
        /// Sets whether any following text should be italic.
        /// </summary>
        /// <param name="value">Whether text should be italic.</param>
        public void SetItalic(bool value)
        {
            _stdout.Write((char)0x1B);
            if (value == true)
                _stdout.Write("i");
            else
                _stdout.Write("!i");
            _stdout.Write((char)0x1B);
        }

        /// <summary>
        /// Sets whether any following text should be bolded.
        /// </summary>
        /// <param name="value">Whether text should be bolded.</param>
        public void SetBold(bool value)
        {
            _stdout.Write((char)0x1B);
            if (value == true)
                _stdout.Write("b");
            else
                _stdout.Write("!b");
            _stdout.Write((char)0x1B);
        }

        /// <summary>
        /// Write text to the console.
        /// </summary>
        /// <param name="text">The text to write.</param>
        public void Write(string text)
        {
            _stdout.Write(text);
        }

        /// <summary>
        /// Write a line of text to the console.
        /// </summary>
        /// <param name="text">The text to write.</param>
        public void WriteLine(string text)
        {
            _stdout.WriteLine(text);
        }

        /// <summary>
        /// Read a line of text from standard input.
        /// </summary>
        /// <returns>The text that was read from the stream.</returns>
        public string ReadLine()
        {
            return _stdin.ReadLine();
        }

        /// <summary>
        /// Read a character from standard input.
        /// </summary>
        /// <returns>The character that was read.</returns>
        public char Read()
        {
            return (char)_stdin.Read();
        }

        /// <summary>
        /// Clear the console.
        /// </summary>
        public void Clear()
        {
            _stdout.Write((char)0x1B);
            _stdout.Write("c");
            _stdout.Write((char)0x1B);
        }
    }

    /// <summary>
    /// Represents a Peacenet console color.
    /// </summary>
    public enum ConsoleColor
    {
        /// <summary>
        /// Represents black.
        /// </summary>
        Black,
        /// <summary>
        /// Represents full white.
        /// </summary>
        White,
        /// <summary>
        /// Represents gray.
        /// </summary>
        Gray,
        /// <summary>
        /// Represents full red.
        /// </summary>
        Red,
        /// <summary>
        /// Represents full green.
        /// </summary>
        Green,
        /// <summary>
        /// Represents full blue.
        /// </summary>
        Blue,
        /// <summary>
        /// Represents yellow.
        /// </summary>
        Yellow,
        /// <summary>
        /// Represents orange.
        /// </summary>
        Orange,
        /// <summary>
        /// Represents purple.
        /// </summary>
        Purple,
        /// <summary>
        /// Represents pink.
        /// </summary>
        Pink
    }
}
