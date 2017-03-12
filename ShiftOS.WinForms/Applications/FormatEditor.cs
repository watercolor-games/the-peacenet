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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.Engine;

namespace ShiftOS.WinForms.Applications {
    [MultiplayerOnly]
    [Launcher("FormatEditor", true, "al_format_editor", "Games")]
    [RequiresUpgrade("format_editor")]
    [WinOpen("formateditor")]
    [DefaultIcon("iconFormatEditor")]

    public partial class FormatEditor : UserControl, IShiftOSWindow {

        IList<CommandFormat> parts = new List<CommandFormat>();
        CommandParser parser = new CommandParser();
        IList<Panel> editorBoxes = new List<Panel>();

        string commandMode = "namespace";
        int avcount = 0;

        public FormatEditor() {
            InitializeComponent();
        }

        public void OnLoad() {
            OnUpgrade();
        }

        public void OnSkinLoad() { }

        public bool OnUnload() { return true; }

        public void OnUpgrade() {
            btnAddOptionalText.Visible = ShiftoriumFrontend.UpgradeInstalled("format_editor_optional_text");
            btnAddRegexText.Visible = ShiftoriumFrontend.UpgradeInstalled("format_editor_regex");
            btnAddColor.Visible = ShiftoriumFrontend.UpgradeInstalled("format_editor_syntax_highlighting");
        }

        private void addPart(CommandFormat part) {
            parser.AddPart(part);

            addPart(part.Draw());
        }

        private void addPart(Control part) {
            Panel container = new Panel();

            Control drawnPart = part;
            container.Size = drawnPart.Size;
            container.Controls.Add(drawnPart);

            int woffset = 0;
            if (editorBoxes.Count > 0) {
                woffset = editorBoxes.Last().Width + editorBoxes.Last().Location.X;
            } else {
                woffset = 0;
            }

            container.Location = new Point(woffset, 0);
            editorBoxes.Add(container);
            panelEditor.Controls.Add(container);
        }

        private void btnAddText_Click(object sender, EventArgs e) {
            addPart(new CommandFormatText());
        }

        private void btnAddOptionalText_Click(object sender, EventArgs e) {
            addPart(new CommandFormatOptionalText());
        }

        private void btnAddRegexText_Click(object sender, EventArgs e) {
            
        }

        private void btnAddColor_Click(object sender, EventArgs e) {

        }

        private void btnAddCommand_Click(object sender, EventArgs e) {
            switch (commandMode) {
                case "namespace":
                    addPart(new CommandFormatNamespace());
                    commandMode = "command";
                    btnAddCommand.Text = "+ Command";
                    break;
                case "command":
                    addPart(new CommandFormatCommand());
                    commandMode = "argument";
                    btnAddCommand.Text = "+ Argument";
                    break;
                case "argument":
                    addPart(new CommandFormatArgument());
                    commandMode = "value";
                    btnAddCommand.Text = "+ \"Value\"";
                    break;
                case "value":
                    addPart(new CommandFormatValue());
                    avcount++;
                    if (avcount >= 2) {
                        commandMode = "";
                        btnAddCommand.Text = "";
                        btnAddCommand.Enabled = false;
                    }else {
                        commandMode = "argument";
                        btnAddCommand.Text = "+ Argument";
                    }
                    break;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) {
            var result = parser.ParseCommand(richTextBox1.Text);

            if (result.Equals(default(KeyValuePair<KeyValuePair<string, string>, Dictionary<string, string>>))) {
                lblExampleCommand.Text = "Syntax Error";
            } else {
                string argvs = "{";

                foreach (KeyValuePair<string, string> entry in result.Value) {
                    argvs += entry.Key + "=\"" + entry.Value + "\", ";
                }

                argvs += "}";

                lblExampleCommand.Text = result.Key + argvs;
            }
        }

        private void btnTest_Click(object sender, EventArgs e) {

        }

        private void btnSave_Click(object sender, EventArgs e) {
            CurrentCommandParser.parser = parser;

            FileSkimmerBackend.GetFile(new string[] { ".cf" }, FileOpenerStyle.Save, new Action<string>((result) => {
                Objects.ShiftFS.Utils.WriteAllText(result, parser.Save());
            }));
        }

        private void btnLoad_Click(object sender, EventArgs e) {
            FileSkimmerBackend.GetFile(new string[] { ".cf" }, FileOpenerStyle.Open, new Action<string>((result) => {
                parser = CommandParser.Load(Objects.ShiftFS.Utils.ReadAllText(result));
                foreach(CommandFormat part in parser.parts) {
                    addPart(part.Draw());
                }
            }));
        }

        private void btnApply_Click(object sender, EventArgs e) {
            CurrentCommandParser.parser = parser;
        }
    }
}
