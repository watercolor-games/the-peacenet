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
using ShiftOS.Objects.ShiftFS;
using Newtonsoft.Json;

namespace ShiftOS.WinForms.Applications
{
    [Launcher("Experience Shifter", false, "", "Customization")]
    [DefaultTitle("Experience Shifter")]
    [DefaultIcon("iconShifter")]
    public partial class ExperienceShifter : UserControl, IShiftOSWindow
    {
        public ExperienceShifter()
        {
            InitializeComponent();
        }

        private string currentUI = "desktop";

        public void SetupUI()
        {
            switch (currentUI)
            {
                case "desktop":
                    pnldesktop.BringToFront();
                    PopulateDesktops();
                    break;
                case "applauncher":
                    pnlapplauncher.BringToFront();
                    PopulateLaunchers();
                    break;
            }
        }

        public void PopulateDesktops()
        {
            lbdesktops.Items.Clear();
            foreach(var desk in GetAllDesktops())
            {
                lbdesktops.Items.Add(desk.DesktopName);
            }
        }

        public List<IDesktop> GetAllDesktops()
        {
            List<IDesktop> dekstops = new List<IDesktop>();
            dekstops.Add(new WinformsDesktop());
            if (!Utils.FileExists(Paths.GetPath("conf.sft")))
                Utils.WriteAllText(Paths.GetPath("conf.sft"), JsonConvert.SerializeObject(new ShiftOSConfigFile(), Formatting.Indented));

            foreach(var script in JsonConvert.DeserializeObject<ShiftOSConfigFile>(Utils.ReadAllText(Paths.GetPath("conf.sft"))).Desktops)
            {
                if(Utils.FileExists(script))
                    dekstops.Add(new LuaDesktop(script));
            }
            return dekstops;
        }

        public void PopulateLaunchers()
        {
            lblaunchers.Items.Clear();
            lbdesktops.Items.Add("ShiftOS App Launcher");
        }

        public void OnLoad()
        {
            SetupUI();
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

        private void desktopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentUI = "desktop";
            SetupUI();
        }

        private void appLauncherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentUI = "applauncher";
            SetupUI();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSkimmerBackend.GetFile(new[] { ".lua" }, FileOpenerStyle.Open, new Action<string>((script) =>
             {
                 ShiftOSConfigFile conf = new WinForms.ShiftOSConfigFile();
                 if (Utils.FileExists(Paths.GetPath("conf.sft")))
                 {
                     conf = JsonConvert.DeserializeObject<ShiftOSConfigFile>(Utils.ReadAllText(Paths.GetPath("conf.sft")));
                 }
                 conf.Desktops.Add(script);
                 Utils.WriteAllText(Paths.GetPath("conf.sft"), JsonConvert.SerializeObject(conf, Formatting.Indented));
             }));
        }

        private void lbdesktops_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach(var desk in GetAllDesktops())
            {
                try
                {
                    if(desk.DesktopName == lbdesktops.SelectedItem.ToString())
                    {
                        Desktop.Init(desk, true);
                    }
                }
                catch
                {

                }
            }
        }
    }
}
