using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    public interface IShiftOSWindow
    {
        void OnLoad();

        void OnSkinLoad();
        bool OnUnload();
        void OnUpgrade();
    }
}
