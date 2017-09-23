/* * Project: Plex *  * Copyright (c) 2017 Watercolor Games. All rights reserved. For internal use only. *  *  * The above copyright notice and this permission notice shall be included in all * copies or substantial portions of the Software. *  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE * SOFTWARE. */













































using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;using Plex.Objects;

namespace Plex.Engine{    /// <summary>    /// Denotes that the following terminal command or namespace must only be used in an elevated environment.    /// </summary>    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]    public class KernelModeAttribute : Attribute    {    }    /// <summary>    /// Denotes that this command requires a specified argument to be in its argument dictionary.    /// </summary>    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]    public class RequiresArgument : Attribute    {        /// <summary>        /// The argument name        /// </summary>        public string argument;        /// <summary>        /// Creates a new instance of the <see cref="RequiresArgument"/> attribute         /// </summary>        /// <param name="argument">The argument name associated with this attribute</param>        public RequiresArgument(string argument)        {            this.argument = argument;        }        public override object TypeId        {            get            {                return this;            }        }    }    /// <summary>    /// Prevents a command from being run in a remote session.    /// </summary>    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]    public class RemoteLockAttribute : Attribute    {    }    public static class EngineCommands
    {
        [MetaCommand]
        [Command("help", "", "{DESC_COMMANDS}")]
        public static bool Commands()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{GEN_COMMANDS}");
            sb.AppendLine("=================");
            sb.AppendLine();
            //print all unique namespaces.
            foreach (var n in TerminalBackend.Commands.Where(x => !(x is TerminalBackend.WinOpenCommand) && Upgrades.UpgradeInstalled(x.Dependencies) && x.CommandInfo.hide == false && x.MatchShell(TerminalBackend.RawShellOverride) == true).OrderBy(x => x.CommandInfo.name))
            {
                sb.Append(" - " + n.CommandInfo.name);
                if (!string.IsNullOrWhiteSpace(n.CommandInfo.description))
                    if (Upgrades.UpgradeInstalled("help_description"))
                        sb.Append(" - " + n.CommandInfo.description);
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());

            return true;
        }

    }}