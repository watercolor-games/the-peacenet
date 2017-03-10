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
        IList<Panel> editorBoxes = new List<Panel>();

        string commandMode = "command";
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
            parts.Add(part);
            Panel container = new Panel();

            Control drawnPart = part.Draw();
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
            string command = "";
            Dictionary<string,string> arguments = new Dictionary<string, string>();

            string text = richTextBox1.Text;
            int position = 0;

            int commandPos;
            int firstValuePos = -1;
            int lastValuePos = -1;

            for(int ii = 0; ii < parts.Count; ii++) {
                CommandFormat part = parts[ii];
                if (part is CommandFormatMarker) {
                    if (part is CommandFormatCommand) {
                        commandPos = ii;
                    } else if (part is CommandFormatValue) {
                        if (firstValuePos > -1)
                            lastValuePos = ii;
                        else
                            firstValuePos = ii;
                    }
                }
            }

            int i = 0;
            string currentArgument = "";
            int help = -1;

            while (position < text.Length) {

                if (i >= parts.Count) {
                    position = text.Length;
                    command = "+FALSE+";
                    i = 0;
                }

                CommandFormat part = parts[i];
                string res = part.CheckValidity(text.Substring(position));

                // ok so:

                // example
                // COMMAND text[ --] ARGUMENT VALUE text[ --] ARGUMENT VALUE
                // COMMAND text[{] ARGUMENT text[=] VALUE text[, ] ARGUMENT text[=] VALUE text[}]

                if (part is CommandFormatMarker) {
                    if (part is CommandFormatCommand) {
                        command = res;
                        help = -1;
                    } else if (part is CommandFormatArgument) {
                        currentArgument = res;
                        help = -1;
                    } else if (part is CommandFormatValue) {
                        arguments[currentArgument] = res;

                        if(i == firstValuePos)
                            help = lastValuePos;
                        if (i == lastValuePos)
                            help = firstValuePos;
                    }
                }

                if(res == "+FALSE+") {
                    if(help > -1) {
                        i = help;
                        if(i >= parts.Count) {
                            position = text.Length;
                            command = "+FALSE+";
                        }
                    }else {
                        position = text.Length;
                        command = "+FALSE+";
                    }
                    help = -1;
                }else {
                    position += res.Length;
                }

                i++;
            }

            if (command == "+FALSE+") {
                lblExampleCommand.Text = "Syntax Error";
            } else {
                string argvs = "{";

                foreach (KeyValuePair<string, string> entry in arguments) {
                    argvs += entry.Key + "=" + entry.Value + ", ";
                }

                argvs += "}";

                lblExampleCommand.Text = command + argvs;
            }
        }

        private void btnTest_Click(object sender, EventArgs e) {

        }
    }

    interface CommandFormat {
        string CheckValidity(string check);
        Control Draw();
    }
    class CommandFormatText : CommandFormat {
        protected string str;
        TextBox textBox;

        public CommandFormatText() {
        }

        public virtual string CheckValidity(string check) {
            return check.StartsWith(str) ? str : "+FALSE+";
        }

        public Control Draw() {
            textBox = new TextBox();
            textBox.TextChanged += new EventHandler(TextChanged);
            textBox.Location = new Point(0,0);

            return textBox;
        }

        void TextChanged(object sender, EventArgs e) {
            str = textBox.Text;
        }
    }

    class CommandFormatOptionalText : CommandFormatText {
        public override string CheckValidity(string check) {
            return check.StartsWith(str) ? str : "";
        }
    }

    class CommandFormatMarker : CommandFormat {
        protected string str;
        Button button;

        public CommandFormatMarker() {
        }

        public virtual string CheckValidity(string check) {
            string res = string.Empty;
            string alphanumeric = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890"; // not using regex for performance reasons

            foreach (char c in check) {
                if (alphanumeric.IndexOf(c) > -1) {
                    res += c;
                } else {
                    break;
                }
            }

            return res;
        }

        public virtual Control Draw() {
            button = new Button();
            button.Location = new Point(0, 0);
            button.Text = "Marker";

            return button;
        }
    }

    class CommandFormatCommand : CommandFormatMarker {
        public override Control Draw() {
            Button draw = (Button)base.Draw();
            draw.Text = "Command";
            return draw;
        }
    }

    class CommandFormatArgument : CommandFormatMarker {
        public override Control Draw() {
            Button draw = (Button)base.Draw();
            draw.Text = "Argument";
            return draw;
        }
    }

    class CommandFormatValue : CommandFormatMarker {
        public override string CheckValidity(string cd) {
            string res = string.Empty;
            var check = "";

            if (cd.StartsWith("\""))
                check = cd.Substring(1);
            bool done = false;

            foreach (char c in check) {
                Console.WriteLine(check);
                if (c != '"') {
                    res += c;
                } else {
                    done = true;
                    res += "\"";
                    break;
                }
            }

            return done ? "\""+res : "+FALSE+";
        }

        public override Control Draw() {
            Button draw = (Button)base.Draw();
            draw.Text = "\"Value\"";
            return draw;
        }
    }
}
