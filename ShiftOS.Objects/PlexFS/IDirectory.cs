using System.Collections.Generic;

namespace Plex.Objects.PlexFS
{
    // This exists so that I can make a remote wrapper that calls the
    // real implementation on the server, like Michael wants.
    public interface IDirectory
    {
        bool Exists(string fname);
        IEnumerable<string> Contents { get; }
        void Delete(string fname);
        void Rename(string oldName, string newName);
        void Move(string fname, IDirectory destDir, string destName);
        IDirectory GetSubdirectory(string dname);
        System.IO.Stream OpenFile(string fname);
    }
}

