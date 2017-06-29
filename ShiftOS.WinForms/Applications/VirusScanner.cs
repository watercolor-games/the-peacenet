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
using System.Threading;
using Newtonsoft.Json;

namespace ShiftOS.WinForms.Applications
{
    [RequiresUpgrade("virus_scanner")]
    [WinOpen("virus_scanner")]
    [Launcher("Virus Scanner", false, null, "Utilities")]
    [DefaultTitle("Virus Scanner")]
    public partial class VirusScanner : UserControl, IShiftOSWindow
    {
        private int grade = 1;

        public VirusScanner()
        {
            InitializeComponent();
        }

        public void OnLoad()
        {
            pnlintro.BringToFront();
        }

        private List<Objects.ViralInfection> FoundViruses = new List<Objects.ViralInfection>();
        
        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
            grade = 1;
            if (Shiftorium.UpgradeInstalled("virus_scanner_grade_2"))
                grade++;
            if (Shiftorium.UpgradeInstalled("virus_scanner_grade_3"))
                grade++;
            if (Shiftorium.UpgradeInstalled("virus_scanner_grade_4"))
                grade++;
            SetupStatus();
        }

        public void SetupStatus()
        {
            lbstatus.Text = $"Grade: {grade}";
        }

        private string _scanMessage = "We are currently scanning your system. Please wait.";
        private int _threatsFound = 0;

        public int ThreatsFound
        {
            get
            {
                return _threatsFound;
            }
            set
            {
                _threatsFound = value;
                lbscanstatus.Text = $@"{_scanMessage}

Threats found: {_threatsFound}";
            }
        }

        public string ScannerMessage
        {
            get
            {
                return _scanMessage;
            }
            set
            {
                _scanMessage = value;
                lbscanstatus.Text = $@"{_scanMessage}

Threats found: {_threatsFound}";
                
            }
        }

        private void btnscanmem_Click(object sender, EventArgs e)
        {
            ScannerMessage = "We are currently scanning the system memory. Please wait.";
            ThreatsFound = 0;
            ShowScannerUI();
            ScanMemory();
        }

        public void ScanMemory()
        {
            pgscannerprogress.Maximum = SaveSystem.CurrentSave.ViralInfections.Count;
            var t = new Thread(() =>
            {
                foreach(var virus in SaveSystem.CurrentSave.ViralInfections)
                {
                    Thread.Sleep(1000);
                    Desktop.InvokeOnWorkerThread(() =>
                    {
                        pgscannerprogress.Value++;
                        ThreatsFound++;
                        if(virus.ThreatLevel <= grade)
                        {
                            FoundViruses.Add(virus);
                        }
                    });
                }
                Desktop.InvokeOnWorkerThread(() =>
                {
                    SetupSummary();
                });
            });
            t.IsBackground = true;
            t.Start();
        }

        public void SetupSummary()
        {
            flviruses.Controls.Clear();
            if(FoundViruses.Count == 0)
            {
                var noViruses = new Label();
                noViruses.Text = "No threats found.";
                noViruses.Tag = "header3";
                Tools.ControlManager.SetupControls(noViruses);
                noViruses.AutoSize = true;
                flviruses.Controls.Add(noViruses);
                noViruses.Show();
            }
            foreach(var virus in FoundViruses)
            {
                var headerLabel = new Label();
                headerLabel.Text = $"virus.th{virus.ThreatLevel}.{virus.ID}";
                flviruses.Controls.Add(headerLabel);
                headerLabel.Tag = "header3";
                headerLabel.MaximumSize = new Size(((flviruses.Width) - flviruses.Padding.Right) - flviruses.Padding.Left, 0);
                headerLabel.AutoSize = true;
                Tools.ControlManager.SetupControl(headerLabel);
                headerLabel.Show();

                var descLabel = new Label();
                descLabel.MaximumSize = new Size(((flviruses.Width) - flviruses.Padding.Right) - flviruses.Padding.Left, 0);
                descLabel.AutoSize = true;
                var virusInfo = GetVirusInformation(virus.ID);
                descLabel.Text = $@"Real name: {virusInfo.Name}
Description: {virusInfo.Description}";
                if(virus is FileViralInfection)
                {
                    descLabel.Text += Environment.NewLine + "File path: " + (virus as FileViralInfection).FilePath;
                }
                flviruses.Controls.Add(descLabel);
                descLabel.Show();

                var cleanButton = new Button();
                cleanButton.Text = "Disinfect";
                cleanButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                cleanButton.AutoSize = true;
                cleanButton.Click += (o, a) =>
                {
                    Disinfect(virus);
                };
                flviruses.Controls.Add(cleanButton);
                Tools.ControlManager.SetupControl(cleanButton);
                cleanButton.Show();
            }
            pnlsummary.BringToFront();
        }

