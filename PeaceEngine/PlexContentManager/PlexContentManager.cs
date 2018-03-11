using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Plex.Objects;

namespace Plex.Engine.PlexContentManager
{
    /// <summary>
    /// Wraps <see cref="ContentManager"/> with the ability to load more formats.
    /// </summary>
    public sealed class PlexContentManager : ContentManager
    {
        readonly Plexgate plex;
        readonly ContentManager real;
        Dictionary<string, WeakReference<object>> cache;
        public PlexContentManager(Plexgate plex) : base(plex.Content.ServiceProvider, plex.Content.RootDirectory)
        {
            this.plex = plex; // no injecting because we need to call base
            real = plex.Content;
            cache = new Dictionary<string, WeakReference<object>>();
        }
        public override T Load<T>(string assetName)
        {
            var split = assetName.Split('/');
            var path = Path.Combine(new[] { real.RootDirectory }.Concat(split.Take(split.Length - 1)).ToArray());
            var fnameb = split[split.Length - 1].ToUpper();
            var options = Directory.EnumerateFiles(path).Where(n => Path.GetFileNameWithoutExtension(n).ToUpper() == fnameb);
            if (!options.Any(n => Path.GetExtension(n).ToUpper() == ".XNB"))
            {
                WeakReference<object> wr;
                object val;
                if (cache.TryGetValue(assetName, out wr) && wr.TryGetTarget(out val) && val != null)
                    return (T)val;
                foreach (var loader in ReflectMan.Types
                         .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ILoader<>) && i.GetGenericArguments()[0] == typeof(T)))
                         .Select(t => (ILoader<T>)plex.New(t)))
                {
                    var fname = options.FirstOrDefault(n => loader.Extensions.Contains(Path.GetExtension(n).ToUpper()));
                    if (fname != null)
                    {
                        T ret;
                        using (var fobj = File.OpenRead(fname))
                            ret = loader.Load(fobj);
                        cache[assetName] = new WeakReference<object>(ret);
                        typeof(T).GetProperty("Name")?.SetValue(ret, assetName);
                        return ret;
                    }
                }
            }
            return real.Load<T>(assetName);
        }
        public override void Unload()
        {
            real.Unload();
            base.Unload();
        }
    }
}
