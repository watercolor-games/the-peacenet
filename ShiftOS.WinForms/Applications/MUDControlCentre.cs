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
using Newtonsoft.Json;
using ShiftOS.Objects;
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications
{
    [RequiresUpgrade("mud_fundamentals")]
    [Launcher("MUD Control Centre", true, "al_mud_control_centre")]
    [WinOpen("mud_control_centre")]
    public partial class MUDControlCentre : UserControl, IShiftOSWindow
    {
        public MUDControlCentre()
        {
            if (SaveSystem.CurrentSave.CurrentLegions == null)
                SaveSystem.CurrentSave.CurrentLegions = new List<string>();
            InitializeComponent();
            ServerManager.MessageReceived += (msg) =>
            {
                try
                {
                    this.Invoke(new Action(() =>
                    {
                        if (msg.Name == "user_not_found_in_legion")
                        {

                            ShowLegionInfo(new Legion
                            {
                                Name = "No legion",
                                ShortName = "NONE",
                                Description = "You are not currently in any legions! You can go to 'Join Legion' and look for a legion to join, however, or create your own.",
                                Publicity = LegionPublicity.UnlistedInviteOnly,
                                BannerColor = ConsoleColor.DarkRed
                            });
                        }
                        else if(msg.Name == "legion_users_found")
                        {
                            lvusers.Items.Clear();
                            foreach(var usr in JsonConvert.DeserializeObject<string[]>(msg.Contents))
                            {
                                lvusers.Items.Add(usr);
                            }
                        }
                        else if (msg.Name == "user_legion")
                        {
                            ShowLegionInfo(JsonConvert.DeserializeObject<Legion>(msg.Contents));
                        }
                        else if(msg.Name == "legion_all")
                        {
                            PopulateJoinLegion(JsonConvert.DeserializeObject<List<Legion>>(msg.Contents));
                        }

                    }));
                }
                catch { }
            };
        }

        public void PopulateJoinLegion(List<Legion> legions)
        {
            lgn_join.BringToFront();

            fllegionlist.Controls.Clear();

            foreach(var lgn in legions)
            {
                var bnr = new Panel();
                bnr.Height = 100;
                bnr.Tag = "keepbg";
                bnr.BackColor = GetColor(lgn.BannerColor);

                bnr.Width = fllegionlist.Width;

                var lTitle = new Label();
                lTitle.AutoSize = true;
                lTitle.Tag = "keepbg header2";
                lTitle.Text = $"[{lgn.ShortName}] {lgn.Name}";
                lTitle.Location = new Point(18, 17);
                bnr.Controls.Add(lTitle);
                lTitle.Show();


                var flButtons = new FlowLayoutPanel();
                flButtons.AutoSize = true;
                flButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                flButtons.Tag = "keepbg";
                flButtons.FlowDirection = FlowDirection.RightToLeft;
                flButtons.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                flButtons.Top = 2;
                flButtons.Left = bnr.Width - flButtons.Width - 2;
                bnr.Controls.Add(flButtons);
                flButtons.Show();

                var btn = new Button();
                btn.Text = "More info";
                btn.Click += (o, a) =>
                {
                    ShowLegionInfo(lgn);
                };
                flButtons.Controls.Add(btn);
                btn.Show();

                fllegionlist.Controls.Add(bnr);
                bnr.Show();
                ControlManager.SetupControls(bnr);
            }
        }

        public Color GetColor(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return Color.Black;
                case ConsoleColor.Gray:
                    return Color.Gray;
                case ConsoleColor.DarkGray:
                    return Color.DarkGray;
                case ConsoleColor.Blue:
                    return Color.Blue;
                case ConsoleColor.Cyan:
                    return Color.Cyan;
                case ConsoleColor.DarkBlue:
                    return Color.DarkBlue;
                case ConsoleColor.DarkCyan:
                    return Color.DarkCyan;
                case ConsoleColor.DarkGreen:
                    return Color.DarkGreen;
                case ConsoleColor.DarkMagenta:
                    return Color.DarkMagenta;
                case ConsoleColor.DarkRed:
                    return Color.DarkRed;
                case ConsoleColor.DarkYellow:
                    return Color.YellowGreen;
                case ConsoleColor.Yellow:
                    return Color.Yellow;
                case ConsoleColor.Green:
                    return Color.Green;
                case ConsoleColor.Magenta:
                    return Color.Magenta;
                case ConsoleColor.Red:
                    return Color.Red;
                case ConsoleColor.White:
                    return Color.White;
                default:
                    return Color.Black;
            }

        }

        public void OnLoad()
        {
            ServerManager.MessageReceived += (msg) =>
            {
                if(msg.Name == "mud_usermemos")
                {
                    try
                    {
                        foreach (var memo in JsonConvert.DeserializeObject<MUDMemo[]>(msg.Contents))
                        {
                            this.Invoke(new Action(() =>
                            {
                                var lbtitle = new Label();
                                lbtitle.Text = memo.Subject;
                                lbtitle.Tag = "header3";
                                ControlManager.SetupControls(lbtitle);
                                flmemos.Controls.Add(lbtitle);

                                var lbsubject = new Label();
                                lbsubject.Text = "From " + memo.UserFrom;
                                flmemos.Controls.Add(lbsubject);

                                var lbbody = new Label();
                                lbbody.Margin = new Padding(0, 15, 0, 15);
                                lbbody.Text = memo.Body;
                                flmemos.Controls.Add(lbbody);

                                lbtitle.Show();
                                lbsubject.Show();
                                lbbody.Show();

                            }));
                        }
                    }
                    catch { }
                }
            };

            SetupSystemStatus();
        }

        public void SetupSystemStatus()
        {
            int scripts = 0;
            string legionname = "";
            
            foreach(var lgn in SaveSystem.CurrentSave.CurrentLegions)
            {
                legionname += Environment.NewLine + " - " + lgn;
            }

            you_systemstatus.BringToFront();

            lblsysstatus.Text = $@"Username: {SaveSystem.CurrentSave.Username}
System name: {SaveSystem.CurrentSave.SystemName}

Codepoints: {SaveSystem.CurrentSave.Codepoints}
Upgrades: {SaveSystem.CurrentSave.CountUpgrades()}/{Shiftorium.GetDefaults().Count}

System version: {SaveSystem.CurrentSave.MajorVersion}.{SaveSystem.CurrentSave.MinorVersion}.{SaveSystem.CurrentSave.Revision}

Shared scripts: {scripts}

Current legions: {legionname}";
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

        private void tsMemos_Click(object sender, EventArgs e)
        {
            ServerManager.SendMessage("get_memos_for_user", $@"{{
    username: ""{SaveSystem.CurrentSave.Username}""                
}}");
            you_memos.BringToFront();
        }

        private void profileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetupSystemStatus();
        }

        private void disconnectFromMuDToolStripMenuItem_Click(object sender, EventArgs e)
        {



        }

        public void ShowLegionInfo(Legion lgn)
        {
            lgn_view.BringToFront();

            lblegiontitle.Text = $"[{lgn.ShortName}] {lgn.Name}";
            lbdescription.Text = lgn.Description;
            if(lgn.Publicity == LegionPublicity.PublicInviteOnly || lgn.Publicity == LegionPublicity.UnlistedInviteOnly)
            {
                btnjoinlegion.Hide();
            }

            banner.BackColor = GetColor(lgn.BannerColor);

            ServerManager.SendMessage("legion_get_users", JsonConvert.SerializeObject(lgn));

            btnleavelegion.Hide();

            if(SaveSystem.CurrentSave.CurrentLegions.Contains(lgn.ShortName))
            {
                btnjoinlegion.Hide();
                btnleavelegion.Show();
            }

        }

        private void myLegionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerManager.SendMessage("user_get_legion", JsonConvert.SerializeObject(SaveSystem.CurrentSave));
        }

        private void joinLegionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerManager.SendMessage("legion_get_all", "");
        }

        private void btnjoinlegion_Click(object sender, EventArgs e)
        {
            string shortname = lblegiontitle.Text.Split(']')[0].Remove(0, 1);

            SaveSystem.CurrentSave.CurrentLegions.Add(shortname);

            SaveSystem.SaveGame();
            ServerManager.SendMessage("user_get_legion", JsonConvert.SerializeObject(SaveSystem.CurrentSave));
        }

        private void btnleavelegion_Click(object sender, EventArgs e)
        {
            string shortname = lblegiontitle.Text.Split(']')[0].Remove(0, 1);

            SaveSystem.CurrentSave.CurrentLegions.Remove(shortname);

            SaveSystem.SaveGame();
            ServerManager.SendMessage("user_get_legion", JsonConvert.SerializeObject(SaveSystem.CurrentSave));

        }

        Legion newLegion = null;

        private void createLegionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newLegion = new Legion();

            SetupLegionEditor(newLegion);
        }

        public void SetupLegionEditor(Legion l)
        {
            editingLegion = l;
            cbcolorchooser.Items.Clear();
            foreach(var name in Enum.GetNames(typeof(ConsoleColor)))
            {
                cbcolorchooser.Items.Add(name);
            }
            cbcolorchooser.Text = l.BannerColor.ToString();
            cbcolorchooser.SelectedIndexChanged += (o, a) =>
            {
                panel4.BackColor = GetColor((ConsoleColor)Enum.Parse(typeof(ConsoleColor), cbcolorchooser.Text));
                editingLegion.BannerColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), cbcolorchooser.Text);
            };

            cbpublicity.Items.Clear();
            cbpublicity.SelectedIndexChanged += (o, a) =>
            {
                var pb = (LegionPublicity)Enum.Parse(typeof(LegionPublicity), cbpublicity.Text);
                editingLegion.Publicity = pb;
            };
            foreach(var pb in Enum.GetNames(typeof(LegionPublicity)))
            {
                cbpublicity.Items.Add(pb);
            }
            cbpublicity.Text = editingLegion.Publicity.ToString();

            if (editingLegion.ShortName == null)
                editingLegion.ShortName = "NAME";
            if (editingLegion.Name == null)
                editingLegion.Name = "Legion name";
            if (editingLegion.Description == null)
                editingLegion.Description = "This is your legion description.";

            txtnewlegionshortname.Text = editingLegion.ShortName;
            txtnewlegiondescription.Text = editingLegion.Description;
            txtnewlegiontitle.Text = editingLegion.Name;
            panel4.BackColor = GetColor(editingLegion.BannerColor);

            lgn_create.BringToFront();
        }

        Legion editingLegion;

        private void txtnewlegionshortname_TextChanged(object sender, EventArgs e)
        {
            var g = txtnewlegionshortname.CreateGraphics();

            SizeF sf = g.MeasureString(txtnewlegionshortname.Text, txtnewlegionshortname.Font);
            editingLegion.ShortName = txtnewlegionshortname.Text;
            txtnewlegionshortname.Size = new Size((int)sf.Width, (int)sf.Height);
        }

        private Shop CurrentShop = null;

        private void txtnewlegiontitle_TextChanged(object sender, EventArgs e)
        {
            var g = txtnewlegiontitle.CreateGraphics();

            SizeF sf = g.MeasureString(txtnewlegiontitle.Text, txtnewlegiontitle.Font);
            editingLegion.Name = txtnewlegiontitle.Text;
            txtnewlegiontitle.Size = new Size((int)sf.Width, (int)sf.Height);

        }

        private void flowLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cbcolorchooser_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void btncreate_Click(object sender, EventArgs e)
        {
            ServerManager.SendMessage("legion_create", JsonConvert.SerializeObject(editingLegion));
            SaveSystem.CurrentSave.CurrentLegions.Clear();
            SaveSystem.CurrentSave.CurrentLegions.Add(editingLegion.ShortName);
            SaveSystem.SaveGame();
            myLegionToolStripMenuItem_Click(sender, e);
        }

        private void txtnewlegiondescription_TextChanged(object sender, EventArgs e)
        {
            editingLegion.Description = txtnewlegiondescription.Text;
        }

        private void currentTaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.job_current.BringToFront();
        }
    }
}
