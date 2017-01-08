using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShiftOS.Engine
{
    public class Infobox
    {
        private static IInfobox _infobox = null;

        [Obsolete("Please use Infobox.Show instead.")]
        public Infobox(string title, string message)
        {
            Infobox.Show(title, message);
        }

        public static void Show(string title, string message)
        {
            _infobox.Open(title, message);
        }

        public static void Init(IInfobox info)
        {
            _infobox = info;
        }
    }

    public interface IInfobox
    {
        void Open(string title, string msg);
    }
}
