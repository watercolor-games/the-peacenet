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
    [Launcher("ShiftSweeper", true, "shiftsweeper", "Games")]
    [RequiresUpgrade("shiftsweeper")]
    [WinOpen("shiftsweeper")]
    [DefaultIcon("iconShiftSweeper")]
    public partial class ShiftSweeper : UserControl, IShiftOSWindow
    {
        private bool gameplayed = false;
        private int mineCount = 0;

        public ShiftSweeper()
        {
            InitializeComponent();
        }

        public void OnLoad()
        {
            buttonE.Visible = true;
            buttonM.Visible = ShiftoriumFrontend.UpgradeInstalled("shiftsweeper_medium");
            buttonH.Visible = ShiftoriumFrontend.UpgradeInstalled("shiftsweeper_hard");
        }

        public void OnSkinLoad() { }

        public bool OnUnload() { return true; }

        public void OnUpgrade()
        {
            
        }

        private void buttonE_Click(object sender, EventArgs e)
        {
            startGame(0);
        }

        private void startGame(int d)
        {
            switch (d)
            {
                case 0:
                    mineCount = 10;
                    minefieldPanel.ColumnCount = 9;
                    minefieldPanel.RowCount = 9;
                    break;

                default:
                    throw new NullReferenceException();
            }
        }
    }
}
