using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Objects;
using static ShiftOS.Objects.ShiftFS.Utils;

namespace ShiftOS.WinForms.Applications
{
    [Launcher("Virus Scanner", true, "al_virus_scanner")]
    [RequiresUpgrade("virus_scanner")]
    [WinOpen("virus_scanner")]
    public partial class VirusScanner : UserControl, IShiftOSWindow
    {
        public VirusScanner()
        {
            InitializeComponent();
            Action<ServerMessage> runner = new Action<ServerMessage>((msg) =>
            {
                if(msg.Name == "virusdb")
                {
                    VirusDB = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Contents);
                }
            });

            ServerManager.MessageReceived += (srv) =>
            {
                runner?.Invoke(srv);
                runner = null;
            };

            ServerManager.SendMessage("getvirusdb", "");
        }

        Dictionary<string, string> VirusDB = null;

        private void btnfullscan_Click(object sender, EventArgs e)
        {
            lblabout.Hide();
            lbviruses.Hide();
            rtbterm.Show();
            rtbterm.Focus();
            rtbterm.Text = "";
            var t = new Thread(new ThreadStart(() =>
            {
                ScanFolder("0:");
            }));
            t.IsBackground = true;
            t.Start();
        }

        public List<string> infected = new List<string>();

        public void ScanFolder(string path)
        {
            this.Invoke(new Action(() =>
            {
                lblresults.Hide();
            }));
            foreach (var file in GetFiles(path))
            {
                Console.WriteLine(file + " is now being scanned.");
                string contents = ReadAllText(file);

                foreach(var kv in VirusDB)
                {
                    if(kv.Value == contents)
                    {
                        if(kv.Key.EndsWith(".0") || kv.Key.EndsWith(".1"))
                        {
                            infected.Add(file);
                            Console.WriteLine($"{file} - Virus detected: {kv.Key}");
                            this.Invoke(new Action(() =>
                            {
                                AddVirusToList(kv.Key);
                            }));
                        }
                    }
                }
            }

            foreach(var dir in GetDirectories(path))
            {
                if (dir != null)
                {
                    ScanFolder(dir);
                }
            }
        }

        public void AddVirusToList(string type)
        {
            lblresults.Hide();
            lbviruses.Show();
            btnremoveviruses.Show();
            lbviruses.Items.Add(type);
        }

        private void VirusScanner_Load(object sender, EventArgs e)
        {
            Applications.Terminal.MakeWidget(rtbterm);
            rtbterm.Hide();
        }

        private void btnhomescan_Click(object sender, EventArgs e)
        {
            lblabout.Hide();
            rtbterm.Show();
            rtbterm.Focus();
            rtbterm.Text = "";
            var t = new Thread(new ThreadStart(() =>
            {
                ScanFolder(Paths.GetPath("home"));
            }));
            t.IsBackground = true;
            t.Start();
        }

        private void btnsysscan_Click(object sender, EventArgs e)
        {
            lblabout.Hide();
            rtbterm.Show();
            rtbterm.Focus();
            rtbterm.Text = "";
            var t = new Thread(new ThreadStart(() =>
            {
                ScanFolder(Paths.GetPath("system"));
            }));
            t.IsBackground = true;
            t.Start();
        }

        private void btnremoveviruses_Click(object sender, EventArgs e)
        {
            while(infected.Count > 0)
            {
                Delete(infected[0]);
                infected.RemoveAt(0);
            }

            lbviruses.Items.Clear();
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
    }

    public class InfectedFile
    {
        public string FilePath { get; set; }
        public List<string> Viruses { get; set; }

        public InfectedFile(string fpath, string[] viruses)
        {
            FilePath = fpath;
            Viruses = new List<string>(viruses);
        }
    }
}
