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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Plex.Engine.SaveSystem;
using System.Diagnostics;
using System.Threading;
using Plex.Objects;
using System.IO;

namespace Plex.Engine
{
    /// <summary>
    /// Backend class for the Shiftorium.
    /// </summary>
    public static class Upgrades
    {
        /// <summary>
        /// Whether or not shiftorium output should be written to the console.
        /// </summary>
        public static bool Silent = false;

        /// <summary>
        /// Causes the engine to alert the frontend of a new Shiftorium upgrade install.
        /// </summary>
        public static void InvokeUpgradeInstalled()
        {
            Installed?.Invoke();
        }

        public static bool Buy(string id, out string error)
        {
            using(var str = new ServerStream(ServerMessageType.UPG_BUY))
            {
                str.Write(id);
                var res = str.Send();
                if(res.Message == (byte)ServerResponseType.REQ_SUCCESS)
                {
                    error = "";
                    return true;
                }
                else if(res.Message == (byte)ServerResponseType.REQ_ERROR)
                {
                    using(var reader = new BinaryReader(ServerManager.GetResponseStream(res)))
                    {
                        error = reader.ReadString();
                    }
                    return false;
                }
            }
            error = "The upgrade could not be purchased for an unknown reason.";
            return false;
        }

        /// <summary>
        /// Determines whether all Shiftorium upgrade attributes for this type have been installed.
        /// </summary>
        /// <param name="type">The type to scan</param>
        /// <returns>Boolean value representing the result of this function.</returns>
        public static bool UpgradeAttributesUnlocked(Type type)
        {
            foreach (var attr in type.GetCustomAttributes(true))
            {
                if (attr is RequiresUpgradeAttribute)
                {
                    var rAttr = attr as RequiresUpgradeAttribute;
                    return IsLoaded(rAttr.Upgrade);
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether all Shiftorium upgrade attributes for this method have been installed.
        /// </summary>
        /// <param name="type">The method to scan</param>
        /// <returns>Boolean value representing the result of this function.</returns>
        public static bool UpgradeAttributesUnlocked(MethodInfo type)
        {
            foreach (var attr in type.GetCustomAttributes(true))
            {
                if (attr is RequiresUpgradeAttribute)
                {
                    var rAttr = attr as RequiresUpgradeAttribute;
                    return IsLoaded(rAttr.Upgrade);
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether all Shiftorium upgrade attributes for this property have been installed.
        /// </summary>
        /// <param name="type">The property to scan</param>
        /// <returns>Boolean value representing the result of this function.</returns>
        public static bool UpgradeAttributesUnlocked(PropertyInfo type)
        {
            foreach (var attr in type.GetCustomAttributes(true))
            {
                if (attr is RequiresUpgradeAttribute)
                {
                    var rAttr = attr as RequiresUpgradeAttribute;
                    return IsLoaded(rAttr.Upgrade);
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether all Shiftorium upgrade attributes for this field have been installed.
        /// </summary>
        /// <param name="type">The field to scan</param>
        /// <returns>Boolean value representing the result of this function.</returns>
        public static bool UpgradeAttributesUnlocked(FieldInfo type)
        {
            foreach (var attr in type.GetCustomAttributes(true))
            {
                if (attr is RequiresUpgradeAttribute)
                {
                    var rAttr = attr as RequiresUpgradeAttribute;
                    return IsLoaded(rAttr.Upgrade);
                }
            }

            return true;
        }
        
        public static ShiftoriumUpgrade GetUpgradeInfo(string id)
        {
            using(var str = new ServerStream(ServerMessageType.UPG_GETINFO))
            {
                str.Write(id);
                var result = str.Send();
                if(result.Message == (byte)ServerResponseType.REQ_SUCCESS)
                {
                    using(var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        return JsonConvert.DeserializeObject<ShiftoriumUpgrade>(reader.ReadString());
                    }
                }
                else if(result.Message == (byte)ServerResponseType.REQ_ERROR)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        throw new UpgradeException(id, reader.ReadString());
                    }

                }
            }
            throw new UpgradeException(id, "A generic error has occurred.");

        }

        public static string[] GetAvailableIDs()
        {
            List<string> upgs = new List<string>();
            using(var str = new ServerStream(ServerMessageType.UPG_GETUPGRADES))
            {
                var result = str.Send();
                if(result.Message == (byte)ServerResponseType.REQ_SUCCESS)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        int len = reader.ReadInt32();
                        for(int i = 0; i < len; i++)
                        {
                            upgs.Add(reader.ReadString());
                        }
                    }
                }
            }
            return upgs.ToArray();
        }


        public static int CountUpgrades()
        {
            using(var sstr = new ServerStream(ServerMessageType.UPG_GETCOUNT))
            {
                var result = sstr.Send();
                if(result.Message == 0x00)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                        return reader.ReadInt32();
                }
            }
            return 0;
        }

        /// <summary>
        /// Determines whether all dependencies of a given upgrade have been installed.
        /// </summary>
        /// <param name="upg">The upgrade to scan</param>
        /// <returns>Boolean representing the result of this function.</returns>
        public static bool DependenciesInstalled(ShiftoriumUpgrade upg)
        {
            if (string.IsNullOrEmpty(upg.Dependencies))
            {
                return true;//root upgrade, no parents
            }
            else if (upg.Dependencies.Contains(";"))
            {
                string[] dependencies = upg.Dependencies.Split(';');
                foreach (var dependency in dependencies)
                {
                    if (!UpgradeInstalled(dependency))
                        return false;
                }
                return true;
            }
            else
            {
                return UpgradeInstalled(upg.Dependencies);
            }
        }

        /// <summary>
        /// Fired when an upgrade is installed.
        /// </summary>
        public static event EmptyEventHandler Installed;

        /// <summary>
        /// Determines if an upgrade is installed.
        /// </summary>
        /// <param name="id">The upgrade ID to scan.</param>
        /// <returns>Whether the upgrade is installed.</returns>
        public static bool UpgradeInstalled(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return true;
            if (id.Contains(":"))
            {
                foreach (var sid in id.Split(';'))
                    if (UpgradeInstalled(sid) == false)
                        return false;
                return true;
            }
            using (var w = new ServerStream(ServerMessageType.UPG_ISINSTALLED))
            {
                w.Write(id);
                var result = w.Send();
                if (result.Message == 0x00)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        return reader.ReadBoolean();
                    }
                }
                return false;
            }
        }

        public static bool IsLoaded(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return true;
            if (id.Contains(":"))
            {
                foreach (var sid in id.Split(';'))
                    if (IsLoaded(sid) == false)
                        return false;
                return true;
            }
            using (var w = new ServerStream(ServerMessageType.UPG_ISLOADED))
            {
                w.Write(id);
                var result = w.Send();
                if (result.Message == 0x00)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        return reader.ReadBoolean();
                    }
                }
                return false;
            }
        }

