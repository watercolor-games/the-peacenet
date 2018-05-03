using Plex.Engine;
using Plex.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Filesystem.ClientMounts
{
    public class BinDirectory : IClientMount
    {
        public string Path => "/bin";

        [Dependency]
        private TerminalManager _terminalManager = null;

        private Random _random = new Random();

        [Dependency]
        private AppLauncherManager _al = null;

        public byte[] GetFileContents(string filename)
        {
            if (!GetFiles().Contains(filename))
                throw new IOException("File not found.");

            //Grab the command.
            var cmd = _terminalManager.GetCommandList().FirstOrDefault(x => x.Name == filename);
            if (cmd == null)
            {
                foreach(var cat in _al.GetAllCategories())
                {
                    var item = _al.GetAllInCategory(cat).FirstOrDefault(x => x.WindowType.Name.ToLower() == filename);
                    if (item != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            using (var writer = new BinaryWriter(ms, Encoding.UTF32, true))
                            {
                                writer.Write(item.GetHashCode());
                                writer.Write(item.WindowType.ToBytes());
                                writer.Write(item.GetType().ToBytes());
                            }
                            return ms.ToArray();
                        }
                    }

                }
            }
            else
            {
                using (var ms = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(ms, Encoding.UTF32, true))
                    {
                        writer.Write(cmd.GetHashCode());
                        writer.Write(cmd.GetType().ToBytes());
                        writer.Write(typeof(ITerminalCommand).ToBytes());
                    }
                    return ms.ToArray();
                }
            }
            throw new IOException("File not found.");
        }

        public string[] GetFiles()
        {
            List<string> names = new List<string>();
            var cmds = _terminalManager.GetCommandList().Select(x => x.Name).ToArray();
            names.AddRange(cmds);
            foreach(var cat in _al.GetAllCategories())
            {
                foreach(var item in _al.GetAllInCategory(cat))
                {
                    names.Add(item.WindowType.Name.ToLower());
                }
            }
            return names.ToArray();
        }
    }
}
