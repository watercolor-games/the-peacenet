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

namespace ShiftOS.WinForms.StatusIcons
{
    [DefaultIcon("iconShiftnet")]
    [RequiresUpgrade("victortran_shiftnet")]
    public partial class ShiftnetStatus : UserControl, IStatusIcon
    {
        public ShiftnetStatus()
        {
            InitializeComponent();
        }

        public void Setup()
        {
            var subscription = Applications.DownloadManager.GetAllSubscriptions()[SaveSystem.CurrentSave.ShiftnetSubscription];
            float kilobytes = (float)subscription.DownloadSpeed / 1024F;
            lbserviceprovider.Text = subscription.Name;
            lbstatus.Text = $@"Company: {subscription.Company}
Download speed (KB/s): {kilobytes}";

        }
    }
}
