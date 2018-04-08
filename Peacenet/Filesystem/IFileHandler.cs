using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Filesystem
{
    public interface IFileHandler
    {
        string Name { get; }
        IEnumerable<string> MimeTypes { get; }

        void OpenFile(string path);
    }

    public interface IFileViewer
    {
        string FilePath { get; }
        void View(string path);
    }
}
