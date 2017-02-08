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