        public void Disinfect(Objects.ViralInfection virus)
        {
            if (FoundViruses.Contains(virus))
            {
                FoundViruses.Remove(virus);
                VirusManager.Disinfect(virus.ID);
                if(virus is FileViralInfection)
                {
                    var fVirus = virus as FileViralInfection;
                    if (Objects.ShiftFS.Utils.FileExists(fVirus.FilePath))
                    {
                        var headerText = Objects.ShiftFS.Utils.GetHeaderText(fVirus.FilePath);
                        try
                        {
                            var list = JsonConvert.DeserializeObject<List<FileViralInfection>>(headerText);
                            var hVirus = list.FirstOrDefault(x => x.ID == virus.ID && x.ThreatLevel == virus.ThreatLevel);
                            list.Remove(hVirus);
                            Objects.ShiftFS.Utils.SetHeaderText(fVirus.FilePath, JsonConvert.SerializeObject(list));
                        }
                        catch
                        {

                        }
                    }
                }
            }
            SetupSummary();
        }

        public VirusAttribute GetVirusInformation(string id)
        {
            foreach(var type in ReflectMan.Types.Where(x => x.GetInterfaces().Contains(typeof(IVirus)) && Shiftorium.UpgradeAttributesUnlocked(x)))
            {
                var attrib = type.GetCustomAttributes(false).FirstOrDefault(x => x is VirusAttribute) as VirusAttribute;
                if(attrib != null)
                {
                    if (attrib.ID == id)
                        return attrib;
                }
            }
            return null;
        }

        public void ShowScannerUI()
        {
            pgscannerprogress.Value = 0;
            pnlscanner.BringToFront();
        }

        private int filesScanned = 0;

        public void ScanFile(string path)
        {
            Thread.Sleep(25);
            var headerinfo = Objects.ShiftFS.Utils.GetHeaderText(path);
            filesScanned++;
            Desktop.InvokeOnWorkerThread(() =>
            {
                ScannerMessage = "Scanning file: " + path + Environment.NewLine + "Files scanned: " + filesScanned.ToString();
            });


            try
            {
                
                var list = JsonConvert.DeserializeObject<List<Objects.ViralInfection>>(headerinfo).Where(x => x.ThreatLevel <= grade);
                foreach (var virus in list)
                {
                    Thread.Sleep(50);
                    Desktop.InvokeOnWorkerThread(() =>
                    {
                        ThreatsFound++;
                    });
                    FoundViruses.Add(new FileViralInfection
                    {
                        ID = virus.ID,
                        ThreatLevel = virus.ThreatLevel,
                        FilePath = path
                    });
                    
                }

            }
            catch { }

        }

        public void ScanDirectory(string dir)
        {
            if (Objects.ShiftFS.Utils.DirectoryExists(dir))
            {
                foreach (var file in Objects.ShiftFS.Utils.GetFiles(dir))
                    ScanFile(file);
                foreach (var subdir in Objects.ShiftFS.Utils.GetDirectories(dir))
                    ScanDirectory(subdir);
            }
        }

        private void btnscanfile_Click(object sender, EventArgs e)
        {
            FileSkimmerBackend.GetFile(new[] { "" }, FileOpenerStyle.Open, (path) =>
             {
                 ScanFile(path);
                 SetupSummary();
             });
        }

        private void btnscanfs_Click(object sender, EventArgs e)
        {
            filesScanned = 0;
            ShowScannerUI();
            pgscannerprogress.Hide();
            ThreatsFound = 0;
            var t = new Thread(() =>
            {
                foreach(var mount in Objects.ShiftFS.Utils.Mounts)
                {
                    var index = Objects.ShiftFS.Utils.Mounts.IndexOf(mount);
                    ScanDirectory(index.ToString() + ":");
                }
                Desktop.InvokeOnWorkerThread(() =>
                {
                    SetupSummary();
                });
            });
            t.Start();
        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            AppearanceManager.Close(this);
        }
    }

    public class FileViralInfection : Objects.ViralInfection
    {
        public string FilePath { get; set; }
    }
}
