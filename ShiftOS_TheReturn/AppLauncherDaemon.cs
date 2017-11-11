/*
 * Project: Plex
 * 
 * Copyright (c) 2017 Watercolor Games. All rights reserved. For internal use only.
 * 






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













































using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Plex.Objects;
using Plex.Objects.ShiftFS;

namespace Plex.Engine
{
    /// <summary>
    /// Provides functionality for pulling data about the App Launcher.
    /// </summary>
    public static class AppLauncherDaemon
    {
        /// <summary>
        /// Extension method that allows you to determine if a list of <see cref="AssemblyName"/>s contains a given name. 
        /// </summary>
        /// <param name="asms">The list of assembly names</param>
        /// <param name="name">The name to look for.</param>
        /// <returns>Whether or not the name was found in the list.</returns>
        public static bool Contains(this AssemblyName[] asms, string name)
        {
            foreach(var asm in asms)
            {
                if (asm.FullName.Contains(name))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Pulls a list of all available App Launcher items.
        /// </summary>
        /// <returns>A <see cref="List{LauncherItem}"/> containing all available App Launcher items.</returns>
        public static List<LauncherItem> Available()
        {
            List<LauncherItem> win = new List<LauncherItem>();
            
            foreach (var type in ReflectMan.Types.Where(t => t.GetInterfaces().Contains(typeof(IPlexWindow)) && Upgrades.UpgradeAttributesUnlocked(t)))
            {
                foreach (var attr in type.GetCustomAttributes(false))
                        if (attr is LauncherAttribute)
                        {
                            var launch = attr as LauncherAttribute;
                            if (launch.UpgradeInstalled)
                            {
                                var data = new LauncherItem { DisplayData = launch, LaunchType = type };
                                data.DisplayData.Category = (data.DisplayData.Category);
                                data.DisplayData.Name = (data.DisplayData.Name);
                                win.Add(data);
                            }
                        }
            }

            foreach(var file in FSUtils.GetFiles(Paths.GetPath("launcheritems")))
            {
                if (file.EndsWith(".al"))
                {
                    var item = JsonConvert.DeserializeObject<LuaLauncherItem>(FSUtils.ReadAllText(file));
                    win.Add(item);
                }
            }
            return win;
        }

    }

    /// <summary>
    /// Provides a data object for app launcher items
    /// </summary>
    public class LauncherItem
    {
        /// <summary>
        /// Display data including icons, names, and the category of the item.
        /// </summary>
        public LauncherAttribute DisplayData { get; set; }
        /// <summary>
        /// A .NET <see cref="Type"/> that is associated with this item. 
        /// </summary>
        public Type LaunchType { get; internal set; }

    }

    /// <summary>
    /// Provides the ability to run Lua scripts from the App Launcher.
    /// </summary>
    public class LuaLauncherItem : LauncherItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="LuaLauncherItem"/>. 
        /// </summary>
        /// <param name="file">A script file to run when the item is activated.</param>
        public LuaLauncherItem(string file)
        {
            LaunchPath = file;
        }

        /// <summary>
        /// Gets or sets the launch path of this App Launcher item.
        /// </summary>
        public string LaunchPath { get; private set; }
    }
}
