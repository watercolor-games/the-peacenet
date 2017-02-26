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
        private int[,] minemap;

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
            makegrid();
        }

        private void makegrid()
        {
            Random rnd1 = new Random();
            minemap = new int[minefieldPanel.ColumnCount, minefieldPanel.RowCount];

            for (int x = 0; x < minefieldPanel.ColumnCount; x++)
            {
                for (int y = 0; y < minefieldPanel.RowCount; y++)
                {
                    minemap[x, y] = 0;
                    minefieldPanel.Controls.Add(makeButton(x, y), x, y);
                }
            }
        }

        private Button makeButton(int col, int row)
        {
            Button bttn = new Button();

            bttn.Text = "";
            bttn.Name = col.ToString() + " " + row.ToString();
            Controls.AddRange(new System.Windows.Forms.Control[] { bttn, });
            bttn.Click += new System.EventHandler(bttnOnclick);
            bttn.MouseDown += new MouseEventHandler(mouseDwn);
            bttn.MouseUp += new MouseEventHandler(mauseUp);
            bttn.MouseHover += new EventHandler(mauseHov);
            bttn.BackgroundImage = Properties.Resources.SweeperTileBlock;

            return bttn;
        }

        private void mauseHov(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage = Properties.Resources.SweeperNormalFace;
        }

        private void mauseUp(object sender, MouseEventArgs e)
        {
            pictureBox1.BackgroundImage = Properties.Resources.SweeperNormalFace;
        }

        private void mouseDwn(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage = Properties.Resources.SweeperClickFace;
        }

        private void bttnOnclick(object sender, EventArgs e)
        {
            
        }
    }
}
