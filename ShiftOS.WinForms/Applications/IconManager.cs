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
using System.Reflection;
using ShiftOS.WinForms.Tools;
using Newtonsoft.Json;

namespace ShiftOS.WinForms.Applications
{
    [FileHandler("Icon Pack", ".icons", "fileiconicons")]
    [RequiresUpgrade("icon_manager")]
    [Launcher("{TITLE_ICONMANAGER}", true, "al_icon_manager", "{AL_CUSTOMIZATION}")]
    [DefaultTitle("{TITLE_ICONMANAGER}")]
    [DefaultIcon("iconIconManager")]
    public partial class IconManager : UserControl, IShiftOSWindow, IFileHandler
    {
        public IconManager()
        {
            InitializeComponent();
        }

        public void OpenFile(string file)
        {
            var contents = Objects.ShiftFS.Utils.ReadAllText(file);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(contents);
            AppearanceManager.SetupWindow(this);
            Icons = dict;
            SetupUI();
        }

        public void OnLoad()
        {
            LoadIconsFromEngine();
            SetupUI();
        }

        public void OnSkinLoad()
        {
            LoadIconsFromEngine();
            SetupUI();
        }

        public bool OnUnload()
        {
            Icons = null;
            return true;
        }

        private Dictionary<string, byte[]> Icons = null;

        private const int pageSize = 10;
        private int currentPage = 0;
        private int pageCount = 0;

        public Image GetIcon(string id)
        {
            if (!Icons.ContainsKey(id))
                Icons.Add(id, null);

            if (Icons[id] == null)
            {
                var img = SkinEngine.GetDefaultIcon(id);
                using (var mstr = new System.IO.MemoryStream())
                {
                    img.Save(mstr, System.Drawing.Imaging.ImageFormat.Png);
                    Icons[id] = mstr.ToArray();
                }
                return img;
            }
            else
            {
                using (var sr = new System.IO.MemoryStream(Icons[id]))
                {
                    return Image.FromStream(sr);
                }
            }
        }

        public void SetIcon(string key, byte[] raw)
        {
            if (!Icons.ContainsKey(key))
                Icons.Add(key, raw);
            Icons[key] = raw;
        }

        public void LoadIconsFromEngine()
        {
            //We have to serialize the engine icon list to JSON to break references with the data.
            string json = JsonConvert.SerializeObject(SkinEngine.LoadedSkin.AppIcons);
            //And deserialize to the local instance...essentially making a clone.
            Icons = JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(json);
        }

        public void SetupUI()
        {
            flbody.Controls.Clear(); //Clear the icon list.

            Type[] types = Array.FindAll(ReflectMan.Types, x => x.GetCustomAttributes(false).FirstOrDefault(y => y is DefaultIconAttribute) != null);

            pageCount = types.GetPageCount(pageSize);

            foreach (var type in Array.FindAll(types.GetItemsOnPage(currentPage, pageSize), t => Shiftorium.UpgradeAttributesUnlocked(t)))
            {
                var pnl = new Panel();
                pnl.Height = 30;
                pnl.Width = flbody.Width - 15;
                flbody.Controls.Add(pnl);
                pnl.Show();
                var pic = new PictureBox();
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
                pic.Size = new Size(24, 24);
                pic.Image = GetIcon(type.Name);
                pnl.Controls.Add(pic);
                pic.Left = 5;
                pic.Top = (pnl.Height - pic.Height) / 2;
                pic.Show();
                var lbl = new Label();
                lbl.Tag = "header3";
                lbl.AutoSize = true;
                lbl.Text = NameChangerBackend.GetNameRaw(type);
                ControlManager.SetupControl(lbl);
                pnl.Controls.Add(lbl);
                lbl.CenterParent();
                lbl.Show();
                var btn = new Button();
                btn.Text = "Change...";
                btn.AutoSize = true;
                btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                pnl.Controls.Add(btn);
                btn.Left = (pnl.Width - btn.Width) - 5;
                btn.Top = (pnl.Height - btn.Height) / 2;
                btn.Click += (o, a) =>
                {
                    var gfp = new GraphicPicker(pic.Image, lbl.Text + " icon", ImageLayout.Stretch, (raw, img, layout) =>
                    {
                        pic.Image = img;
                        SetIcon(type.Name, raw);
                    });
                    AppearanceManager.SetupDialog(gfp);
                };
                btn.Show();
                ControlManager.SetupControls(pnl);
            }

            btnnext.Visible = (currentPage < pageCount - 1);
            btnprev.Visible = (currentPage > 0);

            lbcurrentpage.Text = "Page " + (currentPage + 1).ToString() + " of " + pageCount.ToString();
        }

        public void OnUpgrade()
        {
            LoadIconsFromEngine();
            SetupUI();
        }

        private void btnprev_Click(object sender, EventArgs e)
        {
            currentPage--;
            SetupUI();
        }

        public void ResetToDefaults()
        {
            currentPage = 0;
            foreach (var key in Icons.Keys)
            {
                var img = SkinEngine.GetDefaultIcon(key);
                using(var ms = new System.IO.MemoryStream())
                {
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    Icons[key] = ms.ToArray();
                }
            }
            SetupUI();
        }

        private void btnnext_Click(object sender, EventArgs e)
        {
            currentPage++;
            SetupUI();
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            AppearanceManager.Close(this);
        }

        private void btnreset_Click(object sender, EventArgs e)
        {
            ResetToDefaults();
        }

        private void btnapply_Click(object sender, EventArgs e)
        {
            SkinEngine.LoadedSkin.AppIcons = Icons;
            SkinEngine.SaveSkin();
            SkinEngine.LoadSkin();
            Infobox.Show("Icons applied!", "The new icons have been applied to ShiftOS successfully!");
        }
    }

    public static class PaginationExtensions
    {
        public static int GetPageCount<T>(this IEnumerable<T> collection, int pageSize)
        {
            return (collection.Count() + pageSize - 1) / pageSize;
        }

        public static T[] GetItemsOnPage<T>(this T[] collection, int page, int pageSize)
        {
            List<T> obj = new List<T>();

            for (int i = pageSize * page; i <= pageSize + (pageSize * page) && i < collection.Count(); i++)
            {
                try
                {
                    obj.Add(collection[i]);
                }
                catch
                {
                }
            }
            return obj.ToArray();
        }
    }

}
