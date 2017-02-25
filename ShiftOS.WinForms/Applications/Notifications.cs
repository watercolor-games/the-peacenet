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
        }

        Action<Notification> onMade = null;

        public void SetupUI()
        {

        }

        public void OnLoad()
        {
            SetupUI();
            NotificationDaemon.NotificationMade += onMade;
        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            NotificationDaemon.NotificationMade -= onMade;
            return true;
        }

        public void OnUpgrade()
        {
        }
    }
}
