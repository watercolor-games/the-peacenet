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
using ShiftOS.WinForms.Tools;

namespace ShiftOS.WinForms.Applications
{
    [DefaultTitle("Notifications")]
    [Launcher("Notifications", true, "al_notifications", "Utilities")]
    public partial class Notifications : UserControl, IShiftOSWindow
    {
        public Notifications()
        {
            InitializeComponent();
            onMade = (note) =>
            {
                SetupUI();
            };
            onRead += () =>
            {
                SetupUI();
            };
        }

        Action<Notification> onMade = null;
        Action onRead = null;

        public void SetupUI()
        {
            fllist.Controls.Clear();

            bool showNoNotes = true;
            foreach (var note in NotificationDaemon.GetAllFromFile())
            {
                if (note.Read == false)
                {
                    try
                    {
                        showNoNotes = false;
                        var headerLabel = new Label();
                        headerLabel.Tag = "header2";
                        ControlManager.SetupControl(headerLabel);
                        headerLabel.Text = ParseNotification(note);
                        headerLabel.Width = fllist.Width - 4;
                        fllist.Controls.Add(headerLabel);
                        headerLabel.Show();

                        var markButton = new Button();
                        ControlManager.SetupControl(markButton);
                        markButton.Text = "Mark as read";
                        markButton.AutoSize = true;
                        markButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                        markButton.Click += (o, a) =>
                        {
                            NotificationDaemon.MarkRead(new List<Notification>(NotificationDaemon.GetAllFromFile()).IndexOf(note));
                            SetupUI();
                        };
                        fllist.Controls.Add(markButton);
                        markButton.Show();

                        var dataLabel = new Label();
                        dataLabel.Text = ParseNotificationData(note);
                        dataLabel.MaximumSize = new Size(fllist.Width - 4, 0);
                        dataLabel.AutoSize = true;
                        fllist.Controls.Add(dataLabel);
                        dataLabel.Show();
                    }
                    catch
                    {

                    }
                }
            }
            
            if(showNoNotes == true)
            {
                var lbl = new Label();
                lbl.Tag = "header3";
                ControlManager.SetupControl(lbl);
                lbl.AutoSize = true;
                lbl.Text = "You have no notifications.";
                fllist.Controls.Add(lbl);
                lbl.Show();
            }


        }

