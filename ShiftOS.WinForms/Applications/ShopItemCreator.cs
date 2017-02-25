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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using ShiftOS.Objects;
using ShiftOS.Objects.ShiftFS;

namespace ShiftOS.WinForms.Applications
{
    [DefaultTitle("Shop item editor")]
    [MultiplayerOnly]
    [DefaultIcon("iconSysInfo")]
    public partial class ShopItemCreator : UserControl, IShiftOSWindow
    {
        public ShopItemCreator(ShopItem item, Action<ShopItem> callback)
        {
            InitializeComponent();
            Callback = callback;
            Item = item;
            txtitemname.Text = Item.Name;
            txtdescription.Text = Item.Description;
            txtcost.Text = Item.Cost.ToString();
            txtfilename.Text = "Unselected";
        }

        private Action<ShopItem> Callback { get; set; }
        public ShopItem Item { get; private set; }

        private void btnbrowse_Click(object sender, EventArgs e)
        {
            FileSkimmerBackend.GetFile(new[] { "" }, FileOpenerStyle.Open, new Action<string>((result) =>
            {
                txtfilename.Text = result;
                Item.MUDFile = Utils.ReadAllBytes(result);
                Item.FileType = (int)FileSkimmerBackend.GetFileType(result);
            }));
        }

        private void btndone_Click(object sender, EventArgs e)
        {
            if(Item.MUDFile == null)
            {
                Infobox.Show("No file chosen.", "Please select a file to sell.");
                return;
            }
            Item.Cost = Convert.ToInt32(txtcost.Text);
            Item.Description = txtdescription.Text;
            Item.Name = txtitemname.Text;
            Callback?.Invoke(Item);
            AppearanceManager.Close(this);
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

        private void txtcost_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Item.Cost = Convert.ToInt32(txtcost.Text);
            }
            catch
            {
                txtcost.Text = Item.Cost.ToString();
            }
        }
    }
}
