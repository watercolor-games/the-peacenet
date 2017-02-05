using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShiftOS.MFSProfiler
{
    public partial class FileCreator : Form
    {
        public FileCreator(string fileToCreate)
        {
            InitializeComponent();
            FileToCreate = fileToCreate;
        }

        public string FileToCreate { get; private set; }

        private void FileCreator_Load(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShiftOS.Objects.ShiftFS.Utils.WriteAllText(FileToCreate + "/" + txtfname.Text, txtcontents.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
