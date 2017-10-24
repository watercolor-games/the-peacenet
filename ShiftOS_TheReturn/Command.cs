/* * Project: Plex *  * Copyright (c) 2017 Watercolor Games. All rights reserved. For internal use only. *  *  * The above copyright notice and this permission notice shall be included in all * copies or substantial portions of the Software. *  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE * SOFTWARE. */













































using System;using System.Collections.Generic;using System.IO;
using System.Linq;using System.Text;using System.Threading.Tasks;using Plex.Objects;

namespace Plex.Engine{    /// <summary>    /// Denotes that the following terminal command or namespace must only be used in an elevated environment.    /// </summary>    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]    public class KernelModeAttribute : Attribute    {    }    /// <summary>    /// Denotes that this command requires a specified argument to be in its argument dictionary.    /// </summary>    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]    public class RequiresArgument : Attribute    {        /// <summary>        /// The argument name        /// </summary>        public string argument;        /// <summary>        /// Creates a new instance of the <see cref="RequiresArgument"/> attribute         /// </summary>        /// <param name="argument">The argument name associated with this attribute</param>        public RequiresArgument(string argument)        {            this.argument = argument;        }        public override object TypeId        {            get            {                return this;            }        }    }    /// <summary>    /// Prevents a command from being run in a remote session.    /// </summary>    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]    public class RemoteLockAttribute : Attribute    {    }    public static class EngineCommands
    {
        [ClientMessageHandler("cmd_help"), AsyncExecution]
        public static void CMDHelp(string content, string ip)
        {
            _cmdhelpresult = content;
        }

        private static string _cmdhelpresult = null;

        [MetaCommand]
        [Command("help", "", "{DESC_COMMANDS}")]
        public static bool Commands()
        {
            Dictionary<string, string> cmds = new Dictionary<string, string>();
            using (var w = new ServerStream(ServerMessageType.TRM_GETCMDS))
            {
                var result = w.Send();
                if (result.Message == 0x00)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        int count = reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            cmds.Add(reader.ReadString(), reader.ReadString());
                        }
                    }
                }
            }
            foreach (var n in TerminalBackend.Commands.Where(x => !(x is TerminalBackend.WinOpenCommand) && Upgrades.UpgradeInstalled(x.Dependencies) && x.CommandInfo.hide == false && x.MatchShell(TerminalBackend.RawShellOverride) == true).OrderBy(x => x.CommandInfo.name))
            {
                if(!cmds.ContainsKey(n.CommandInfo.name))
                    cmds.Add(n.CommandInfo.name, n.CommandInfo.description);
            }

                var sb = new StringBuilder();
            sb.AppendLine("{GEN_COMMANDS}");
            sb.AppendLine("=================");
            sb.AppendLine();
            //print all unique namespaces.
            foreach (var cmd in cmds.OrderBy(x=>x.Key))
            {
                sb.Append(" - " + cmd.Key);
                if (!string.IsNullOrWhiteSpace(cmd.Value))
                        sb.Append(": " + cmd.Value);
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());

            return true;
        }

    }}