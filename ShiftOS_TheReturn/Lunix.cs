/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    /// <summary>
    /// Special functions for when we're running under Wine. (Pure Mono is not detected)
    /// </summary>
    public static class Lunix
    {
        /// <summary>
        /// Determines if the game is running under Wine or not.
        /// </summary>
        public static bool InWine
        {
            get
            {
                var WineKey = Registry.CurrentUser.OpenSubKey(@"Software\Wine");
                return WineKey != null;
            }
        }
        /// <summary>
        /// Escape Wine and run shell commands.
        /// </summary>
        public static class Bash
        {
            /// <summary>
            /// Run a command in Bash on the host OS.
            /// </summary>
            /// <param name="command">The command to run.</param>
            /// <returns>Everything put on STDOUT by the command.</returns>
            public static string RunCommand(string command)
            {
                string outfile = Path.GetRandomFileName();
                string outpath = "Z:\\tmp\\" + outfile;
                string copypath = Path.GetTempPath() + Path.GetRandomFileName();
                var p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "cmd";
                p.StartInfo.Arguments = "/C start Z:\\bin\\bash -c \"" + command.Replace("\"", "\\\"") + " > /tmp/" + outfile + "\"";
                p.Start();
                p.WaitForExit();
                p.StartInfo.Arguments = "/C move " + outpath + " " + copypath; // Move the file somewhere .NET can see it.
                p.Start();
                p.WaitForExit();
                string output = File.ReadAllText(copypath);
                File.Delete(copypath); // Get rid of that.
                return output;
            }
        }
        /// <summary>
        /// Basic, unreliable implementation of DriveInfo using df and lsblk.
        /// </summary>
        public class DFDriveInfo
        {
            private int dfrow, lsblkrow;
            public string Name;
            private static string[] rundf(string output)
            {
                return Bash.RunCommand("df --output='" + output + "'").Split('\n');
            }

            private string ourdf(string output)
            {
                return rundf(output)[dfrow];
            }

            private static string[] runlsblk(string output)
            {
                return Bash.RunCommand("lsblk -b --output='" + output + "'").Split('\n');
            }

            private string ourlsblk(string output)
            {
                return runlsblk(output)[lsblkrow];
            }

            public DFDriveInfo(string driveName)
            {
                dfrow = Array.IndexOf(rundf("target"), driveName);
                lsblkrow = Array.IndexOf(runlsblk("MOUNTPOINT"), driveName);
                Name = driveName;
            }
            public long AvailableFreeSpace
            {
                get
                {
                    return Convert.ToInt64(ourdf("avail"));
                }
            }

            public string DriveFormat
            {
                get
                {
                    return ourdf("fstype");
                }
            }

            public DriveType DriveType
            {
                get
                {
                    return DriveType.Unknown;
                }
            }

            public bool IsReady
            {
                get
                {
                    return true;
                }
            }
            
            public DirectoryInfo RootDirectory
            {
                get
                {
                    return new DirectoryInfo("Z:" + Name.Replace('/', '\\'));
                }
            }

            public long TotalFreeSpace
            {
                get
                {
                    return AvailableFreeSpace;
                }
            }

            public long TotalSize
            {
                get
                {
                    return Convert.ToInt64(ourlsblk("SIZE"));
                }
            }

            public string VolumeLabel
            {
                get
                {
                    return ourlsblk("LABEL");
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public static DFDriveInfo[] GetDrives()
            {
                string[] dnames = rundf("target");
                var output = new DFDriveInfo[dnames.Length - 1];
                for (int i = 1; i < dnames.Length; i++)
                    output[i - 1] = new DFDriveInfo(dnames[i]);
                return output;
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