        public static void LoadUpgrade(string upgradeid)
        {
            using(var sstr = new ServerStream(ServerMessageType.UPG_LOAD))
            {
                sstr.Write(upgradeid);
                var result = sstr.Send();
                if(result.Message == 0x00)
                {
                    using(var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        byte r = (byte)reader.ReadInt32();
                        HandleUpgradeResult((UpgradeResult)r, upgradeid);

                    }
                }
            }
        }

        public static void HandleUpgradeResult(UpgradeResult res, string id)
        {
            switch (res)
            {
                case UpgradeResult.ALREADY_LOADED:
                    throw new UpgradeException(id, "The requested upgrade is already loaded.");
                case UpgradeResult.ALREADY_UNLOADED:
                    throw new UpgradeException(id, "The requested upgrade is already unloaded.");
                case UpgradeResult.LOADED:
                case UpgradeResult.UNLOADED:
                    Upgrades.InvokeUpgradeInstalled();
                    break;
                case UpgradeResult.MISSING_UPGRADE:
                    throw new UpgradeException(id, "You do not own this upgrade.");
                case UpgradeResult.NO_SLOTS:
                    throw new UpgradeException(id, "You do not have enough slots to load this upgrade.");
                case UpgradeResult.UNCAUGHT_ERROR:
                    throw new UpgradeException(id, "An unknown error occurred servicing the upgrade request.");

            }
        }

        public static void UnloadUpgrade(string upgradeid)
        {
            using (var sstr = new ServerStream(ServerMessageType.UPG_UNLOAD))
            {
                sstr.Write(upgradeid);
                var result = sstr.Send();
                if (result.Message == 0x00)
                {
                    using (var reader = new BinaryReader(ServerManager.GetResponseStream(result)))
                    {
                        byte r = (byte)reader.ReadInt32();
                        HandleUpgradeResult((UpgradeResult)r, upgradeid);

                    }
                }
            }
        }


        //LEAVE THIS AS FALSE. The game will set it when the save is loaded.
        public static bool LogOrphanedUpgrades = false;
    }

    public class UpgradeException : Exception
    {
        public UpgradeException(string upgid, string message) : base(upgid + ": " + message)
        {
            UpgradeID = upgid;
            ErrorMessage = message;
        }

        public string UpgradeID { get; private set; }
        public string ErrorMessage { get; private set; }
    }

    public class ShiftoriumUpgradeLookupException : Exception
    {
        public ShiftoriumUpgradeLookupException(string id) : base("A shiftorium upgrade of ID \"" + id + "\" was not found in the system.")
        {
            ID = id;

            Debug.WriteLine("UpgradeNotFound: " + id);

        }

        public string ID { get; private set; }
    }
}
