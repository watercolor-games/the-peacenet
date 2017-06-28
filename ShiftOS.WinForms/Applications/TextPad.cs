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

using ShiftOS.Objects.ShiftFS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Applications
{
    [FileHandler("Text File", ".txt", "fileicontext")]
    [Launcher("{TITLE_TEXTPAD}", true, "al_textpad", "{AL_ACCESSORIES}")]
    [RequiresUpgrade("textpad")]
    [WinOpen("{WO_TEXTPAD}")]
    [DefaultTitle("{TITLE_TEXTPAD}")]
    [DefaultIcon("iconTextPad")]
    public partial class TextPad : UserControl, IShiftOSWindow, IFileHandler
    {
        public TextPad()
        {
            InitializeComponent();
        }

        public void OpenFile(string file)
        {
            AppearanceManager.SetupWindow(this);
            LoadFile(file);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtcontents.Text = "";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var types = new List<string>();
            types.Add(".txt");
            if (ShiftoriumFrontend.UpgradeInstalled("textpad_lua_support"))
                types.Add(".lua");
            if (ShiftoriumFrontend.UpgradeInstalled("textpad_python_support"))
                types.Add(".py");


            AppearanceManager.SetupDialog(new FileDialog(types.ToArray(), FileOpenerStyle.Open, new Action<string>((file) => this.LoadFile(file))));
        }

        public void LoadFile(string file)
        {
            txtcontents.Text = Utils.ReadAllText(file);
        }

        public void SaveFile(string file)
        {
            Utils.WriteAllText(file, txtcontents.Text);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var types = new List<string>();
            types.Add(".txt");
            if (ShiftoriumFrontend.UpgradeInstalled("textpad_lua_support"))
                types.Add(".lua");
            if (ShiftoriumFrontend.UpgradeInstalled("textpad_python_support"))
                types.Add(".py");

            AppearanceManager.SetupDialog(new FileDialog(types.ToArray(), FileOpenerStyle.Save, new Action<string>((file) => this.SaveFile(file))));

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
        }

        private void txtcontents_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
    }
}
