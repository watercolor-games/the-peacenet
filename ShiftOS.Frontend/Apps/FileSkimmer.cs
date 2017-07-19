using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ShiftOS.Engine;
using static ShiftOS.Objects.ShiftFS.Utils;

namespace ShiftOS.Frontend.Apps
{
    [WinOpen("fileskimmer")]
    [Launcher("File Skimmer", false, null, "System")]
    [DefaultTitle("File Skimmer")]
    public class FileSkimmer : GUI.Control, IShiftOSWindow
    {
        private string _currentdirectory = "0:";
        private const string SD_SYSTEM = "__system";
        private GUI.ListBox _fList = null;
        private GUI.TextControl _currentdirtext = null;

        public void OnLoad()
        {
            Width = 720;
            Height = 480;
            _fList = new GUI.ListBox();
            _fList.KeyEvent += (e) =>
            {
                if(e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                {
                    Navigate(_fList.SelectedItem.ToString());
                }
            };
            _fList.DoubleClick += () =>
            {
                try
                {
                    Navigate(_fList.SelectedItem.ToString());
                }
                catch { }
            };
            AddControl(_fList);
            _currentdirtext = new GUI.TextControl();
            _currentdirtext.AutoSize = true;
            AddControl(_currentdirtext);
            ResetList();
        }

        public void Navigate(string relativepath)
        {
            if (relativepath == "Up one...")
            {
                if (_currentdirectory.Contains('/'))
                {
                    int _index = _currentdirectory.LastIndexOf('/');
                    int _len = _currentdirectory.Length - _index;
                    _currentdirectory = _currentdirectory.Remove(_index, _len);
                    ResetList();
                }
                else
                {
                    _currentdirectory = SD_SYSTEM;
                    ResetList();
                }
                return;
            }

            string path = "";
            if (_currentdirectory == SD_SYSTEM)
                path = relativepath;
            else
                path = _currentdirectory + "/" + relativepath;
            if (DirectoryExists(path))
            {
                _currentdirectory = path;
                ResetList();
            }
            else if (FileExists(path))
            {
                if (!FileSkimmerBackend.OpenFile(path))
                {
                    Engine.Infobox.Show("File Skimmer can't open this file!", "A program that can open files of this type was not found on your computer.");
                }
            }
        }

        public void OnSkinLoad()
        {
            _currentdirtext.Font = SkinEngine.LoadedSkin.Header3Font;
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }

        public void ResetList()
        {
            if (_currentdirectory == SD_SYSTEM)
                _currentdirtext.Text = "My storage drives";
            else
                _currentdirtext.Text = _currentdirectory;

            _fList.ClearItems();
            if (_currentdirectory != SD_SYSTEM)
                _fList.AddItem("Up one...");

            if(_currentdirectory == SD_SYSTEM)
            {
                foreach(var mount in Mounts)
                {
                    _fList.AddItem(Mounts.IndexOf(mount) + ":");
                }
            }
            else
            {
                foreach(var dir in GetDirectories(_currentdirectory))
                {
                    var dinf = GetDirectoryInfo(dir);
                    _fList.AddItem(dinf.Name);
                }
                foreach (var dir in GetFiles(_currentdirectory))
                {
                    var dinf = GetFileInfo(dir);
                    _fList.AddItem(dinf.Name);
                }

            }
            InvalidateTopLevel();
        }


        protected override void OnLayout(GameTime gameTime)
        {
            try
            {
                _currentdirtext.Layout(gameTime);
                _fList.X = 0;
                _fList.Y = 0;
                _fList.Width = Width;
                _fList.Height = Height - _currentdirtext.Height;
                _currentdirtext.X = (Width - _currentdirtext.Width) / 2;
                _currentdirtext.Y = _fList.Height;
            }
            catch { }
        }
    }
}
