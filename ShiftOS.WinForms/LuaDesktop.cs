using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Engine.Scripting;
using ShiftOS.Objects.ShiftFS;
using static ShiftOS.Engine.SkinEngine;


namespace ShiftOS.WinForms
{
    public partial class LuaDesktop : Form, IDesktop
    {
        public LuaDesktop(string script)
        {
            InitializeComponent();
            interpreter = new LuaInterpreter();
            interpreter.Lua.getCanvas = new Func<Panel>(() =>
            {
                return this.pnlcanvas;
            });
            if (Utils.FileExists(script))
            {
                interpreter.ExecuteFile(script);
            }
            else
            {
                Desktop.Init(new WinformsDesktop(), true);
                Infobox.Show("Script not found.", "Couldn't find a ShiftOS script to handle the desktop environment.");
                this.Close();
            }
        }

        private LuaInterpreter interpreter = null;

        public string DesktopName
        {
            get
            {
                string name = "Unknown";
                try
                {
                    name = (string.IsNullOrWhiteSpace(interpreter.Lua.deskName)) ? "Unknown" : interpreter.Lua.deskName; 
                }
                catch
                {

                }
                return name;
            }
        }

        private IWindowBorder focused = null;

        public Size GetSize()
        {
            return this.Size;
        }

        public void InvokeOnWorkerThread(Action act)
        {
            this.Invoke(act);
        }

        public void PopulateAppLauncher(LauncherItem[] items)
        {
            interpreter.Lua.populateAppLauncher(interpreter.Lua.totable(new List<LauncherItem>(items)));
        }

        public void PopulatePanelButtons()
        {
            interpreter.Lua.populatePanelButtons();
        }

        public void SetupDesktop()
        {
            try
            {
                interpreter.Lua.setupDesktop();
            }
            catch (Exception ex)
            {
                Infobox.Show("Desktop setup error", "The desktop environment threw an exception: \r\n\r\n\r\n" + ex.Message);
                Desktop.Init(new WinformsDesktop(), true);
                this.Close();
            }
        }

        public void ShowWindow(IWindowBorder border)
        {
            var brdr = border as Form;
            focused = border;
            brdr.GotFocus += (o, a) =>
            {
                focused = border;
            };
            brdr.FormBorderStyle = FormBorderStyle.None;
            brdr.Show();
            brdr.TopMost = true;
        }

        /// <summary>
        /// Kills the window.
        /// </summary>
        /// <returns>The window.</returns>
        /// <param name="border">Border.</param>
        public void KillWindow(IWindowBorder border)
        {
            border.Close();
        }

        /// <summary>
        /// Minimizes the window.
        /// </summary>
        /// <param name="brdr">Brdr.</param>
        public void MinimizeWindow(IWindowBorder brdr)
        {
            var loc = (brdr as WindowBorder).Location;
            var sz = (brdr as WindowBorder).Size;
            (brdr as WindowBorder).Tag = JsonConvert.SerializeObject(new
            {
                Size = sz,
                Location = loc
            });
            (brdr as WindowBorder).Location = new Point(this.GetSize().Width * 2, this.GetSize().Height * 2);
        }

        /// <summary>
        /// Maximizes the window.
        /// </summary>
        /// <returns>The window.</returns>
        /// <param name="brdr">Brdr.</param>
        public void MaximizeWindow(IWindowBorder brdr)
        {
            int startY = (LoadedSkin.DesktopPanelPosition == 1) ? 0 : LoadedSkin.DesktopPanelHeight;
            int h = this.GetSize().Height - LoadedSkin.DesktopPanelHeight;
            var loc = (brdr as WindowBorder).Location;
            var sz = (brdr as WindowBorder).Size;
            (brdr as WindowBorder).Tag = JsonConvert.SerializeObject(new
            {
                Size = sz,
                Location = loc
            });
            (brdr as WindowBorder).Location = new Point(0, startY);
            (brdr as WindowBorder).Size = new Size(this.GetSize().Width, h);

        }

        /// <summary>
        /// Restores the window.
        /// </summary>
        /// <returns>The window.</returns>
        /// <param name="brdr">Brdr.</param>
        public void RestoreWindow(IWindowBorder brdr)
        {
            dynamic tag = JsonConvert.DeserializeObject<dynamic>((brdr as WindowBorder).Tag.ToString());
            (brdr as WindowBorder).Location = tag.Location;
            (brdr as WindowBorder).Size = tag.Size;

        }

        private void LuaDesktop_Load(object sender, EventArgs e)
        {
            this.LocationChanged += (o, a) =>
            {
                if (this.Left != 0)
                    this.Left = 0;
                if (this.Top != 0)
                    this.Top = 0;
            };

            this.SizeChanged += (o, a) =>
            {
                if(this.DisplayRectangle != Screen.PrimaryScreen.Bounds)
                {
                    this.WindowState = FormWindowState.Maximized;
                }
            };

            interpreter.Lua.onLoadDesktop();
            SetupDesktop();
            SaveSystem.GameReady += () =>
            {
                InvokeOnWorkerThread(new Action(() =>
                {
                    SetupDesktop();
                }));
            };
            SkinEngine.SkinLoaded += () =>
            {
                if(this.Visible == true)
                {
                    SetupDesktop();
                }
            };
            Shiftorium.Installed += () =>
            {
                if (this.Visible == true)
                {
                    SetupDesktop();
                }

            };

        }
    }
}
