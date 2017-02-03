using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShiftOS.WinForms.Applications;
using ShiftOS.Engine;

namespace ShiftOS.WinForms
{
    public partial class DownloadControl : UserControl
    {
        public DownloadControl(int index)
        {
            InitializeComponent();
            var d = DownloadManager.Downloads[index];
            lbshiftneturl.Text = d.ShiftnetUrl;
            pcicon.Image = FileSkimmerBackend.GetImage(d.Destination);
            int bytesTransferred = 0;
            DownloadManager.ProgressUpdate += (i, p) =>
            {
                try
                {
                    this.Invoke(new Action(() =>
                    {
                        if (i == index)
                        {
                            bytesTransferred += 256;
                            pgprogress.Value = bytesTransferred;
                            lbshiftneturl.Text = $@"{d.ShiftnetUrl}
{bytesTransferred} B out of {d.Bytes.Length} B transferred at 256 B per second.
To {d.Destination}";
                            pgprogress.Maximum = d.Bytes.Length;
                        }
                    }));
                }
                catch
                {

                }
            };
        }
    }
}
