using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatercolorGames.CommandLine
{
    public static class Tokenizer
    {
        public static string[] TokenizeString(string cmd)
        {
            List<string> tokens = new List<string>();
            string current = "";
            bool escaping = false;
            bool inQuote = false;

            for (int i = 0; i < cmd.Length; i++)
            {
                char c = cmd[i];
                //Escape sequences
                if (c == '\\')
                {
                    if (escaping == false)
                        escaping = true;
                    else
                    {
                        escaping = false;
                        current += c;
                    }
                    continue;
                }
                if (escaping == true)
                {
                    switch (c)
                    {
                        case ' ':
                            current += " ";
                            break;
                        case 'n':
                            current += '\n';
                            break;
                        case 'r':
                            current += '\r';
                            break;
                        case 't':
                            current += '\t';
                            break;
                        case '"':
                            current += "\"";
                            break;
                        default:
                            throw new Exception("Invalid escape sequence: \\" + c);
                    }
                    escaping = false;
                    continue;
                }
                if (c == ' ')
                {
                    if (inQuote)
                    {
                        current += ' ';
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(current))
                        {
                            tokens.Add(current);
                            current = "";
                        }
                    }
                    continue;
                }
                if (c == '"')
                {
                    inQuote = !inQuote;
                    if (inQuote == false)
                    {
                        if (i + 1 < cmd.Length)
                            if (cmd[i + 1] == '"')
                                throw new Exception("String splice detected. Use '\"' if you need a literal double-quote in a string.");
                    }
                    continue;
                }
                current += c;
            }
            if (inQuote == true)
            {
                throw new Exception("End of string literal expected.");
            }
            if (escaping == true)
                throw new Exception("End of escape sequence expected.");
            if (!string.IsNullOrEmpty(current))
            {
                tokens.Add(current);
                current = "";
            }
            return tokens.ToArray();
        }


    }
}
