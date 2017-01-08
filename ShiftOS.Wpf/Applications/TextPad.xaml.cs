using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShiftOS.Objects.ShiftFS;
using ShiftOS.Engine;

namespace ShiftOS.Wpf.Applications
{
    /// <summary>
    /// Interaction logic for TextPad.xaml
    /// </summary>
    [Launcher("TextPad", true, "al_textpad")]
    [RequiresUpgrade("textpad")]
    public partial class TextPad : UserControl, IShiftOSWindow
    {
        public TextPad()
        {
            InitializeComponent();
        }

        private void btnnew_Click(object sender, RoutedEventArgs e)
        {
            txtbody.Text = "";
        }

        private void btnsave_Click(object sender, RoutedEventArgs e)
        {
            string filters = ".txt";
            if (Shiftorium.UpgradeInstalled("textpad_lua_support"))
                filters += ";.lua";
            if (Shiftorium.UpgradeInstalled("textpad_python_support"))
                filters += ";.py";

            FileSkimmerBackend.GetFile(filters.Split(';'), FileOpenerStyle.Save, new Action<string>((file) => SaveFile(file)));
        }

        private void btnopen_Click(object sender, RoutedEventArgs e)
        {
            string filters = ".txt";
            if (Shiftorium.UpgradeInstalled("textpad_lua_support"))
                filters += ";.lua";
            if (Shiftorium.UpgradeInstalled("textpad_python_support"))
                filters += ";.py";

            FileSkimmerBackend.GetFile(filters.Split(';'), FileOpenerStyle.Open, new Action<string>((file) => LoadFile(file)));

        }

        public void LoadFile(string path)
        {
            txtbody.Text = Utils.ReadAllText(path);
        }

        public void SaveFile(string path)
        {
            Utils.WriteAllText(path, txtbody.Text);
        }

        public void OnLoad()
        {
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
/*            btnnew.Upgrade("textpad_new");
            btnopen.Upgrade("textpad_open");
            btnsave.Upgrade("textpad_save");
*/        }
    }
}
