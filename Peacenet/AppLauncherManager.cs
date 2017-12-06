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
    public class AppLauncherManager : IEngineComponent
    {
        private List<AppLauncherItem> _items = new List<AppLauncherItem>();

        public void Initiate()
        {
            Logger.Log("App launcher is now looking for items...");
            foreach(var type in ReflectMan.Types)
            {
                if(type.BaseType == typeof(Window))
                {
                    var attrib = type.GetCustomAttributes(false).FirstOrDefault(x => x is AppLauncherAttribute) as AppLauncherAttribute;
                    if(attrib != null)
                    {
                        Logger.Log($"Found: {type.Name} - {attrib.Category}: {attrib.Name}");
                        _items.Add(new AppLauncherItem(attrib, type));
                    }
                }
            }
        }

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

        public AppLauncherItem[] GetAllInCategory(string cat)
        {
            return _items.Where(x => x.Attribute.Category == cat).ToArray();
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
        }

        public void OnGameUpdate(GameTime time)
        {
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
        }

        public void Unload()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AppLauncherAttribute : Attribute
    {
        public AppLauncherAttribute(string name, string cat)
        {
            Name = name;
            Category = cat;
        }

        public string Name { get; private set; }
        public string Category { get; set; }
    }

    public class AppLauncherItem
    {
        public AppLauncherItem(AppLauncherAttribute attribute, Type window)
        {
            Attribute = attribute;
            WindowType = window;
        }

        public AppLauncherAttribute Attribute { get; private set; }
        public Type WindowType { get; private set; }
    }
}