        public string ParseNotificationData(Notification note)
        {
            string data = "";
            data = note.Timestamp.ToString();
            data += Environment.NewLine + Environment.NewLine;

            switch (note.Type)
            {
                case NotificationType.ChatBan:
                    data += "You have been banned from " + note.Data.ToString() + ".";
                    break;
                case NotificationType.CodepointsReceived:
                    data += "You have received " + note.Data.ToString() + " Codepoints.";
                    break;
                case NotificationType.CodepointsSent:
                    data += "You have lost " + note.Data.ToString() + " Codepoints.";
                    break;
                case NotificationType.CriticalBugwatch:
                    data += "ShiftOS is in a critical bugwatch state. If you see any bugs, report them to us immediately.";
                    break;
                case NotificationType.DownloadComplete:
                    data += "Download of file " + note.Data.ToString() + " complete.";
                    break;
                case NotificationType.DownloadStarted:
                    data += "Download started. Destination: " + note.Data.ToString() + ".";
                    break;
                case NotificationType.Generic:
                    data += note.Data.ToString();
                    break;
                case NotificationType.LegionBan:
                    data += "You have been banned from " + note.Data.ToString();
                    break;
                case NotificationType.LegionInvite:
                    data += "You have been invited to a legion. Invite code: " + note.Data.ToString();
                    break;
                case NotificationType.LegionKick:
                    data += "You have been kicked out of " + note.Data.ToString() + ".";
                    break;
                case NotificationType.MemoReceived:
                    data += "New memo received from " + note.Data.ToString() + "!";
                    break;
                case NotificationType.MemoSent:
                    data += "Memo successfully sent to " + note.Data.ToString() + ".";
                    break;
                case NotificationType.MUDAnnouncement:
                    data += note.Data.ToString();
                    break;
                case NotificationType.MUDMaintenance:
                    data += "The MUD will be going down for a little while at " + note.Data.ToString() + ". Please beware.";
                    break;
                case NotificationType.NewAppveyor:
                    data += "A new AppVeyor build of ShiftOS has been released.";
                    break;
                case NotificationType.NewDeveloper:
                    data += "A new developer, " + note.Data.ToString() + " has been added to the team!";
                    break;
                case NotificationType.NewShiftOSStable:
                    data += "The latest ShiftOS stable, " + note.Data.ToString() + " has been released!";
                    break;
                case NotificationType.NewShiftOSStream:
                    data += "We're going to be live at http://youtube.com/ShiftOS/live at " + note.Data.ToString() + " (Eastern Standard Time).";
                    break;
                case NotificationType.NewShiftOSUnstable:
                    data += "A new unstable version of ShiftOS has been released: " + note.Data.ToString() + ".";
                    break;
                case NotificationType.NewShiftOSVideo:
                    data += "A new ShiftOS video has been released! Get to it before Victor Tran!";
                    break;
                case NotificationType.SavePurge:
                    data += "A purge has occurred inside the multi-user domain. If you have lost your save, it is because it has gone rogue and has been purged.";
                    break;
                case NotificationType.ShopPurchase:
                    data += "You have successfully purchased " + note.Data.ToString() + ".";
                    break;
                default:
                    data += "Corrupt notification data.";
                    break;
            }

            return data;
        }

        public string ParseNotification(Notification note)
        {
            switch (note.Type)
            {
                case NotificationType.ChatBan:
                    return "Banned from chat";
                case NotificationType.CodepointsReceived:
                    return "Codepoints received.";
                case NotificationType.CodepointsSent:
                    return "Codepoints sent.";
                case NotificationType.CriticalBugwatch:
                    return "Critical Bugwatch in progress";
                case NotificationType.DownloadComplete:
                    return "Download complete.";
                case NotificationType.DownloadStarted:
                    return "Download started.";
                case NotificationType.Generic:
                    return "System update";
                case NotificationType.LegionBan:
                    return "Banned from legion";
                case NotificationType.LegionInvite:
                    return "Legion invite received.";
                case NotificationType.LegionKick:
                    return "Kicked from legion";
                case NotificationType.MemoReceived:
                    return "New memo";
                case NotificationType.MemoSent:
                    return "Memo sent.";
                case NotificationType.MUDAnnouncement:
                    return "MUD Announcement";
                case NotificationType.MUDMaintenance:
                    return "MUD maintenance warning!";
                case NotificationType.NewAppveyor:
                    return "New AppVeyor build of ShiftOS";
                case NotificationType.NewDeveloper:
                    return "Please welcome our newest developer...";
                case NotificationType.NewShiftOSStable:
                    return "The newest ShiftOS stable is out!";
                case NotificationType.NewShiftOSStream:
                    return "We're going live soon!";
                case NotificationType.NewShiftOSUnstable:
                    return "The latest ShiftOS unstable is out!";
                case NotificationType.NewShiftOSVideo:
                    return "New ShiftOS video is out!";
                case NotificationType.SavePurge:
                    return "MUD save purge";
                case NotificationType.ShopPurchase:
                    return "Item purchased from shop.";
                default:
                    return "Unknown";
            }
        }

        public void OnLoad()
        {
            SetupUI();
            NotificationDaemon.NotificationMade += onMade;
            NotificationDaemon.NotificationRead += onRead;
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            NotificationDaemon.NotificationMade -= onMade;
            NotificationDaemon.NotificationRead -= onRead;
            return true;
        }

        public void OnUpgrade()
        {
        }
    }
}
