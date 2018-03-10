using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine;
using Plex.Objects;
using Plex.Engine.GUI;

namespace Peacenet
{
    /// <summary>
    /// Provides a simple app launcher backend for the Peacenet.
    /// </summary>
    public class AppLauncherManager : IEngineComponent
    {

        private List<AppLauncherItem> _items = new List<AppLauncherItem>();

        /// <inheritdoc/>
        public void Initiate()
        {
            Logger.Log("App launcher is now looking for items...");
            foreach(var type in ReflectMan.Types.Where(typeof(Window).IsAssignableFrom))
            {
                var attrib = type.GetCustomAttributes(false).FirstOrDefault(x => x is AppLauncherAttribute) as AppLauncherAttribute;
                if(attrib != null)
                {
                    Logger.Log($"Found: {type.Name} - {attrib.Category}: {attrib.Name}");
                    _items.Add(new AppLauncherItem(attrib, type));
                }
            }
        }

        /// <summary>
        /// Retrieve a list of all categories.
        /// </summary>
        /// <returns>A list of all the App Launcher categories found in the database.</returns>
        public string[] GetAllCategories()
        {
            var cats = new List<string>();
            foreach(var item in _items)
            {
                if (!cats.Contains(item.Attribute.Category))
                    cats.Add(item.Attribute.Category);
            }
            return cats.ToArray();
        }

        /// <summary>
        /// Retrieve all app launcher items in a category.
        /// </summary>
        /// <param name="cat">The category to search</param>
        /// <returns>An array of <see cref="AppLauncherItem"/> objects representing items found in the category.</returns>
        public AppLauncherItem[] GetAllInCategory(string cat)
        {
            return _items.Where(x => x.Attribute.Category == cat).ToArray();
        }
    }

    /// <summary>
    /// Indicates that a <see cref="Window"/> should be displayed in the Peacegate OS's App Launcher menu. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AppLauncherAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AppLauncherAttribute"/> class. 
        /// </summary>
        /// <param name="name">The name of the app launcher item for this window.</param>
        /// <param name="cat">The category where this window's app launcher item should reside.</param>
        public AppLauncherAttribute(string name, string cat, string desc = "")
        {
            Name = name;
            Category = cat;
            Description = desc;
        }

        /// <summary>
        /// Retrieves the name of this app launcher attribute.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Retrieves the category where this app launcher attribute should reside.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Displayed under the entry in the launcher.
        /// </summary>
        public string Description { get; private set; }
    }

    /// <summary>
    /// A class which describes an app launcher item.
    /// </summary>
    public class AppLauncherItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AppLauncherItem"/> class. 
        /// </summary>
        /// <param name="attribute">An <see cref="AppLauncherAttribute"/> specifying the name and category for the app launcher item</param>
        /// <param name="window">A <see cref="Type"/> describing a <see cref="Window"/> to open when the item is activated.</param>
        public AppLauncherItem(AppLauncherAttribute attribute, Type window)
        {
            Attribute = attribute;
            WindowType = window;
        }

        /// <summary>
        /// Retrieves the app launcher item's attribute data.
        /// </summary>
        public AppLauncherAttribute Attribute { get; private set; }
        /// <summary>
        /// Retrieves the type of <see cref="Window"/> to open when the item is clicked. 
        /// </summary>
        public Type WindowType { get; private set; }
    }
}
