using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Filesystem
{
    public interface IClientMount
    {
        string Path { get; }

        string[] GetFiles();
        byte[] GetFileContents(string filename);
    }
}
