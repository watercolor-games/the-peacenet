using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatercolorGames.CommandLine
{
    public static class Tokenizer
    {
        public static CommandInstruction GetCommandList(string cmd)
        {
            List<string> commands = new List<string>();
            bool escaping = false;
            bool inQuote = false;
            string current = "";
            bool isFileName = false;
            string fileName = null;
            var fileType = OutputFileType.Append;

            for (int i = 0; i < cmd.Length; i++)
            {
                char c = cmd[i];
                if (c == '\\')
                {
                    if (!(inQuote || isFileName))
                        continue;
                    if (escaping == false)
                    {
                        escaping = true;
                        if (isFileName)
                        {
                            fileName += c;
                        }
                        else
                        {
                            current += c;
                        }
                        continue;
                    }
                    else
                    {
                        escaping = false;
                    }
                }
                else if (c == '"')
                {
                    if (!isFileName)
                    {
                        if (!escaping)
                        {
                            inQuote = !inQuote;
                        }
                    }
                }
                {
                    if (c == '|')
                    {
                        if (!isFileName)
                        {
                            if (!inQuote)
                            {
                                if (string.IsNullOrWhiteSpace(current))
                                    throw new Exception("Attempting to pipe the output of no command.");
                                commands.Add(current);
                                current = "";
                                continue;
                            }
                        }
                    }
                    else if (char.IsWhiteSpace(c))
                    {
                        if (isFileName)
                        {
                            if (!escaping)
                            {
                                if (string.IsNullOrEmpty(fileName))
                                    continue;
                                else
                                    throw new Exception("Whitespace in filename.");

                            }
                        }
                    }
                    else if (c == '>')
                    {
                        if (!isFileName)
                        {
                            isFileName = true;
                            fileType = OutputFileType.Overwrite;
                            continue;
                        }
                        else
                        {
                            if(cmd[i-1] == '>')
                            {
                                if (fileType == OutputFileType.Overwrite)
                                    fileType = OutputFileType.Append;
                                else
                                    throw new Exception("Syntax error: filename expected.");
                                continue;
                            }
                        }
                    }
                }
                if (isFileName)
                {
                    fileName += c;
                }
                else
                {
                    current += c;
                }
                if (escaping == true)
                    escaping = false;
            }
            if (inQuote == true)
            {
                throw new Exception("End of string literal expected.");
            }
            if (escaping == true)
                throw new Exception("End of escape sequence expected.");
            if (!string.IsNullOrEmpty(current))
            {
                commands.Add(current);
                current = "";
            }
            for(int i = 0; i < commands.Count; i++)
            {
                string command = commands[i];
                commands[i] = command.Trim();
            }
            return new CommandInstruction
            {
                Commands = commands.ToArray(),
                OutputFile = fileName,
                OutputFileType = fileType
            };


        }


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

    public class CommandInstruction
    {
        public string[] Commands { get; set; }
        public string OutputFile { get; set; }
        public OutputFileType OutputFileType { get; set; }
    }

    public enum OutputFileType
    {
        Append,
        Overwrite
    }
}
