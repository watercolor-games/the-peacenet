using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;
using ShiftOS.Frontend.GUI;
using Newtonsoft.Json;
using static ShiftOS.Objects.ShiftFS.Utils;
using Microsoft.Xna.Framework;

namespace ShiftOS.Frontend.Apps
{
    [DefaultTitle("Installer")]
    [FileHandler("Setup file", ".stp", "")]
    public class Installer : Control, IShiftOSWindow, IFileHandler
    {
        private SetupFile _setup = null;
        private TextControl _header = null;
        private TextControl _body = null;
        private Button _cancel = null;
        private Button _install = null;

        public Installer()
        {
            Width = 600;
            Height = 400;
            _header = new GUI.TextControl();
            _body = new GUI.TextControl();
            _cancel = new Button();
            _install = new Button();

            _install.Text = "Install";
            _cancel.Text = "Close";

            _install.AutoSize = true;
            _cancel.AutoSize = true;

            AddControl(_header);
            AddControl(_body);
            AddControl(_install);
            AddControl(_cancel);

            _install.Click += () =>
            {
                Install();
                
            };
            _cancel.Click += () =>
            {
                AppearanceManager.Close(this);
            };

        }

        public void Install()
        {
            switch (_setup.SourceType)
            {
                case SetupSource.ShiftoriumUpgrade:
                    if (Shiftorium.UpgradeInstalled(_setup.Source))
                    {
                        Engine.Infobox.Show("Upgrade installed.", "This upgrade has already been installed either through the Shiftorium or through another setup file.");
                        return;
                    }
                    Shiftorium.Buy(_setup.Source, 0);
                    Engine.Infobox.Show("Upgrade installed.", "The upgrade \"" + _setup.Source + "\" has been installed and is now ready to be used!");

                    break;
                case SetupSource.CowFile:
                    string cow = _setup.Source;
                    string[] split = cow.Split('\t');
                    string fname = split[0] + ".cow";
                    string ascii = split[1];
                    string csCowfiles = Paths.GetPath("data") + "/cows";
                    if (!DirectoryExists(csCowfiles))
                        CreateDirectory(csCowfiles);
                    WriteAllText(csCowfiles + "/" + fname, ascii);
                    Engine.Infobox.Show("Cowsay", "New cowfile installed! Have fun with your talking " + split[0] + "!");
                    break;
            }
        }


        protected override void OnLayout(GameTime gameTime)
        {
            _header.X = 10;
            _header.Y = 10;
            _header.Font = SkinEngine.LoadedSkin.Header3Font;
            _header.AutoSize = true;

            _body.X = 10;
            _body.Y = _header.Y + _header.Height + 5;
            _body.Width = Width - 20;

            _cancel.X = Width - _cancel.Width - 10;
            _cancel.Y = Height - _cancel.Height - 10;
            _body.Height = (_cancel.Y - _body.Y);
            _install.Y = _cancel.Y;
            _install.X = _cancel.X - _install.Width - 5;
        }


        public void OpenFile(string file)
        {
            _setup = JsonConvert.DeserializeObject<SetupFile>(ReadAllText(file));
            AppearanceManager.SetupDialog(this);
        }

        public void OnLoad()
        {
            _header.Text = _setup.Name;
            _body.Text = _setup.Description;
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

    public class SetupFile
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public SetupSource SourceType { get; set; }
        public string Source { get; set; }
    }

    public enum SetupSource
    {
        ShiftoriumUpgrade,
        CowFile
    }
}
