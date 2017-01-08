using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShiftOS.Engine {
    public partial class AltTabWindow : Form {
        public AltTabWindow() {
            InitializeComponent();
        }

        internal void CycleBack() {
            Console.WriteLine("Cycle Backwards");
        }

        internal void CycleForwards() {
            Console.WriteLine("Cycle Forwards");
        }

        private void AltTabWindow_Load(object sender, EventArgs e) {

        }
    }
}
