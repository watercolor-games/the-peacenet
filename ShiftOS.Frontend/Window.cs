using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.Frontend
{
    public abstract class Window : IShiftOSWindow
    {
        public void OnLoad()
        {
            throw new NotImplementedException();
        }

        public void OnSkinLoad()
        {
            throw new NotImplementedException();
        }

        public bool OnUnload()
        {
            throw new NotImplementedException();
        }

        public void OnUpgrade()
        {
            throw new NotImplementedException();
        }
    }
}
