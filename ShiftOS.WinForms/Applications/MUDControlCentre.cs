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
    [Launcher("MUD Control Centre", true, "al_mud_control_centre", "Networking")]
    [WinOpen("mud_control_centre")]
    [DefaultIcon("iconSysinfo")]
    [DefaultTitle("MUD Control Centre")]
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
                        else if (msg.Name == "legion_create_ok")
                        {
                            SaveSystem.CurrentSave.CurrentLegions.Clear();
                            SaveSystem.CurrentSave.CurrentLegions.Add(editingLegion.ShortName);
                            SaveSystem.SaveGame();
                            this.Invoke(new Action(() =>
                            {
                                myLegionToolStripMenuItem_Click(this, EventArgs.Empty);
                            }));
                        }
                        else if(msg.Name == "chat_all")
                        {
                            this.Invoke(new Action(() =>
                            {
                                this.ListAllChats(JsonConvert.DeserializeObject<Channel[]>(msg.Contents));
                            }));
                        }
                        else if (msg.Name == "user_shop_check_result")
                        {
                            if (msg.Contents == "0")
                            {
                                ShowCreateShop();
                            }
                            else
                            {
                                Infobox.PromptYesNo("Close Shop", "You already own a MUD shop. Would you like to close it and create a new one?", new Action<bool>((result) =>
                                {
                                    if (result == true)
                                    {
                                        ServerManager.SendMessage("shop_removeowned", JsonConvert.SerializeObject(new
                                        {
                                            username = SaveSystem.CurrentSave.Username
                                        }));
                                        ShowCreateShop();
                                    }
                                }));
                            }
                        }
                        else if (msg.Name == "legion_alreadyexists")
                        {
                            this.Invoke(new Action(() =>
                            {
                                Infobox.Show("Legion already exists", "A legion with the short name you provided already exists. Please choose another.");
                            }));
                        }
                        else if (msg.Name == "legion_users_found")
                        {
                            lvusers.Items.Clear();
                            foreach (var usr in JsonConvert.DeserializeObject<string[]>(msg.Contents))
                            {
                                lvusers.Items.Add(usr);
                            }
                        }
                        else if (msg.Name == "user_legion")
                        {
                            ShowLegionInfo(JsonConvert.DeserializeObject<Legion>(msg.Contents));
                        }
                        else if (msg.Name == "shop_taken")
                        {
                            this.Invoke(new Action(() =>
                            {
                                Infobox.Show("Shop name taken.", "A shop with the same name already exists.");
                            }));
                        }
                        else if (msg.Name == "shop_added")
                        {
                            ServerManager.SendMessage("shop_getall", "");
                        }
                        else if (msg.Name == "legion_all")
                        {
                            PopulateJoinLegion(JsonConvert.DeserializeObject<List<Legion>>(msg.Contents));
                        }
                        else if(msg.Name == "user_shop")
                        {
                            this.Invoke(new Action(() =>
                            {
                                ShowShop(JsonConvert.DeserializeObject<Shop>(msg.Contents));
                            }));
                        }
                        else if(msg.Name == "shop_additem")
                        {
                            var contents = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Contents);
                            if((string)contents["shop"] == CurrentShop.Name)
                            {
                                CurrentShop.Items.Add(JsonConvert.DeserializeObject<ShopItem>(DownloadManager.Decompress(DownloadManager.Decompress(contents["itemdata"] as string))));
                                this.Invoke(new Action(PopulateShopView));
                            }
                        }
                        else if(msg.Name == "user_noshop")
                        {
                            this.Invoke(new Action(() =>
                            {
                                Infobox.Show("No shop.", "You do not currently own any shops. You must open one to use this screen.");
                            }));
                        }
                        else if (msg.Name == "shop_all")
                        {
                            this.Invoke(new Action(() =>
                            {
                                PopulateShopList(JsonConvert.DeserializeObject<Shop[]>(msg.Contents));
                            }));
                        }
                    }));
                }
                catch { }
            };
        }

        public void ListAllChats(Channel[] channels)
        {
            shop_all.BringToFront();

            flshoplist.Controls.Clear();

            lblistname.Text = "Chat";
            lblistdesc.Text = "Want to talk with other Shifters on the multi-user domain? Simply select a chatroom below and click 'Join' to join in!";

            foreach (var shop in channels)
            {
                var bnr = new Panel();
                bnr.Height = 100;
                bnr.Tag = "keepbg";

                bnr.Width = flshoplist.Width;

                var lTitle = new Label();
                lTitle.AutoSize = true;
                lTitle.Tag = "keepbg header2";
                lTitle.Text = shop.Name;
                lTitle.Location = new Point(18, 17);
                bnr.Controls.Add(lTitle);
                lTitle.Show();
                var desc = new Label();
                desc.Text = shop.Topic;
                bnr.Controls.Add(desc);
                desc.Show();

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
                btn.Text = "Join";
                btn.Click += (o, a) =>
                {
                    OpenChat(shop.ID);
                };
                flButtons.Controls.Add(btn);
                btn.Show();

                flshoplist.Controls.Add(bnr);
                bnr.Show();
                ControlManager.SetupControls(bnr);
                desc.Left = lTitle.Left;
                desc.Width = (bnr.Width - desc.Left - desc.Left);
                desc.Top = lTitle.Top + lTitle.Height;
                desc.Height = (bnr.Height - lTitle.Top);

            }

        }

        public void OpenChat(string id)
        {
            AppearanceManager.SetupWindow(new Chat(id));
        }

        private Shop editingShop = null;
        private string editingShopOldName = "";

        public void ShowCreateShop()
        {
            this.Invoke(new Action(() =>
            {
                editingShop = new Shop();
                creatingShop = true;
                editingShop.Name = "My shop";
                editingShop.Description = "My shop has lots of awesome items. You should buy from my shop.";
                editingShop.Owner = SaveSystem.CurrentSave.Username;
                editingShop.Items = new List<ShopItem>();
                shop_editor.BringToFront();
                PopulateShopEditor();
            }));
        }

        public void PopulateShopEditor()
        {
            txtshopdescription.Text = editingShop.Description;
            txtshopname.Text = editingShop.Name;
            lbeditingshopitems.Items.Clear();
            foreach(var item in editingShop.Items)
            {
                lbeditingshopitems.Items.Add(item.Name);
            }
        }

        public void PopulateShopList(Shop[] shops)
        {
            shop_all.BringToFront();

            flshoplist.Controls.Clear();
            lblistname.Text = "Shops";
            lblistdesc.Text = "The multi-user domain is full of various shops ran by other users. They can contain anything from skins to applications to full system modifications. Just select a shop below to browse its contents!";

            foreach (var shop in shops)
            {
                var bnr = new Panel();
                bnr.Height = 100;
                bnr.Tag = "keepbg";
                
                bnr.Width = flshoplist.Width;

                var lTitle = new Label();
                lTitle.AutoSize = true;
                lTitle.Tag = "keepbg header2";
                lTitle.Text = shop.Name;
                lTitle.Location = new Point(18, 17);
                bnr.Controls.Add(lTitle);
                lTitle.Show();
                var desc = new Label();
                desc.Text = shop.Description;
                bnr.Controls.Add(desc);
                desc.Show();

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
                btn.Text = "Browse";
                btn.Click += (o, a) =>
                {
                    ShowShop(shop);
                };
                flButtons.Controls.Add(btn);
                btn.Show();

                flshoplist.Controls.Add(bnr);
                bnr.Show();
                ControlManager.SetupControls(bnr);
                desc.Left = lTitle.Left;
                desc.Width = (bnr.Width - desc.Left - desc.Left);
                desc.Top = lTitle.Top + lTitle.Height;
                desc.Height = (bnr.Height - lTitle.Top);

            }

        }

        public void PopulateShopView()
        {
            lbupgrades.Items.Clear();
            shopItems = CurrentShop.Items;
            foreach (var item in CurrentShop.Items)
            {
                lbupgrades.Items.Add(item.Name);
            }

        }

        public void ShowShop(Shop shop)
        {
            shop_view.BringToFront();
            CurrentShop = shop;
            lbshopname.Text = shop.Name;
            lbupgradetitle.Text = $"Welcome to {shop.Name}.";
            lbupgradedesc.Text = shop.Description;
            lbprice.Text = "Select an item from the list on the left.";
            btnbuy.Hide();

            ServerManager.SendMessage("shop_getitems", JsonConvert.SerializeObject(new
            {
                shopname = CurrentShop.Name
            }));

            lbupgrades.SelectedIndexChanged += (o, a) =>
            {
                item = shopItems[lbupgrades.SelectedIndex];
                lbupgradetitle.Text = item.Name;
                lbupgradedesc.Text = item.Description;
                lbprice.Text = $"Cost: {item.Cost} CP";
                btnbuy.Show();
            };
            if(shop.Owner == SaveSystem.CurrentSave.Username)
            {
                btneditshop.Show();
            }
            else
            {
                btneditshop.Hide();
            }
        }

        private ShopItem item = null;
        private List<ShopItem> shopItems = null;

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
            ServerManager.SendMessage("legion_createnew", JsonConvert.SerializeObject(editingLegion));
        }

        private void txtnewlegiondescription_TextChanged(object sender, EventArgs e)
        {
            editingLegion.Description = txtnewlegiondescription.Text;
        }

        private void currentTaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.job_current.BringToFront();
        }

        private void browseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerManager.SendMessage("shop_getall", "");
        }

        private void btnbuy_Click(object sender, EventArgs e)
        {
            if (SaveSystem.CurrentSave.Codepoints >= item.Cost)
            {
                //Send the Codepoints to the shop owner. If they're online, the MUD will tell them to update their Codepoints.
                ServerManager.SendMessage("usr_givecp", JsonConvert.SerializeObject(new
                {
                    username = this.CurrentShop.Owner,
                    amount = item.Cost
                }));

                string fileExt = FileSkimmerBackend.GetFileExtension((FileType)item.FileType);
                FileSkimmerBackend.GetFile(new[] { fileExt }, FileOpenerStyle.Save, new Action<string>((dest) =>
                 {
                     var d = new Download
                     {
                         ShiftnetUrl = $"{CurrentShop.Name}: {item.Name}",
                         Destination = dest,
                         Bytes = item.MUDFile
                     };
                     DownloadManager.StartDownload(d);
                 }));
            
                

            }
            else
            {
                Infobox.Show("Not enough Codepoints", "You cannot afford this item. You need at least " + item.Cost.ToString() + ".");
            }
        }

        private void openAShopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerManager.SendMessage("user_shop_check", JsonConvert.SerializeObject(new
            {
                username = SaveSystem.CurrentSave.Username
            }));
        }

        private void txtshopname_TextChanged(object sender, EventArgs e)
        {
            try
            {
                editingShop.Name = txtshopname.Text;
            }
            catch { }
        }

        private void txtshopdescription_TextChanged(object sender, EventArgs e)
        {
            try
            {
                editingShop.Description = txtshopdescription.Text;
            }
            catch { }
        }

        bool creatingShop = false;

        private void btnsaveshop_Click(object sender, EventArgs e)
        {
            if(creatingShop == true)
            {
                ServerManager.SendMessage("create_shop", JsonConvert.SerializeObject(editingShop));
            }
            else
            {
                ServerManager.SendMessage("update_shop_by_user", JsonConvert.SerializeObject(new
                {
                    username = editingShop.Owner,
                    shop = editingShop
                }));
            }
        }

        private void btnaddshopitem_Click(object sender, EventArgs e)
        {
            AppearanceManager.SetupWindow(new ShopItemCreator(new ShopItem(), new Action<ShopItem>((item) =>
            {
                editingShop.Items.Add(item);
                PopulateShopEditor();
            })));
        }

        private void btnremoveitem_Click(object sender, EventArgs e)
        {
            try
            {
                int i = lbeditingshopitems.SelectedIndex;
                editingShop.Items.RemoveAt(i);
                PopulateShopEditor();
            }
            catch
            {

            }
        }

        private void btnedititem_Click(object sender, EventArgs e)
        {
            try
            {
                int i = lbeditingshopitems.SelectedIndex;
                AppearanceManager.SetupWindow(new ShopItemCreator(editingShop.Items[i], new Action<ShopItem>((item) =>
                {
                    editingShop.Items[i] = item;
                })));

                PopulateShopEditor();
            }
            catch
            {

            }
        }

        private void myShopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerManager.SendMessage("user_get_shop", SaveSystem.CurrentSave.Username);
        }

        private void btneditshop_Click(object sender, EventArgs e)
        {
            editingShop = CurrentShop;
            creatingShop = false;
            shop_editor.BringToFront();
            PopulateShopEditor();
        }

        private void joinAChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerManager.SendMessage("chat_getallchannels", "");
        }

        private void btndeletesave_Click(object sender, EventArgs e)
        {
            Infobox.PromptYesNo("Delete your system", "Are you SURE you want to delete your system? If you do, you will lose ALL of your Codepoints and file system.", (firstres) =>
            {
                if(firstres == true)
                {
                    Infobox.PromptYesNo("Last chance.", "What you are about to do is IRREVERSABLE. You will lose access to any legions or shops you are a part of. Continue anyway?", (second) =>
                    {
                        if(second == true)
                        {
                            ServerManager.SendMessage("delete_save", JsonConvert.SerializeObject(new ClientSave
                            {
                                Username = SaveSystem.CurrentSave.Username,
                                Password = SaveSystem.CurrentSave.Password
                            }));

                            SaveSystem.CurrentSave = null;

                            ShiftOS.Objects.ShiftFS.Utils.Delete(Paths.GetPath("user.dat"));
                            TerminalBackend.InvokeCommand("sos.shutdown");
                            MessageBox.Show("Thank you for playing ShiftOS. Your save data has been deleted successfully. You can come back at anytime, but you will have to restart your game.");
                            System.IO.File.Delete(Paths.SaveFile);
                        }
                    });
                }
            });
        }
    }
}
