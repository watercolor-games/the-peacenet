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
        /// Gets all Shiftorium categories.
        /// </summary>
        /// <param name="onlyAvailable">Should we look in the "available" upgrade list (i.e, what the user can buy right now), or the full upgrade list?</param>
        /// <returns>All Shiftorium categories from the list, in a <see cref="System.String[]"/>. </returns>
        public static string[] GetCategories(bool onlyAvailable = true)
        {
            List<string> cats = new List<string>();
            IEnumerable<ShiftoriumUpgrade> upgrades = GetDefaults();
            if (onlyAvailable)
                upgrades = new List<ShiftoriumUpgrade>(GetAvailable());

            foreach (var upg in upgrades)
            {
                if (!cats.Contains(upg.Category))
                    cats.Add(upg.Category);
            }

            return cats.ToArray();
        }

        /// <summary>
        /// Causes the engine to alert the frontend of a new Shiftorium upgrade install.
        /// </summary>
        public static void InvokeUpgradeInstalled()
        {
            Installed?.Invoke();
        }

        /// <summary>
        /// Gets the category of an upgrade.
        /// </summary>
        /// <param name="id">The upgrade ID to check</param>
        /// <returns>"Other" if the upgrade is not found, else, the upgrade category.</returns>
        public static string GetCategory(string id)
        {
            var upg = GetDefaults().FirstOrDefault(x => x.ID == id);
            if (upg == null)
                return "Other";
            return (upg.Category == null) ? "Other" : upg.Category;
        }

        /// <summary>
        /// Gets all upgrades in a given category.
        /// </summary>
        /// <param name="cat">The category name to search</param>
        /// <returns>The upgrades in the category.</returns>
        public static IEnumerable<ShiftoriumUpgrade> GetAllInCategory(string cat)
        {
            return GetDefaults().Where(x => x.Category == cat);
        }

        /// <summary>
        /// Gets whether or not the user has installed all upgrades in a category.
        /// </summary>
        /// <param name="cat">The category to search.</param>
        /// <returns>Boolean value representing whether the user has installed all upgrades in the category.</returns>
        public static bool IsCategoryEmptied(string cat)
        {
            return GetDefaults().Where(x => x.Category == cat).FirstOrDefault(x => !UpgradeInstalled(x.ID)) == null;
        }

        /// <summary>
        /// Buy an upgrade, deducting the specified amount of Experience.
        /// </summary>
        /// <param name="id">The upgrade ID to buy</param>
        /// <param name="cost">The amount of Experience to deduct</param>
        /// <returns>True if the upgrade was installed successfully, false if the user didn't have enough Experience or the upgrade wasn' found.</returns>
        public static bool Buy(string id, ulong cost)
        {
            if(CashManager.Deduct((long)cost, "upgrademgr") == true)
            {
                if (!SaveSystem.CurrentSave.Upgrades.ContainsKey(id))
                    SaveSystem.CurrentSave.Upgrades.Add(id, false);
                SaveSystem.CurrentSave.Upgrades[id] = true;
                Installed?.Invoke();
                return true;
            }
            return false;
        }

        public static List<ShiftoriumUpgrade> AllUpgrades
        {
            get
            {
                return upgDb;
            }
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

        private static List<ShiftoriumUpgrade> upgDb = null;

        private static ShiftoriumUpgrade[] serverUpgrades = null;

        [ClientMessageHandler("upgrades_db"), AsyncExecution]
        public static void GetDB(string content, string ip)
        {
            serverUpgrades = JsonConvert.DeserializeObject<ShiftoriumUpgrade[]>(content);
        }

        public static void CreateUpgradeDatabase()
        {
            upgDb = new List<ShiftoriumUpgrade>();
            serverUpgrades = null;
            ServerManager.SendMessage("upgrades_getdb", "");
            while (serverUpgrades == null)
                Thread.Sleep(10);
            upgDb.AddRange(serverUpgrades);
            //Now we probe for ShiftoriumUpgradeAttributes for mods.
            foreach (var type in ReflectMan.Types)
            {
                if (type.GetInterfaces().Contains(typeof(IShiftoriumProvider)))
                    if (type.GetCustomAttributes().Any(x => x is ShiftoriumProviderAttribute))
                        upgDb.AddRange((Activator.CreateInstance(type, null) as IShiftoriumProvider).GetDefaults());


                ShiftoriumUpgradeAttribute attrib = type.GetCustomAttributes(false).FirstOrDefault(x => x is ShiftoriumUpgradeAttribute) as ShiftoriumUpgradeAttribute;
                if (attrib != null)
                {
                    if (upgDb.FirstOrDefault(x => x.ID == attrib.Upgrade) != null)
                        throw new ShiftoriumConflictException(attrib.Upgrade);
                    upgDb.Add(new ShiftoriumUpgrade
                    {
                        Name = attrib.Name,
                        Cost = attrib.Cost,
                        Description = attrib.Description,
                        Dependencies = attrib.Dependencies,
                        Category = attrib.Category,
                        Purchasable = attrib.Purchasable
                    });
                }

                foreach (var mth in type.GetMethods())
                {
                    attrib = mth.GetCustomAttributes(false).FirstOrDefault(x => x is ShiftoriumUpgradeAttribute) as ShiftoriumUpgradeAttribute;
                    if (attrib != null)
                    {
                        if (upgDb.FirstOrDefault(x => x.ID == attrib.Upgrade) != null)
                            throw new ShiftoriumConflictException(attrib.Upgrade);
                        upgDb.Add(new ShiftoriumUpgrade
                        {
                            Name = attrib.Name,
                            Cost = attrib.Cost,
                            Description = attrib.Description,
                            Dependencies = attrib.Dependencies,
                            Category = attrib.Category,
                            Purchasable = attrib.Purchasable
                        });

                    }
                }

                foreach (var mth in type.GetFields())
                {
                    attrib = mth.GetCustomAttributes(false).FirstOrDefault(x => x is ShiftoriumUpgradeAttribute) as ShiftoriumUpgradeAttribute;
                    if (attrib != null)
                    {
                        if (upgDb.FirstOrDefault(x => x.ID == attrib.Upgrade) != null)
                            throw new ShiftoriumConflictException(attrib.Upgrade);
                        upgDb.Add(new ShiftoriumUpgrade
                        {
                            Name = attrib.Name,
                            Cost = attrib.Cost,
                            Description = attrib.Description,
                            Dependencies = attrib.Dependencies,
                            Category = attrib.Category,
                            Purchasable = attrib.Purchasable
                        });

                    }
                }

                foreach (var mth in type.GetProperties())
                {
                    attrib = mth.GetCustomAttributes(false).FirstOrDefault(x => x is ShiftoriumUpgradeAttribute) as ShiftoriumUpgradeAttribute;
                    if (attrib != null)
                    {
                        if (upgDb.FirstOrDefault(x => x.ID == attrib.Upgrade) != null)
                            throw new ShiftoriumConflictException(attrib.Upgrade);
                        upgDb.Add(new ShiftoriumUpgrade
                        {
                            Name = attrib.Name,
                            Cost = attrib.Cost,
                            Description = attrib.Description,
                            Dependencies = attrib.Dependencies,
                            Category = attrib.Category,
                            Purchasable = attrib.Purchasable
                        });

                    }
                }

            }

            foreach(var duplicate in upgDb.Where(x=>serverUpgrades.FirstOrDefault(y=>y.ID == x.ID) != null && !serverUpgrades.Contains(x)).ToArray())
            {
                Debug.Print($"[WARN] Client-side upgrade {duplicate.ID} overlaps server-side upgrade with the same ID.");
                upgDb.Remove(duplicate);
            }

            foreach (var item in upgDb)
            {
                if (upgDb.Where(x => x.ID == item.ID).Count() > 1)
                    throw new ShiftoriumConflictException(item.ID);
            }
        }


        /// <summary>
        /// Gets or sets whether the Shiftorium has been initiated.
        /// </summary>
        public static bool IsInitiated { get; private set; }


        /// <summary>
        /// Initiates the Shiftorium.
        /// </summary>
        public static void Init()
        {
            if (IsInitiated == false)
            {
                IsInitiated = true;
                //Let the crash handler deal with this one...
                CreateUpgradeDatabase();
                var dict = upgDb;
                foreach (var itm in dict)
                {
                    if (!SaveSystem.CurrentSave.Upgrades.ContainsKey(itm.ID))
                    {
                        try
                        {
                            SaveSystem.CurrentSave.Upgrades.Add(itm.ID, false);
                        }
                        catch
                        {

                        }
                    }
                }
            }

        }

        /// <summary>
        /// Get the codepoint value for an upgrade.
        /// </summary>
        /// <param name="id">The upgrade ID to search</param>
        /// <returns>The codepoint value.</returns>
        public static ulong GetCPValue(string id)
        {
            foreach (var upg in GetDefaults())
            {
                if (upg.ID == id)
                    return upg.Cost;
            }
            return 0;
        }

        /// <summary>
        /// Gets all available upgrades.
        /// </summary>
        /// <returns></returns>
        public static ShiftoriumUpgrade[] GetAvailable()
        {
            List<ShiftoriumUpgrade> available = new List<ShiftoriumUpgrade>();
            foreach (var defaultupg in GetDefaults().Where(x=>x.Purchasable==true))
            {
                if (!UpgradeInstalled(defaultupg.ID) && DependenciesInstalled(defaultupg))
                    available.Add(defaultupg);
            }
            return available.ToArray();
        }

        private static int? upgrade_installed_count = null;

        [ClientMessageHandler("upgrades_count"), AsyncExecution]
        public static void UpgradeCountHandler(string content, string ip)
        {
            upgrade_installed_count = Convert.ToInt32(content);
        }

        public static int CountUpgrades()
        {
            upgrade_installed_count = null;
            ServerManager.SendMessage("upgrades_getcount", "");
            while (upgrade_installed_count == null)
                Thread.Sleep(10);
            return (int)upgrade_installed_count;
        }

        public static ShiftoriumUpgrade[] GetAllPurchasable()
        {
            return upgDb.Where(x => x.Purchasable == true).ToArray();
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

        private static bool? upgrade_Installed_state = null;
        private static bool? upgrade_loaded_state = null;

        /// <summary>
        /// Handles server replies for the "upgrade_getinstalled" request.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="ip"></param>
        [ClientMessageHandler("upgrades_installed"), AsyncExecution]
        public static void Upgrade_InstalledHandler(string content, string ip)
        {
            upgrade_Installed_state = (content == "1") ? true : false;
        }

        /// <summary>
        /// Handles server replies for the "upgrade_getloaded" request.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="ip"></param>
        [ClientMessageHandler("upgrades_loaded"), AsyncExecution]
        public static void Upgrade_LoadedHandler(string content, string ip)
        {
            upgrade_loaded_state = (content == "1") ? true : false;
        }


        /// <summary>
        /// Determines if an upgrade is installed.
        /// </summary>
        /// <param name="id">The upgrade ID to scan.</param>
        /// <returns>Whether the upgrade is installed.</returns>
        public static bool UpgradeInstalled(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return true;
            upgrade_Installed_state = null;
            ServerManager.SendMessage("upgrades_getinstalled", id);
            while (upgrade_Installed_state == null)
                Thread.Sleep(10);
            return (bool)upgrade_Installed_state;
        }

        public static bool IsLoaded(string upgradeid)
        {
            if (string.IsNullOrWhiteSpace(upgradeid))
                return false;

            if (upgradeid.Contains(";"))
            {
                foreach (var id in upgradeid.Split(';'))
                    if (!IsLoaded(id))
                        return false;
                return true;
            }

            upgrade_loaded_state = null;
            ServerManager.SendMessage("upgrades_getloaded", upgradeid);
            while (upgrade_loaded_state == null)
                Thread.Sleep(10);
            return (bool)upgrade_loaded_state;
        }

        public static void LoadUpgrade(string upgradeid)
        {
            if (IsLoaded(upgradeid))
                throw new UpgradeException(upgradeid, "This upgrade has already been loaded.");
            else
            {
                var upgradedata = upgDb.FirstOrDefault(x => x.ID == upgradeid);
                if(!string.IsNullOrWhiteSpace(upgradedata.Dependencies))
                {
                    if (!IsLoaded(upgradedata.Dependencies))
                        throw new UpgradeException(upgradeid, "This upgrade is missing some dependencies which must be loaded before you can use it.");
                }

                if (SaveSystem.CurrentSave.LoadedUpgrades.Count < SaveSystem.CurrentSave.MaxLoadedUpgrades)
                {
                    SaveSystem.CurrentSave.LoadedUpgrades.Add(upgradeid);
                    InvokeUpgradeInstalled();
                }
                else
                    throw new UpgradeException(upgradeid, "The maximum number of upgrades has already been loaded.");
            }
        }

        public static void UnloadUpgrade(string upgradeid)
        {
            if (!IsLoaded(upgradeid))
                throw new UpgradeException(upgradeid, "This upgrade has not been loaded.");
            else
            {
                var upgrades = upgDb.Where(x => x.Dependencies?.Contains(upgradeid) == true);
                if(upgrades.Count() > 0)
                {
                    foreach(var upg in upgrades)
                    {
                        if (IsLoaded(upg.ID))
                            throw new UpgradeException(upgradeid, "An attempt was made to unload an upgrade which has children that are loaded.");
                    }
                }

                SaveSystem.CurrentSave.LoadedUpgrades.Remove(upgradeid);
                InvokeUpgradeInstalled();
            }
        }


        //LEAVE THIS AS FALSE. The game will set it when the save is loaded.
        public static bool LogOrphanedUpgrades = false;

        /// <summary>
        /// Gets every upgrade inside the frontend and all mods.
        /// </summary>
        /// <returns>Every single found Shiftorium upgrade.</returns>
        public static List<ShiftoriumUpgrade> GetDefaults()
        {
            return upgDb;
        }
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

    public interface IShiftoriumProvider
    {
        /// <summary>
        /// Retrieves all frontend upgrades.
        /// </summary>
        /// <returns></returns>
        List<ShiftoriumUpgrade> GetDefaults();
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

    



    public class ShiftoriumProviderAttribute : Attribute
    {

    }
}
