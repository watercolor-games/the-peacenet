using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShiftOS.Engine;

namespace ShiftOS.Wpf.Applications
{
    /// <summary>
    /// Interaction logic for Infobox.xaml
    /// </summary>
    public partial class Infobox : UserControl, IShiftOSWindow
    {
        public Infobox(string title, string msg)
        {
            InitializeComponent();
            Title = title;
            Message = msg;
        }

        public static void Show(string title, string msg)
        {
            ShiftOS.Engine.Infobox.Show(title, msg);
        }

        public string Title { get; set; }
        public string Message { get; set; }

        public void OnLoad()
        {
            this.SetTitle(Title);
            txtmessage.Text = Message;
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

        private void btnokay_Click(object sender, RoutedEventArgs e)
        {
            AppearanceManager.Close(this);
        }
    }
}
