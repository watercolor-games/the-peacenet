/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;
using Newtonsoft.Json;
using static ShiftOS.Objects.ShiftFS.Utils;
using ShiftOS.WinForms.Applications;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Localization.RegisterProvider(new WFLanguageProvider());
            Shiftorium.RegisterProvider(new WinformsShiftoriumProvider());
            AppearanceManager.OnExit += () =>
            {
                Environment.Exit(0);
            };

            TerminalBackend.TerminalRequested += () =>
            {
                AppearanceManager.SetupWindow(new Applications.Terminal());
            };
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppearanceManager.Initiate(new WinformsWindowManager());
            OutOfBoxExperience.Init(new Oobe());
            Infobox.Init(new WinformsInfobox());
            FileSkimmerBackend.Init(new WinformsFSFrontend());
            var desk = new WinformsDesktop();
            Desktop.Init(desk);
            Application.Run(desk);
        }
    }

    internal class WinformsShiftoriumProvider : IShiftoriumProvider
    {
        public List<ShiftoriumUpgrade> GetDefaults()
        {
            return JsonConvert.DeserializeObject<List<ShiftoriumUpgrade>>(Properties.Resources.Shiftorium);
        }
    }

    internal class WinformsInfobox : IInfobox
    {
        public void Open(string title, string msg)
        {
            Dialog frm = new Dialog();
            frm.Text = title;
            var pnl = new Panel();
            var flow = new FlowLayoutPanel();
            var btnok = new Button();
            btnok.AutoSize = true;
            btnok.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flow.Height = btnok.Height + 4;
            btnok.Text = "ok";
            flow.Dock = DockStyle.Bottom;
            flow.Controls.Add(btnok);
            btnok.Show(); btnok.Click += (o, a) =>
            {
                frm.Close();
            };
            pnl.Controls.Add(flow);
            flow.Show();
            var lbl = new Label();
            lbl.Text = msg;
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.Dock = DockStyle.Fill;
            lbl.AutoSize = false;
            pnl.Controls.Add(lbl); lbl.Show();
            frm.Controls.Add(pnl);
            pnl.Dock = DockStyle.Fill;
            frm.Size = new Size(320, 200);
            AppearanceManager.SetupDialog(frm);

        }
    }

    public class WinformsFSFrontend : IFileSkimmer
    {
        public void OpenDirectory(string path)
        {
            var fs = new Applications.FileSkimmer();
            AppearanceManager.SetupWindow(fs);
            fs.ChangeDirectory(path);
        }

        public void GetPath(string[] filetypes, FileOpenerStyle style, Action<string> callback)
        {
            AppearanceManager.SetupDialog(new Applications.FileDialog(filetypes, style, callback));
        }

        public void OpenFile(string path)
        {
            try
            {
                switch (FileSkimmerBackend.GetFileType(path))
                {
                    case FileType.TextFile:
                        if (!Shiftorium.UpgradeInstalled("textpad"))
                            throw new Exception();

                        var txt = new TextPad();
                        AppearanceManager.SetupWindow(txt);
                        txt.LoadFile(path);
                        break;
                    case FileType.Executable:
                        //NYI
                        throw new Exception();
                    case FileType.Python:
                        var p = new Engine.Scripting.PythonInterpreter();
                        try
                        {
                            p.ExecuteFile(path);
                        }
                        catch (Exception ex)
                        {
                            Infobox.Show("{PY_EXCEPTION}", ex.Message);
                        }
                        break;
                    case FileType.Lua:
                        try
                        {
                            var runner = new Engine.Scripting.LuaInterpreter();
                            runner.ExecuteFile(path);
                        }
                        catch (Exception ex)
                        {
                            Infobox.Show("{LUA_ERROR}", ex.Message);
                        }
                        break;
                    case FileType.JSON:
                        //NYI
                        throw new Exception();
                    case FileType.Filesystem:
                        MountPersistent(path);
                        //If this doesn't fail...
                        FileSkimmerBackend.OpenDirectory((Mounts.Count - 1).ToString() + ":");
                        break;
                    case FileType.Skin:
                        if (!Shiftorium.UpgradeInstalled("skinning"))
                            throw new Exception();

                        var sl = new Skin_Loader();
                        AppearanceManager.SetupWindow(sl);
                        sl.LoadedSkin = JsonConvert.DeserializeObject<Skin>(ReadAllText(path));
                        sl.SetupUI();
                        break;
                    case FileType.Image:
                        if (!Shiftorium.UpgradeInstalled("artpad_open"))
                            throw new Exception();

                        var ap = new Artpad();
                        AppearanceManager.SetupWindow(ap);
                        ap.LoadPicture(path);
                        break;
                    default:
                        throw new Exception();

                }
            }
            catch
            {
                Infobox.Show("{NO_APP_TO_OPEN}", "{NO_APP_TO_OPEN_EXP}");
            }

        }

    }
}
