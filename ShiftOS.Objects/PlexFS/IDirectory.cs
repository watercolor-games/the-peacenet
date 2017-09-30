namespace Plex.Objects.PlexFS
{
    // This exists so that I can make a remote wrapper that calls the
    // real implementation on the server, like Michael wants.
    public interface IDirectory
    {
        public bool Exists(string fname);
        public List<string> Contents { get; };
        public void Delete(string fname);
        public void Rename(string oldName, string newName);
        public void Move(string fname, IDirectory destDir, string destName);
        public IDirectory GetSubdirectory(string dname);
        public System.IO.Stream GetFile(string fname);
    }
}

