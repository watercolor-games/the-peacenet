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
using System.Reflection;

namespace ShiftOS.WinForms.Applications
{
    [WinOpen("about")]
    [Launcher("About", false, null, "Accessories")]
    [DefaultTitle("About ShiftOS")]
    public partial class About : UserControl, IShiftOSWindow
    {
        public About()
        {
            InitializeComponent();
        }

        public void SetupUI()
        {
            lbshiftit.Top = label1.Top + label1.Height;

            lbaboutdesc.Text = $@"ShiftOS
Copyright (c) 2017-{DateTime.Now.Year} Michael VanOverbeek and ShiftOS devs

Engine version: Milestone 3, 1.0 Beta Series (Developer mode ON)
Frontend version: 1.0 Beta 1.2
Multi-user domain version: 1.0 Rolling-Release

Music courtesy of Selulance. Listen to the Fractal Forest album here:
https://www.youtube.com/watch?v=LB5jAYDL3VU&t=913s

Special thanks to Philip Adams, the original creator of ShiftOS for helping us grow our community of amazing Shifters by featuring us on the YouTube Millionaire series and advertising us throughout various other series ran by him.

Also, thanks to Rylan Arbour, Victor Tran and the other community moderators and administrators for helping us keep the community peaceful.

Lastly, a huge special thanks to the community themselves - for testing, debugging, fixing, reporting bugs for, and enjoying our game even through its many failures, successes, revamps, etc. You guys are the reason we develop the game!";
        }

        public string GetEngineVersion()
        {
            foreach(var attr in typeof(IShiftOSWindow).Assembly.GetCustomAttributes(true))
            {
                if(attr is AssemblyVersionAttribute)
                {
                    var ver = attr as AssemblyVersionAttribute;
                    return ver.Version;
                }
            }
            return "Unknown";
        }

        public string GetFrontendVersion()
        {
            foreach (var attr in this.GetType().Assembly.GetCustomAttributes(true))
            {
                if (attr is AssemblyVersionAttribute)
                {
                    var ver = attr as AssemblyVersionAttribute;
                    return ver.Version;
                }
            }
            return "Unknown";
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
            
        }
    }
}
