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
using ShiftOS.WinForms.Tools;
using Newtonsoft.Json;
using ShiftOS.Objects.ShiftFS;

namespace ShiftOS.WinForms.Applications
{
    [WinOpen("desktop_widgets")]
    [Launcher("Widget Manager", true, "al_widget_manager", "Customization")]
    [DefaultTitle("Widget Manager")]
    [StpContents("Desktop Widgets", "SuperDesk Inc.", "desktop;wm_free_placement;shifter")]
    public partial class WidgetManagerFrontend : UserControl, IShiftOSWindow
    {
        public WidgetManagerFrontend()
        {
            InitializeComponent();
        }

        Dictionary<string, WidgetDetails> temp_details = null;

        public void SetupUI()
        {
            flbody.Controls.Clear();
            if(temp_details == null)
                temp_details = new Dictionary<string, WinForms.WidgetDetails>();
            foreach(var widgetType in WidgetManager.GetAllWidgetTypes())
            {
                
                var details = WidgetManager.LoadDetails(widgetType.Value);
                if (temp_details.ContainsKey(widgetType.Key.ToString()))
                    details = temp_details[widgetType.Key.ToString()];
                else
                    temp_details.Add(widgetType.Key.ToString(), details);
                var cbox = new CheckBox();
                cbox.Checked = details.IsVisible;
                cbox.Size = new Size(32, 32);
                cbox.CheckedChanged += (o, a) =>
                {
                    details.IsVisible = cbox.Checked;
                };
                flbody.Controls.Add(cbox);
                cbox.Show();
                var title = new Label();
                title.Text = widgetType.Key.Name;
                title.AutoSize = true;
                title.Tag = "header3";
                ControlManager.SetupControl(title);
                flbody.Controls.Add(title);
                title.Show();
                var desc = new Label();
                desc.Text = widgetType.Key.Description;
                flbody.Controls.Add(desc);
                flbody.SetFlowBreak(desc, true);
                flbody.SetFlowBreak(title, true);
                desc.Show();
            }
        }

        public void OnLoad()
        {
            SetupUI();
        }

        public void OnSkinLoad()
        {
            SetupUI();
        }

        public bool OnUnload()
        {
            return false;
        }

        public void OnUpgrade()
        {
            SetupUI();
        }

        private void btnapply_Click(object sender, EventArgs e)
        {
            Utils.WriteAllText(Paths.GetPath("widgets.dat"), JsonConvert.SerializeObject(temp_details));
            Desktop.CurrentDesktop.SetupDesktop();
        }

        private void btnexport_Click(object sender, EventArgs e)
        {
            FileSkimmerBackend.GetFile(new[] { ".wid" }, FileOpenerStyle.Save, (path) =>
             {
                 Utils.WriteAllText(path, JsonConvert.SerializeObject(temp_details));
             });
        }

        private void btnimport_Click(object sender, EventArgs e)
        {
            FileSkimmerBackend.GetFile(new[] { ".wid" }, FileOpenerStyle.Open, (path) =>
            {
                temp_details = JsonConvert.DeserializeObject<Dictionary<string, WidgetDetails>>(Utils.ReadAllText(path));
                SetupUI();
            });

        }

        private void btnloaddefault_Click(object sender, EventArgs e)
        {
            temp_details.Clear();
            foreach(var type in WidgetManager.GetAllWidgetTypes())
            {
                temp_details.Add(type.Key.ToString(), new WidgetDetails
                {
                    IsVisible = false,
                    Location = new Point(-1, -1)
                });
            }
            SetupUI();
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            AppearanceManager.Close(this);
        }
    }
}
