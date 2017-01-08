using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
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
    /// Interaction logic for ShiftoriumFrontend.xaml
    /// </summary>
    [Launcher("Shiftorium", true, "al_shiftorium")]
    [RequiresUpgrade("shiftorium_gui")]
    public partial class ShiftoriumFrontend : UserControl, IShiftOSWindow
    {
        public ShiftoriumFrontend()
        {
            InitializeComponent();
            this.DataContextChanged += (o, a) =>
            {
                if (this.DataContext is ShiftoriumUpgrade)
                {
                    var upg = this.DataContext as ShiftoriumUpgrade;
                    title.Text = upg.Name;
                    this.SetTitle("Shiftorium - " + upg.Name);
                    description.Text = upg.Description;
                    cost.Text = upg.Cost.ToString();
                }
            };
            this.DataContext = new ShiftoriumUpgrade
            {
                Name = "Welcome to the Shiftorium",
                Description = @"The Shiftorium is an application that lets you buy upgrades for ShiftOS using Codepoints.

Upgrades can be anything from new apps, to games, to system enhancements, to eyecandy, to anything else. Just be careful! The best thing to do is to buy new ways of earning Codepoints otherwise you'll find yourself unable to earn Codepoints later on.",
                Cost = 0,
                Dependencies = null
            };
        }

        public void Setup()
        {
            lbupgrades.Items.Clear();

            foreach(var itm in Shiftorium.GetAvailable())
            {
                lbupgrades.Items.Add(itm.Name);
            }
        }

        public void OnLoad()
        {
            this.SetTitle("Shiftorium");
            var t = new Timer();
            t.Interval = 500;
            t.Elapsed += (o,a) =>
            {
                try
                {
                    currentcodepoints.Text = SaveSystem.CurrentSave.Codepoints.ToString();
                }
                catch { t.Stop(); }
                
            };
            t.Start();

            lbupgrades.SelectionChanged += (o, a) =>
            {
                try
                {
                    btnbuy.Visibility = Visibility.Visible;
                    this.DataContext = GetUpgradeFromName(lbupgrades.SelectedItem.ToString());
                }
                catch
                {

                }
            };

            btnbuy.Visibility = Visibility.Collapsed;
            Setup();
        }

        public ShiftoriumUpgrade GetUpgradeFromName(string upg)
        {
            foreach(var upgrade in Shiftorium.GetDefaults())
            {
                if (upgrade.Name == upg)
                    return upgrade;
            }
            return null;
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
            codepointDisplay.Upgrade("shiftorium_gui_cp_display");
        }

        private void lbupgrades_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnbuy_Click_1(object sender, RoutedEventArgs e)
        {
            var upg = this.DataContext as ShiftoriumUpgrade;
            Shiftorium.Silent = true;
            if(Shiftorium.Buy(upg.ID, upg.Cost) == true)
            {
                btnbuy.Visibility = Visibility.Collapsed;
                Setup();
            }
            else
            {
                Infobox.Show("Shiftorium", $"You do not have enough Codepoints to buy this upgrade. You need {upg.Cost - SaveSystem.CurrentSave.Codepoints} more.");
            }
        }
    }
}
