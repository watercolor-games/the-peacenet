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
using static ShiftOS.Objects.ShiftFS.Utils;
using Newtonsoft.Json;

namespace ShiftOS.WinForms.Applications
{
    [WinOpen("address_book")]
    [AppscapeEntry("Address Book", "Add and edit your contacts within the digital society in an easy-to-use application.", 1024, 750, null, "Office")]
    [DefaultTitle("Address Book")]
    [Launcher("Address Book", false, null, "Office")]
    public partial class AddressBook : UserControl, IShiftOSWindow
    {
        public AddressBook()
        {
            InitializeComponent();
        }

        string data_dir = Paths.GetPath("data") + "/address_book";

        public void OnLoad()
        {
            if (!DirectoryExists(data_dir))
                CreateDirectory(data_dir);

            var userDefined = new TreeNode();
            userDefined.Text = "User-defined";
            foreach(var f in GetFiles(data_dir))
            {
                try
                {
                    var contact = JsonConvert.DeserializeObject<Contact>(ReadAllText(f));
                    var node = new TreeNode();
                    node.Text = contact.UserName + "@" + contact.SystemName;
                    node.Tag = contact;
                    userDefined.Nodes.Add(node);
                }
                catch { }
            }
            tvcontacts.Nodes.Add(userDefined);
            userDefined.Expand();
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

        private void addContactToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }

    public class Contact
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string SystemName { get; set; }
        public ContactRelationship Relationship { get; set; }
        public bool IsStoryCharacter { get; set; }
        public string Description { get; set; }
    }

    public enum ContactRelationship
    {
        Acquaintance,
        Friend,
        Enemy
    }
}
