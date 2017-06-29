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
using System.IO;
using System.Reflection;
using System.Threading;

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
            var upgrades = Shiftorium.GetDefaults().Where(x => (x.Dependencies == null) ? false : x.Dependencies.Contains("appscape_"));
            List<string> cats = new List<string>();
            cats.Add("All");
            try
            {
                if (upgrades.Count() > 0)
                    foreach (var upg in upgrades)
                    {
                        if (!cats.Contains(Localization.Parse(upg.Category)))
                            cats.Add(Localization.Parse(upg.Category));
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
            Category = Localization.Parse(cat);
            var upgrades = GetAllInCategory();
            lbtitle.Text = Localization.Parse(cat);
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
                fl.AutoScroll = true;
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
                        cp_value.Text = "Already Purchased.";
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


                    if(cp_value.Text != "Already Purchased.")
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

        public bool DependenciesInstalled(ShiftoriumUpgrade upg)
        {
            string[] split = upg.Dependencies.Split(';');
            foreach(var u in split)
            {
                if (!u.StartsWith("appscape_handled"))
                {
                    if (!Shiftorium.UpgradeInstalled(u))
                        return false;
                }
            }
            return true;
        }

        public void ViewMoreInfo(ShiftoriumUpgrade upg)
        {
            lbtitle.Text = upg.Name;
            pnlappslist.Controls.Clear();

            var cp_display = new Panel();
            cp_display.Height = 30;
            cp_display.Dock = DockStyle.Bottom;
            pnlappslist.Controls.Add(cp_display);
            cp_display.Show();

            var cp_value = new Label();
            if (Shiftorium.UpgradeInstalled(upg.ID))
            {
                cp_value.Text = "Already Purchased.";
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


            if (cp_value.Text != "Already Purchased.")
            {
                var more_info = new Button();
                more_info.Text = "Buy";
                more_info.Click += (o, a) =>
                {
                    //Detect if dependencies are installed.
                    if (DependenciesInstalled(upg))
                    {
                        //Detect sufficient codepoints
                        if (SaveSystem.CurrentSave.Codepoints >= upg.Cost)
                        {
                            Infobox.PromptYesNo("Confirm Purchase", "Do you want to purchase " + upg.Name + " from Appscape for " + upg.Cost.ToString() + " Codepoints?", (result) =>
                            {
                                if (result == true)
                                {
                                    SaveSystem.CurrentSave.Codepoints -= upg.Cost;
                                    foreach (var type in ReflectMan.Types)
                                    {
                                        var attrib = type.GetCustomAttributes(false).FirstOrDefault(x => x is AppscapeEntryAttribute) as AppscapeEntryAttribute;
                                        if (attrib != null)
                                        {
                                            if (attrib.Name == upg.Name)
                                            {
                                                var installer = new Applications.Installer();
                                                var installation = new AppscapeInstallation(upg.Name, attrib.DownloadSize, upg.ID);
                                                AppearanceManager.SetupWindow(installer);
                                                installer.InitiateInstall(installation);
                                                return;
                                            }
                                        }
                                    }
                                }
                            });
                        }
                        else
                        {
                            Infobox.Show("Not enough Codepoints", "You do not have enough Codepoints to buy this package.");
                        }
                    }
                    else
                    {
                        Infobox.Show("Missing dependencies", "You are missing some Shiftorium upgrades that this package requires. Please upgrade your system and try again!");
                    }
                };
                more_info.AutoSize = false;
                more_info.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                more_info.Top = (cp_display.Height - more_info.Height) / 2;
                more_info.Left = cp_display.Width - more_info.Width - 5;
                cp_display.Controls.Add(more_info);
                more_info.Show();
                ControlManager.SetupControls(pnlappslist);
            }

            var desc = new Label();
            desc.Text = upg.Description;
            desc.AutoSize = false;
            desc.Dock = DockStyle.Fill;
            pnlappslist.Controls.Add(desc);
            desc.Show();
            desc.BringToFront();

            desc.Text += Environment.NewLine + Environment.NewLine + "Dependencies:" + Environment.NewLine;
            string[] deplist = upg.Dependencies.Split(';');
            if(deplist.Length > 1)
            {
                for(int i = 1; i < deplist.Length; i++)
                {
                    ShiftoriumUpgrade dep = Shiftorium.GetDefaults().FirstOrDefault(x => x.ID == deplist[i]);
                    if(dep != null)
                    {
                        desc.Text += $" - {dep.Name}{Environment.NewLine}";
                    }
                }
            }
            else
            {
                desc.Text += " - No dependencies.";
            }


        }

        public ShiftoriumUpgrade[] GetAllInCategory()
        {
            var upgrades = Shiftorium.GetDefaults().Where(x => (x.Dependencies == null) ? false : x.Dependencies.Contains("appscape_"));
            if (upgrades.Count() == 0)
                return new ShiftoriumUpgrade[0];

            if (Category == "All")
                return upgrades.ToArray();
            else
                return upgrades.Where(x => Localization.Parse(x.Category) == Localization.Parse(Category)).ToArray();
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
                btn.Width = flcategories.Width - 10;
                flcategories.Controls.Add(btn);
                btn.Show();
            }
            SetupCategory("All");
        }
    }

    public class AppscapeInstallation : Applications.Installation
    {
        public AppscapeInstallation(string name, int size, string s_id)
        {
            Name = name;
            ShiftoriumId = s_id;
            Size = size;
        }

        public string ShiftoriumId { get; private set; }
        public int Size { get; private set; }
        
        protected override void Run()
        {
            this.SetStatus("Downloading...");
            SetProgress(0);
            int i = 0;
            while (i <= Size)
            {
                double progress = ((i / Size) * 100);

                SetProgress((int)progress);

                i += Applications.DownloadManager.GetDownloadSpeed();
                Thread.Sleep(1000);
            }
            SetProgress(0);
            SetStatus("Installing...");
            i = 0;
            while (i <= Size)
            {
                double progress = ((i / Size) * 100);

                SetProgress((int)progress);

                i+=1024;
                Thread.Sleep(1000);
            }
            Desktop.InvokeOnWorkerThread(() =>
            {
                Shiftorium.Buy(ShiftoriumId, 0);
                Infobox.Show("Install complete!", "The installation of " + Name + " has completed.");
                SaveSystem.SaveGame();
            });
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
        public AppscapeEntryAttribute(string id, string name, string description, int downloadSize, ulong cost, string dependencies = "", string category = "Misc") : base(id)
        {
            Name = name;
            Description = description;
            Category = category;
            Cost = cost;
            DependencyString = dependencies;
            DownloadSize = downloadSize;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public ulong Cost { get; private set; }
        public string DependencyString { get; private set; }
        public int DownloadSize { get; private set; }
    }
}