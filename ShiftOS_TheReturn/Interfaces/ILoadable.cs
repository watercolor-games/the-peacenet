using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Engine.Interfaces
{
    public interface ILoadable
    {
        void Load(ContentManager content);
    }
}
