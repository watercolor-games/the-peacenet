using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WatercolorGames.CommandLine;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace tercolorGames.CommandLine.Testing
{
    [TestClass]
    public class TokenizerTests
    {
        [TestMethod]
        public void SimpleCommandTest()
        {
            string input = "echo Hello.";
            string expected = new string[]
            {
                "echo",
                "Hello."
            }.ToJSON();

            string actual = Tokenizer.TokenizeString(input).ToJSON();

            Assert.AreEqual(expected, actual, "Simple word parsing does not work correctly.");
        }

        [TestMethod]
        public void SimpleStringTest()
        {
            string input = "echo \"Hello world.\"";
            string expected = new string[]
            {
                "echo",
                "Hello world."
            }.ToJSON();

            string actual = Tokenizer.TokenizeString(input).ToJSON();

            Assert.AreEqual(expected, actual, "Strings without escape sequences don't work.");

        }

        [TestMethod]
        public void ParseErrorTest()
        {
            string input1 = "echo \\";
            string input2 = "echo \\a";
            string input3 = "echo \"unfinished string";
            string input4 = "echo \"string\"\"splice\"";
            try
            {
                Tokenizer.TokenizeString(input1);
                Assert.Fail("A broken escape sequence went through as valid.");
                return;
            }
            catch { }

            try
            {
                Tokenizer.TokenizeString(input2);
                Assert.Fail("An unsupported escape sequence went through as valid.");
                return;
            }
            catch { }

            try
            {
                Tokenizer.TokenizeString(input3);
                Assert.Fail("An unfinished string went through as valid.");
                return;
            }
            catch { }

            try
            {
                Tokenizer.TokenizeString(input4);
                Assert.Fail("A string splice went through as valid.");
                return;
            }
            catch { }

        }

        [TestMethod]
        public void EscapeSequenceTest()
        {
            Dictionary<string, string> escapes = new Dictionary<string, string>
            {
                { "\\ ", "[\" \"]" },
                {"\\n", new string[] {"\n"}.ToJSON() },
                {"\\r", new string[] {"\r"}.ToJSON() },
                {"\\t", new string[] {"\t"}.ToJSON() },
                {"\\\\", new string[] {"\\"}.ToJSON() },
                {"\\\"", new string[] {"\""}.ToJSON() }
            };

            foreach(var kv in escapes)
            {
                string input = kv.Key;
                string expected = kv.Value;

                string actual = Tokenizer.TokenizeString(input).ToJSON();

                Assert.AreEqual(expected, actual, $"Escape sequance \"{input}\" failed.");
            }
        }

        [TestMethod]
        public void RealWorldTest1()
        {
            string input = "nmap alfa.kilo-s4 -sV -p 80";
            string expected = new string[]
            {
                "nmap",
                "alfa.kilo-s4",
                "-sV",
                "-p",
                "80"
            }.ToJSON();

            string actual = Tokenizer.TokenizeString(input).ToJSON();

            Assert.AreEqual(expected, actual, "NMap real world test failed.");
        }

        [TestMethod]
        public void CopyCommandTest()
        {
            string input = "cp 0:/bin/bash 0:/bin/zsh";
            string expected = new string[]
            {
                "cp",
                "0:/bin/bash",
                "0:/bin/zsh"
            }.ToJSON();

            string actual = Tokenizer.TokenizeString(input).ToJSON();

            Assert.AreEqual(expected, actual, "Multiarg + file path args not working.");
        }

        [TestMethod]
        public void SeparatorAfterSpaceEscape()
        {
            string input = "echo Space\\  ";
            string expected = new string[]
            {
                "echo",
                "Space "
            }.ToJSON();

            string actual = Tokenizer.TokenizeString(input).ToJSON();

            Assert.AreEqual(expected, actual, "Having a real space directly after an escaped space does not work.");
        }

        [TestMethod]
        public void MultistringTest()
        {
            string input = "echo \"I love\" \"multiple strings\"";
            string expected = new string[]
            {
                "echo",
                "I love",
                "multiple strings"
            }.ToJSON();

            string actual = Tokenizer.TokenizeString(input).ToJSON();

            Assert.AreEqual(expected, actual, "Multiple strings in a row do not work.");
        }

        [TestMethod]
        public void BulkRealWorldTest()
        {
            /*
youtube-dl --extract-audio --audio-format 3 https://www.youtube.com/watch\?v\=OFSC4pgN3Mw
scp -P 2200 reagan@wlsctm.xyz:/tmp/disk.zip ./
git commit -m "Fixed bug with module path resolution"
git add ion.id ion_frontend.id prettyconsole.id Makefile
git commit -m "Refactored ion.id, added prettyconsole.id"
make ; sudo make install
ssh viper@cssserv.int0x10.com -p 2200
cd ~/.config/hexchat
sudo mount -o=ro /dev/sda1 Windows
cat /tmp/*.tmp
              */

            var cmds = new Dictionary<string, string>
            {
                {
                    "youtube-dl --extract-audio --audio-format 3 https://www.youtube.com/watch?v=OFSC4pgN3Mw",
                    new string[] {
                        "youtube-dl",
                        "--extract-audio",
                        "--audio-format",
                        "3",
                        "https://www.youtube.com/watch?v=OFSC4pgN3Mw"
                    }.ToJSON()
                },
                {
                    "scp -P 2200 reagan@wlsctm.xyz:/tmp/disk.zip ./",
                    new string[] {
                        "scp",
                        "-P",
                        "2200",
                        "reagan@wlsctm.xyz:/tmp/disk.zip",
                        "./"
                    }.ToJSON()
                },
                {
                    "git commit -m \"Fixed bug with module path resolution\"",
                    new string[] {
                        "git",
                        "commit",
                        "-m",
                        "Fixed bug with module path resolution"
                    }.ToJSON()
                },
                {
                    "git add ion.id ion_frontend.id prettyconsole.id Makefile",
                    new string[] {
                        "git",
                        "add",
                        "ion.id",
                        "ion_frontend.id",
                        "prettyconsole.id",
                        "Makefile"
                    }.ToJSON()
                },
                {
                    "git commit -m \"Refactored ion.id, added prettyconsole.id\"",
                    new string[] {
                        "git",
                        "commit",
                        "-m",
                        "Refactored ion.id, added prettyconsole.id"
                    }.ToJSON()
                },
                {
                    "make ; sudo make install",
                    new string[] {
                        "make",
                        ";",
                        "sudo",
                        "make",
                        "install"

                    }.ToJSON()
                },
                {
                    "ssh viper@cssserv.int0x10.com -p 2200",
                    new string[]
                    {
                        "ssh",
                        "viper@cssserv.int0x10.com",
                        "-p",
                        "2200"
                    }.ToJSON()
                },
                {
                    "cd ~/.config/hexchat",
                    new string[] {
                        "cd",
                        "~/.config/hexchat"
                    }.ToJSON()
                },
                {
                    "sudo mount -o=ro /dev/sda1 Windows",
                    new string[] {
                        "sudo",
                        "mount",
                        "-o=ro",
                        "/dev/sda1",
                        "Windows"
                    }.ToJSON()
                },
                {
                    "cat /tmp/*.tmp",
                    new string[] {
                        "cat",
                        "/tmp/*.tmp"
                    }.ToJSON()
                }

            };

            foreach(var kv in cmds)
            {
                Console.WriteLine("Real-world test: {0}", kv.Key);
                string input = kv.Key;
                string expected = kv.Value;
                string actual = Tokenizer.TokenizeString(input).ToJSON();
                Assert.AreEqual(expected, actual, "Command \"" + input + "\" was not parsed properly.");
            }

        }
    }

    public static class Serializer
    {
        public static string ToJSON(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
