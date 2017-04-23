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
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.ShiftnetSites
{
    [ShiftnetSite("shiftnet/appscape", "Appscape", "Bringing ShiftOS to life")]
    [ShiftnetFundamental]
    public partial class AppscapeMain : UserControl, IShiftnetSite
    {
        public AppscapeMain()
        {
            InitializeComponent();
        }

        public event Action GoBack;
        public event Action<string> GoToUrl;

        public void OnSkinLoad()
        {
            ControlManager.SetupControls(this);
        }

        public void OnUpgrade()
        {
        }

        public string Category = "All";

        public string[] GetCategories()
        {
            var upgrades = Shiftorium.GetDefaults().Where(x => x.Dependencies.Contains("appscape_"));
            List<string> cats = new List<string>();
            cats.Add("All");
            try
            {
                if (upgrades.Count() > 0)
                    foreach (var upg in upgrades)
                    {
                        if (!cats.Contains(upg.Category))
                            cats.Add(upg.Category);
                    }
            }
            catch { }
            return cats.ToArray();
        }

        public ShiftoriumUpgrade CurrentUpgrade = null;

        public void SetupCategory(string cat)
        {
            pnlappslist.Controls.Clear();
            pnlappslist.Show();
            pnlappslist.BringToFront();
            Category = cat;
            var upgrades = GetAllInCategory();
            lbtitle.Text = cat;
            if(upgrades.Length == 0)
            {
                var err = new Label();
                err.AutoSize = true;
                err.Text = "There are no apps in this list! Come back later for more.";
                pnlappslist.Controls.Add(err);
                err.Show();
            }
            else
            {
                var fl = new FlowLayoutPanel();
                fl.Dock = DockStyle.Fill;
                pnlappslist.Controls.Add(fl);
                fl.Show();
                foreach(var upg in upgrades)
                {
                    var pnl = new Panel();
                    pnl.Height = 250;
                    pnl.Width = 200;
                    fl.Controls.Add(pnl);
                    pnl.Show();
                    var upgTitle = new Label();
                    upgTitle.Text = upg.Name;
                    upgTitle.Dock = DockStyle.Top;
                    upgTitle.AutoSize = true;
                    upgTitle.MaximumSize = new Size(pnl.Width, 0);
                    upgTitle.Tag = "header3";
                    pnl.Controls.Add(upgTitle);
                    upgTitle.Show();

                    var cp_display = new Panel();
                    cp_display.Height = 30;
                    cp_display.Dock = DockStyle.Bottom;
                    pnl.Controls.Add(cp_display);
                    cp_display.Show();

                    var cp_value = new Label();
                    if (Shiftorium.UpgradeInstalled(upg.ID))
                    {
                        cp_value.Text = "Out of stock.";
                    }
                    else
                    {
                        cp_value.Text = $"{upg.Cost} CP";
                    }
                    cp_value.AutoSize = true;
                    cp_value.Top = (cp_display.Height - cp_value.Height) / 2;
                    cp_value.Left = 5;
                    cp_display.Controls.Add(cp_value);
                    cp_value.Show();


                    if(cp_value.Text != "Out of stock.")
                    {
                        var more_info = new Button();
                        more_info.Text = "More info";
                        more_info.Click += (o, a) =>
                        {
                            ViewMoreInfo(upg);
                        };
                        more_info.AutoSize = false;
                        more_info.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        more_info.Top = (cp_display.Height - more_info.Height) / 2;
                        more_info.Left = cp_display.Width - more_info.Width - 5;
                        cp_display.Controls.Add(more_info);
                        more_info.Show();
                    }

                    var desc = new Label();
                    desc.Text = upg.Description;
                    desc.AutoSize = false;
                    desc.Dock = DockStyle.Fill;
                    pnl.Controls.Add(desc);
                    desc.Show();
                    desc.BringToFront();


                    ControlManager.SetupControls(pnl);
                }
            }
        }

        public void ViewMoreInfo(ShiftoriumUpgrade upg)
        {

        }

        public ShiftoriumUpgrade[] GetAllInCategory()
        {
            var upgrades = Shiftorium.GetDefaults().Where(x => (x.Dependencies == null) ? false : x.Dependencies.Contains("appscape_"));
            if (upgrades.Count() == 0)
                return new ShiftoriumUpgrade[0];

            if (Category == "All")
                return upgrades.ToArray();
            else
                return upgrades.Where(x => x.Category == Category).ToArray();
        }

        public void Setup()
        {
            flcategories.Controls.Clear();
            foreach(var cat in this.GetCategories())
            {
                var btn = new Button();
                btn.Text = cat;
                btn.Click += (o, a) =>
                {
                    SetupCategory(cat);
                };
                ControlManager.SetupControl(btn);
                btn.Width = flcategories.Width - 2;
                flcategories.Controls.Add(btn);
                btn.Show();
            }
            SetupCategory("All");
        }
    }
}

namespace ShiftOS.WinForms
{
    /// <summary>
    /// Special version of <see cref="RequiresUpgradeAttribute"/> for specifying Appscape applications as Shiftorium upgrades. 
    /// </summary>
    public class AppscapeEntryAttribute : RequiresUpgradeAttribute
    {
        public AppscapeEntryAttribute(string name, string description, long cost, string dependencies = "", string category = "Misc") : base((string.IsNullOrWhiteSpace(dependencies)) ? name.ToLower().Replace(" ","_") : name.ToLower().Replace(" ", "_") + dependencies)
        {
            Name = name;
            Description = description;
            Category = category;
            Cost = cost;
            DependencyString = dependencies;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public long Cost { get; private set; }
        public string DependencyString { get; private set; }
    }
}