using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Objects;
using Peacenet.CoreUtils;

namespace Peacenet.Filesystem
{
    public class FileUtilities : IEngineComponent
    {
        private List<IFileHandler> _handlers = new List<IFileHandler>();

        [Dependency]
        private GameLoop _GameLoop = null;

        [Dependency]
        private FSManager _fs = null;

        [Dependency]
        private FileUtils _path = null;

        public void Initiate()
        {
            foreach(var type in ReflectMan.Types.Where(x=>x.GetInterfaces().Contains(typeof(IFileHandler))))
            {
                var obj = (IFileHandler)_GameLoop.New(type);
                Logger.Log($"Found {obj.Name} file handler from {type.FullName}. Handles the following MIME types:");
                foreach (var mtype in obj.MimeTypes)
                    Logger.Log($" - {mtype}");
                _handlers.Add(obj);
            }
        }

        public bool OpenFile(string path)
        {
            var type = _path.GetMimeType(_path.GetNameFromPath(path));
            var opener = _handlers.FirstOrDefault(x => x.MimeTypes.Contains(type));
            if (opener == null)
                return false;
            opener.OpenFile(path);
            return true;
        }

    }
}
