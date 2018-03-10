using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Plex.Engine
{
    /// <summary>
    /// Wraps <see cref="ContentManager"/> with the ability to load more formats.
    /// </summary>
    public sealed class PlexContentManager : ContentManager
    {
        readonly ContentManager real;
        readonly GraphicsDeviceManager gfx;
        static readonly IEnumerable<string> texfmts = new[] { ".BMP", ".GIF", ".JPG", ".JPEG", ".PNG", ".TIF", ".TIFF", ".DDS" };
        Dictionary<string, WeakReference<object>> cache;
        public PlexContentManager(ContentManager real, GraphicsDeviceManager gfx) : base(real.ServiceProvider, real.RootDirectory)
        {
            this.real = real;
            this.gfx = gfx;
            cache = new Dictionary<string, WeakReference<object>>();
        }
        public override T Load<T>(string assetName)
        {
            if (typeof(T) == typeof(Texture2D))
            {
                var split = assetName.Split('/');
                var path = Path.Combine(new[] { real.RootDirectory }.Concat(split.Take(split.Length - 1)).ToArray());
                var fnameb = split[split.Length - 1].ToUpper();
                var options = Directory.EnumerateFiles(path).Where(n => Path.GetFileNameWithoutExtension(n).ToUpper() == fnameb);
                if (!options.Any(n => Path.GetExtension(n).ToUpper() == ".XNB"))
                {
                    WeakReference<object> wr;
                    object val = null;
                    var cached = cache.TryGetValue(assetName, out wr);
                    if (cached) cached = wr.TryGetTarget(out val);
                    if (cached) cached = val != null;
                    if (!cached)
                    {
                        using (var fobj = File.OpenRead(options.First(n => texfmts.Contains(Path.GetExtension(n).ToUpper()))))
                        {
                            val = Texture2D.FromStream(gfx.GraphicsDevice, fobj);
                            (val as Texture2D).Name = assetName;
                            cache[assetName] = new WeakReference<object>(val);
                        }
                    }
                    return (dynamic)val;
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
