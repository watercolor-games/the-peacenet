using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects.Hacking;

namespace Plex.Server.Puzzles
{
    [Puzzle("crypt_polybius", 3)]
    public class Crypt_Polybius : IPuzzle
    {
        private string _generatedCipher = "";

        private static readonly string[] messages =
        {
            "goaway",
            "alkaline",
            "thunder",
            "lightning",
            "acid",
            "earth",
            "society",
            "polybius",
            "plexnet",
            "slate",
            "watercolor",
            "secret"
        };

        public Crypt_Polybius()
        {
            var rnd = new Random();
            _generatedCipher = Encrypt(messages[rnd.Next(0, messages.Length)]);
        }

        private static readonly string[] polybius =
        {
            "abcdef",
            "ghijkl",
            "mnopqr",
            "stuvwx",
            "yz1234",
            "567890"
        }; //We could use the traditional 5x5 square, but then we can't fit the entire English language.

        public bool Process(string regular)
        {
            foreach(char c in regular)
            {
                if (polybius.FirstOrDefault(x => x.Contains(c.ToString().ToLower())) == null)
                    return false;
            }
            return true;
        }

        public string Encrypt(string regular)
        {
            string processed = "";
            string cipher = "";
            foreach (char c in regular)
            {
                if (polybius.FirstOrDefault(x => x.Contains(c.ToString().ToLower())) == null)
                    continue; //skip invalid symbols
                processed += c.ToString().ToLower();
            }
            foreach(char c in processed)
            {
                string rowtext = polybius.FirstOrDefault(x => x.Contains(c.ToString()));
                int col = rowtext.IndexOf(c)+1;
                int row = Array.IndexOf(polybius, rowtext)+1;
                cipher += $"{col}{row}";
            }
            return cipher;
        }

        private Puzzle _data = null;

        public Puzzle Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }

        public bool Evaluate(string value)
        {
            if (Process(value) == false)
                return false;
            return Encrypt(value) == this._generatedCipher;
        }

        public string GetHint()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Cipher: " + _generatedCipher);
            foreach(var row in polybius)
            {
                sb.AppendLine();
                foreach (char c in row)
                    sb.Append($" {c}");
            }
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
