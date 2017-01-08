/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
