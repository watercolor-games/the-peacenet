using Microsoft.Xna.Framework.Content;
using Plex.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using System.IO;

namespace Peacenet
{
    /// <summary>
    /// A C# implementation of Linux's famous Cowsay command.
    /// </summary>
    public class cowsay : ITerminalCommand
    {
        /// <inheritdoc/>
        public string Description
        {
            get
            {
                return "Make a cow say something.";
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "cowsay";
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Usages
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Turn text into a talking cow.
        /// </summary>
        /// <param name="text">A string representing what the cow will say</param>
        /// <param name="cow">The cow ASCII art.</param>
        /// <returns>The talking cow.</returns>
        public string MakeSpeech(string text, string cow)
        {
            string[] cowlines = cow.Split('\n');
            StringBuilder sb = new StringBuilder();
            int length = Math.Min(text.Length, 30);
            sb.AppendLine(" _" + "_".Repeat(length) + "_ ");
            string[] lines = SplitLines(text, length);
            for(int i = 0; i < lines.Length; i++)
            {
                char begin = '|';
                char end = '|';
                if(i == 0)
                {
                    if(lines.Length == 1)
                    {
                        begin = '<';
                        end = '>';
                    }
                    else
                    {
                        begin = '/';
                        end = '\\';
                    }
                }
                else if(i == lines.Length - 1)
                {
                    begin = '\\';
                    end = '/';
                }
                string line = lines[i];
                int lineLength = line.Length;
                int pad = length - lineLength;
                sb.AppendLine(begin + " " + line + " ".Repeat(pad) + " " + end);
            }

            sb.AppendLine(" -" + "-".Repeat(length) + "- ");
            foreach(var cowline in cowlines)
            {
                sb.AppendLine(" ".Repeat(length+4) + cowline);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Split text into lines of a specified length.
        /// </summary>
        /// <param name="text">The text to split.</param>
        /// <param name="wrap">The preferred length of each line, in characters.</param>
        /// <returns>The split text.</returns>
        public string[] SplitLines(string text, int wrap)
        {
            List<string> lines = new List<string>();
            string current = "";
            for(int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                switch (c)
                {
                    case '\0':
                    case '\b':
                    case '\t':
                    case '\v':
                    case '\r':
                        continue;
                    case '\n':
                        lines.Add(current);
                        current = "";
                        continue;
                    default:
                        current += c;
                        break;
                }
                if(current.Length >= wrap)
                {
                    lines.Add(current);
                    current = "";
                }
                
            }
            if (!string.IsNullOrWhiteSpace(current))
            {
                lines.Add(current);
                current = "";
            }
            return lines.ToArray();
        }

        /// <inheritdoc/>
        public void Run(ConsoleContext console, Dictionary<string, object> arguments)
        {
            string basedir = AppDomain.CurrentDomain.BaseDirectory;
            string text = console.ReadToEnd();
            string cow = File.ReadAllText(Path.Combine(basedir, "Content", "cowsaycow.txt"));
            console.WriteLine(MakeSpeech(text, cow));
        }
    }

    /// <summary>
    /// Simple string extensions for command-line UIs.
    /// </summary>
    public static class StringExtensionsForCowsay
    {
        /// <summary>
        /// Repeats the specified <paramref name="input"/> a specified amount of times. 
        /// </summary>
        /// <param name="input">The input string to repeat.</param>
        /// <param name="length">The amount of times the string should be repeated.</param>
        /// <returns>The resulting string of the repeat operation.</returns>
        public static string Repeat(this string input, int length)
        {
            string output = "";
            for(int i = 0; i < length; i++)
            {
                output += input;
            }
            return output;
        }
    }
}
