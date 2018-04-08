using Plex.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Filesystem.ClientMounts
{
    public class BinDirectory : IClientMount
    {
        public string Path => "/bin";

        [Dependency]
        private TerminalManager _terminalManager = null;

        private Random _random = new Random();

        public byte[] GetFileContents(string filename)
        {
            if (!GetFiles().Contains(filename))
                throw new IOException("File not found.");

            byte[] nameBytes = Encoding.UTF32.GetBytes(filename);
            byte[] real = new byte[nameBytes.Length * 32];

            //We're generating random binary based off the program's name.

            //We'll use a random number generator to grab a random byte from the original array.
            //Then we use it to choose a number between 0 and 4. This will be the number of bits we shift the byte.
            //Every second number will be shifted left, others shifted right.
            for(int i = 0; i < real.Length; i++)
            {
                var b = nameBytes[_random.Next(nameBytes.Length)];
                var s = _random.Next(4);
                if (i % 2 == 0)
                    b = (byte)(b << s);
                else
                    b = (byte)(b >> s);
                real[i] = b;
            }

            return real;
        }

        public string[] GetFiles()
        {
            return _terminalManager.GetCommandList().Select(x => x.Name).ToArray();
        }
    }
}
