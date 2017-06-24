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
    [WinOpen("{WO_ADDRESSBOOK}")]
    [AppscapeEntry("address_book", "{TITLE_ADDRESSBOOK}", "{DESC_ADDRESSBOOK}", 1024, 750, null, "{AL_OFFICE}")]
    [DefaultTitle("{TITLE_ADDRESSBOOK}")]
    [Launcher("{TITLE_ADDRESSBOOK}", false, null, "{AL_OFFICE}")]
    public partial class AddressBook : UserControl, IShiftOSWindow
    {
        public AddressBook()
        {
            InitializeComponent();
        }

        string data_dir = Paths.GetPath("data") + "/address_book";
        public void OnLoad()
        {
            removeToolStripMenuItem.Visible = false;
            if (!DirectoryExists(data_dir))
                CreateDirectory(data_dir);
            tvcontacts.Nodes.RemoveByKey("userdefined");
            var userDefined = new TreeNode();
            userDefined.Name = "userdefined";
            userDefined.Text = "User-defined";
            tvcontacts.Click += (o, a) =>
            {
                if (tvcontacts.SelectedNode == userDefined)
                {
                    removeToolStripMenuItem.Visible = false;
                }
            };
            foreach(var f in GetFiles(data_dir))
            {
                try
                {
                    var contact = JsonConvert.DeserializeObject<Contact>(ReadAllText(f));
                    var node = new TreeNode();
                    node.Text = contact.UserName + "@" + contact.SystemName;
                    node.Tag = contact;
                    userDefined.Nodes.Add(node);
                    tvcontacts.Click += (o, a) =>
                    {
                        if(tvcontacts.SelectedNode == node)
                        {
                            lbtitle.Text = contact.Name;
                            txtbody.Text = $@"Username: {contact.UserName}
System Name: {contact.SystemName}

Description:
{contact.Description}";
                            removeToolStripMenuItem.Visible = true;
                            SelectedContact = contact;
                        }
                    };
                }
                catch { }
            }
            tvcontacts.Nodes.Add(userDefined);
            userDefined.Expand();
        }

        public Contact SelectedContact = null;

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
            Infobox.PromptText("Add Contact", "What is the contact's name?", delegate(string name) {
                if (name != "")
                {
                    Infobox.PromptText("Add Contact", "What is the user's username?", delegate (string uname)
                    {
                        if (uname != "")
                        {
                            Infobox.PromptText("Add Contact", "What is the user's systemname?", delegate(string sysname)
                            {
                                if (sysname != "")
                                {
                                    Infobox.PromptText("Add Contact", "How would you describe this user?", delegate (string desc)
                                    {
                                        if (desc != "")
                                        {
                                            Contact contact= new Contact();
                                            contact.Name = name;
                                            contact.UserName = uname;
                                            contact.SystemName = sysname;
                                            contact.Relationship = ContactRelationship.Acquaintance;
                                            contact.IsStoryCharacter = false;
                                            contact.Description = desc;
                                            var contactJson = JsonConvert.SerializeObject(contact);
                                            WriteAllText(data_dir + "/" + name, contactJson);
                                            OnLoad(); // Reload to show changes
                                        } else
                                        {
                                            Infobox.Show("Add Contact", "Description cannot be empty.");
                                        }
                                    });
                                } else
                                {
                                    Infobox.Show("Add Contact", "System name cannot be empty.");
                                }
                            });
                        } else
                        {
                            Infobox.Show("Add Contact", "Username cannot be empty.");
                        }
                    });
                } else
                {
                    Infobox.Show("Add Contact", "Name cannot be empty.");
                }
            });
        }

        private void AddressBook_Load(object sender, EventArgs e)
        {

        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(SelectedContact != null)
            {
                string file = data_dir + "/" + SelectedContact.Name;
                if (FileExists(file))
                {
                    Infobox.PromptYesNo("Remove contact", $"Are you sure you want to remove {SelectedContact.Name} from your Address Book?", (result) =>
                    {
                        if (result == true)
                        {
                            Delete(file);
                            OnLoad();
                        }
                    });
                }
            }
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
