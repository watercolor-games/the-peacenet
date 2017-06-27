using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.WinForms
{
    public static class VirtualEnvironments
    {
        private static List<ShiftOSEnvironment> _environments = new List<ShiftOSEnvironment>();

        public static void Create(string sysname, List<ShiftOS.Objects.ClientSave> users, ulong cp, ShiftOS.Objects.ShiftFS.Directory fs)
        {
            var env = new ShiftOSEnvironment
            {
                SystemName = sysname,
                Users = users,
                Codepoints = cp,
                Filesystem = fs
            };
            _environments.Add(env);
        }

        public static void Clear()
        {
            _environments.Clear();
        }

        const string VALID_PASSWORD_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_";


        [RequiresUpgrade("brute")]
        [Command("brute")]
        public static void Brute()
        {
            TerminalBackend.PrefixEnabled = false;
            bool cracked = false;
            var brute = Properties.Resources.brute;
            var str = new System.IO.MemoryStream(brute);
            var reader = new NAudio.Wave.Mp3FileReader(str);
            var _out = new NAudio.Wave.WaveOut();
            _out.Init(reader);
            _out.PlaybackStopped += (o, a) =>
            {
                if (cracked == false)
                {
                    cracked = true;
                    TerminalCommands.Clear();
                    ConsoleEx.Bold = true;
                    ConsoleEx.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" - access denied - ");
                    ConsoleEx.ForegroundColor = ConsoleColor.Gray;
                    ConsoleEx.Bold = false;
                    ConsoleEx.Italic = true;
                    Console.WriteLine("password could not be cracked before connection termination.");
                }
                TerminalBackend.PrefixEnabled = true;
                TerminalBackend.PrintPrompt();
                _out.Dispose();
                reader.Dispose();
                str.Dispose();
            };
            _out.Play();

            var t = new Thread(() =>
            {


                Console.WriteLine("brute - version 1.0");
                Console.WriteLine("Copyright (c) 2018 hacker101. All rights reserved.");
                Console.WriteLine();
                Thread.Sleep(4000);
                Console.WriteLine("Scanning outbound connections...");
                if (string.IsNullOrWhiteSpace(Applications.FileSkimmer.OpenConnection.SystemName))
                {
                    Thread.Sleep(2000);
                    Console.WriteLine(" - no outbound connections to scan, aborting - ");
                    _out.Stop();
                    _out.Dispose();
                    reader.Dispose();
                    str.Dispose();
                }
                else
                {
                    Thread.Sleep(2000);
                    var con = Applications.FileSkimmer.OpenConnection;
                    Console.WriteLine($@"{con.SystemName}
------------------

Active connection: ftp, rts
System name: {con.SystemName}
Users: {con.Users.Count}");
                    Thread.Sleep(500);
                    var user = con.Users.FirstOrDefault(x => x.Permissions == Objects.UserPermissions.Root);
                    if (user == null)
                        Console.WriteLine(" - no users found with root access - this is a shiftos bug - ");
                    else
                    {
                        Console.WriteLine(" - starting bruteforce attack on user: " + user.Username + " - ");
                        
                        char[] pass = new char[user.Password.Length];
                        for (int i = 0; i < pass.Length; i++)
                        {
                            if (cracked == true)
                                break;
                            for(char c = (char)0; c < (char)255; c++)
                            {
                                if (!char.IsLetterOrDigit(c))
                                    continue;
                                pass[i] = c;
                                if (pass[i] == user.Password[i])
                                    break;
                                Console.WriteLine(new string(pass));
                            }
                        }
                        if (cracked == false)
                        {
                            cracked = true;
                            TerminalCommands.Clear();
                            Console.WriteLine(" - credentials cracked. -");
                            Console.WriteLine($@"sysname: {con.SystemName}
user: {user.Username}
password: {user.Password}");
                        }
                    }
                }
            });
            t.Start();
        }

        public static ShiftOSEnvironment Get(string sysname)
        {
            return _environments.FirstOrDefault(x => x.SystemName == sysname);
        }
    }

    public class ShiftOSEnvironment
    {
        public string SystemName { get; set; }
        public ulong Codepoints { get; set; }
        public ShiftOS.Objects.ShiftFS.Directory Filesystem { get; set; }
        public List<ShiftOS.Objects.ClientSave> Users { get; set; }
    }
}
