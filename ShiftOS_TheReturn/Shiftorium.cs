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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static ShiftOS.Engine.SaveSystem;
using System.Diagnostics;

namespace ShiftOS.Engine
{
    /// <summary>
    /// Backend class for the Shiftorium.
    /// </summary>
    public static class Shiftorium
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
            return GetDefaults().Where(x => x.Category == cat).FirstOrDefault(x => x.Installed == false) == null;
        }

        /// <summary>
        /// Buy an upgrade, deducting the specified amount of Codepoints.
        /// </summary>
        /// <param name="id">The upgrade ID to buy</param>
        /// <param name="cost">The amount of Codepoints to deduct</param>
        /// <returns>True if the upgrade was installed successfully, false if the user didn't have enough Codepoints or the upgrade wasn' found.</returns>
        public static bool Buy(string id, ulong cost)
        {
            if (SaveSystem.CurrentSave.Codepoints >= cost)
            {
                SaveSystem.CurrentSave.Upgrades[id] = true;
                TerminalBackend.InvokeCommand("sos.save");
                SaveSystem.TransferCodepointsToVoid(cost);
                Installed?.Invoke();
                Desktop.ResetPanelButtons();
                Desktop.PopulateAppLauncher();
                return true;
            }
            else
            {
                if (!Silent)
                    Console.WriteLine($"{{SHIFTORIUM_NOTENOUGHCP}}: {cost} > {SaveSystem.CurrentSave.Codepoints}");
                return false;
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
                    return rAttr.Installed;
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
                    return rAttr.Installed;
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
                    return rAttr.Installed;
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
                    return rAttr.Installed;
                }
            }

            return true;
        }

        private static List<ShiftoriumUpgrade> upgDb = null;

        public static void CreateUpgradeDatabase()
        {
            upgDb = new List<ShiftoriumUpgrade>();
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
                                    Id = attrib.Upgrade,
                                    Name = attrib.Name,
                                    Cost = attrib.Cost,
                                    Description = attrib.Description,
                                    Dependencies = attrib.Dependencies,
                                    Category = attrib.Category
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
                                        Id = attrib.Upgrade,
                                        Name = attrib.Name,
                                        Cost = attrib.Cost,
                                        Description = attrib.Description,
                                        Dependencies = attrib.Dependencies,
                                        Category = attrib.Category
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
                                        Id = attrib.Upgrade,
                                        Name = attrib.Name,
                                        Cost = attrib.Cost,
                                        Description = attrib.Description,
                                        Dependencies = attrib.Dependencies,
                                        Category = attrib.Category
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
                                        Id = attrib.Upgrade,
                                        Name = attrib.Name,
                                        Cost = attrib.Cost,
                                        Description = attrib.Description,
                                        Dependencies = attrib.Dependencies,
                                        Category = attrib.Category
                                    });

                                }
                            }

                        }



            foreach (var item in upgDb)
            {
                if (upgDb.Where(x => x.ID == item.ID).Count() > 1)
                    throw new ShiftoriumConflictException(item.Id);
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
            foreach (var defaultupg in GetDefaults())
            {
                if (!UpgradeInstalled(defaultupg.ID) && DependenciesInstalled(defaultupg))
                    available.Add(defaultupg);
            }
            return available.ToArray();
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
            if (SaveSystem.IsSandbox == true)
                return true;

            if (string.IsNullOrWhiteSpace(id))
                return true;
            if (SaveSystem.CurrentSave != null)
            {
                if (!IsInitiated)
                    Init();
            }
            try
            {
                if (SaveSystem.CurrentSave == null)
                    return false;

                if (SaveSystem.CurrentSave.StoriesExperienced == null)
                    SaveSystem.CurrentSave.StoriesExperienced = new List<string>();

                if (id.Contains(';'))
                {
                    foreach (var u in id.Split(';'))
                    {
                        if (UpgradeInstalled(u) == false)
                            return false;
                    }
                    return true;
                }

                bool upgInstalled = false;
                if (SaveSystem.CurrentSave.Upgrades.ContainsKey(id))
                    upgInstalled = SaveSystem.CurrentSave.Upgrades[id];

                if (upgInstalled == false)
                    return SaveSystem.CurrentSave.StoriesExperienced.Contains(id);
                return true;
            }
            catch
            {
                Console.WriteLine("Upgrade " + id + "DNE.");
                Console.WriteLine();
                return false;
            }

        }

        //LEAVE THIS AS FALSE. The game will set it when the save is loaded.
        public static bool LogOrphanedUpgrades = false;

        private static IShiftoriumProvider _provider = null;

        [Obsolete("Please annotate your provider with a [ShiftoriumProvider] attribute instead. This function doesn't do anything.")]
        public static void RegisterProvider(IShiftoriumProvider p)
        {
            _provider = p;
        }

        /// <summary>
        /// Gets every upgrade inside the frontend and all mods.
        /// </summary>
        /// <returns>Every single found Shiftorium upgrade.</returns>
        public static List<ShiftoriumUpgrade> GetDefaults()
        {
            return upgDb;
        }
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

    

    public class ShiftoriumUpgrade
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ulong Cost { get; set; }
        public string ID { get { return (this.Id != null ? this.Id : (Name.ToLower().Replace(" ", "_"))); } }
        public string Id { get; set; }
        public string Category { get; set; }
        public bool Installed
        {
            get
            {
                return Shiftorium.UpgradeInstalled(ID);
            }
        }
        public string Dependencies { get; set; }
    }

    public class ShiftoriumUpgradeAttribute : RequiresUpgradeAttribute
    {
        public ShiftoriumUpgradeAttribute(string name, ulong cost, string desc, string dependencies, string category) : base(name.ToLower().Replace(" ", "_"))
        {
            Name = name;
            Description = desc;
            Dependencies = dependencies;
            Cost = cost;
            Category = category;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public ulong Cost { get; private set; }
        public string Dependencies { get; private set; }
        public string Category { get; private set; }
    }

    public class ShiftoriumConflictException : Exception
    {
        public ShiftoriumConflictException() : base("An upgrade conflict has occurred while loading Shiftorium Upgrades from an assembly. Is there a duplicate upgrade ID?")
        {

        }

        public ShiftoriumConflictException(string id) : base("An upgrade conflict has occurred while loading Shiftorium Upgrades from an assembly. An upgrade with the ID \"" + id + "\" has already been loaded.")
        {

        }
    }

    public class ShiftoriumProviderAttribute : Attribute
    {

    }
}
