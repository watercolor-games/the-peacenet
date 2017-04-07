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
using static ShiftOS.Engine.SkinEngine;
using System.Net;
using Newtonsoft.Json;
using System.Reflection;
using System.IO;
using System.IO.Compression;

namespace ShiftOS.WinForms.Applications
{
    [Launcher("Update Manager", true, "al_update_manager", "Networking")]
    [DefaultTitle("Update Manager")]
    [WinOpen("update_manager")]
    public partial class UpdateManager : UserControl, IShiftOSWindow
    {
        public UpdateManager()
        {
            InitializeComponent();
            btnaction.Click += (o, a) => OnActionButtonClick?.Invoke();
        }

        public void OnLoad()
        {
            CheckForUpdate();
        }

        public void CheckForUpdate()
        {
            ConstructHtml(@"ShiftOS is grabbing information on the latest version. Please be patient.");
            pgdownload.Show();
            btnaction.Show();
            lbupdatetitle.Text = "Checking for updates...";
            pgdownload.Value = 0;
            pgdownload.Maximum = 100;

            var wc = new WebClient();

            wc.DownloadProgressChanged += (o, a) =>
            {
                this.Invoke(new Action(() =>
                {
                    pgdownload.Text = "Grabbing update data from multi-user-domain...";
                    pgdownload.Value = a.ProgressPercentage;
                }));
            };

            wc.DownloadStringCompleted += (o, a) =>
            {
                var releases = JsonConvert.DeserializeObject<ShiftOS.Objects.Unite.Download[]>(a.Result);

                var download = releases.OrderByDescending(x => x.PostDate).First();

                var thisBuildDate = Assembly.GetExecutingAssembly().GetBuildDate(TimeZoneInfo.Utc);

                if(download.PostDate > thisBuildDate)
                {
                    this.Invoke(new Action(() =>
                    {
                        //This build is more recent than the currently running ShiftOS client.
                        SetupBuildUpdate(download);
                    }));
                }
                else
                {
                    if (thisBuildDate > download.PostDate)
                    {
                        this.Invoke(new Action(() => ConstructHtml($@"### Unreleased build.

You are currently running an unreleased build of ShiftOS, compiled on {thisBuildDate}, after the latest released version, {download.Name}, compiled on {download.PostDate}.")));
                    }
                    else
                    {
                        this.Invoke(new Action(() => ConstructHtml($@"### Up to date.

You are currently running {download.Name}, compiled on {download.PostDate}.")));

                    }
                    this.Invoke(new Action(() =>
                    {
                        lbupdatetitle.Text = "No update available";
                        pgdownload.Hide();
                        btnaction.Hide();
                    }));
                }
            };

            wc.DownloadStringAsync(new Uri("http://getshiftos.ml/API/Releases?showUnstable=true&showObsolete=false"));
        }

        public void OnSkinLoad()
        {
            ConstructHtml(CurrentMD);
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
            ConstructHtml(CurrentMD);
        }

        public void SetupBuildUpdate(ShiftOS.Objects.Unite.Download download)
        {
            string devUpdate = "";
            if (!string.IsNullOrEmpty(download.DevUpdateId))
            {
                devUpdate = $@"## Development update

<iframe src=""http://youtube.com/embed/{download.DevUpdateId}"" allowfullscreen width=""720"" height=""480""></iframe>
";
            }

            string screenshot = "";

            if(!string.IsNullOrEmpty(download.ScreenshotUrl))
            {
                screenshot = $"<img src=\"http://getshiftos.ml{download.ScreenshotUrl}\" style=\"max-width:720px;width:auto;height:auto;\"/>";
            }

            lbupdatetitle.Text = download.Name;
            string markdown = $@"**Built on {download.PostDate}**

{devUpdate}

{screenshot}

## Changelog

{download.Changelog}";

            ConstructHtml(markdown);

            pgdownload.Value = 0;
            pgdownload.Text = "Waiting.";
            btnaction.Text = "Update";
            btnaction.Show();
            OnActionButtonClick = () =>
            {
                pgdownload.Show();
                var wc = new WebClient();
                wc.DownloadProgressChanged += (o, a) =>
                {
                    this.Invoke(new Action(() =>
                    {
                        pgdownload.Text = "Downloading " + download.Name + "..."; 
                        pgdownload.Value = a.ProgressPercentage;
                    }));
                };

                wc.DownloadDataCompleted += (o, a) =>
                {
                    if (Directory.Exists("updater-work"))
                        Directory.Delete("updater-work", true);
                    Directory.CreateDirectory("updater-work");

                    string temp_guid = Guid.NewGuid().ToString();

                    File.WriteAllBytes($"{temp_guid}.zip", a.Result);

                    ZipFile.ExtractToDirectory($"{temp_guid}.zip", "updater-work");

                    File.Delete($"{temp_guid}.zip");

                    //Start the updater helper.
                    System.Diagnostics.Process.Start("ShiftOS.Updater.exe");

                    //Now we stop the engine.
                    TerminalBackend.InvokeCommand("sos.shutdown");

                };

                wc.DownloadDataAsync(new Uri($"http://getshiftos.ml{download.DownloadUrl}"));
            };
        }

        public Action OnActionButtonClick = () => { };

        public void ConstructHtml(string markdown)
        {
            CurrentMD = markdown;
            var TerminalForeColor = ControlManager.ConvertColor(SkinEngine.LoadedSkin.TerminalForeColorCC);
            var TerminalBackColor = ControlManager.ConvertColor(SkinEngine.LoadedSkin.TerminalBackColorCC);
            string html = $@"<html>
    <head>
        <style>
            body {{
                background-color: rgb({LoadedSkin.ControlColor.R}, {LoadedSkin.ControlColor.G}, {LoadedSkin.ControlColor.B});
                color: rgb({LoadedSkin.ControlTextColor.R}, {LoadedSkin.ControlTextColor.G}, {LoadedSkin.ControlTextColor.B});
                font-family: ""{LoadedSkin.MainFont.Name}"";
                font-size: {LoadedSkin.MainFont.SizeInPoints}pt;
            }}

            h1 {{
                font-family: ""{LoadedSkin.HeaderFont.Name}"";
                font-size: {LoadedSkin.HeaderFont.SizeInPoints}pt;
            }}

            h2 {{
                font-family: ""{LoadedSkin.Header2Font.Name}"";
                font-size: {LoadedSkin.Header2Font.SizeInPoints}pt;
            }}

            h3 {{
                font-family: ""{LoadedSkin.Header3Font.Name}"";
                font-size: {LoadedSkin.Header3Font.SizeInPoints}pt;
            }}

            pre, code {{
                font-family: ""{LoadedSkin.TerminalFont.Name}"";
                font-size: {LoadedSkin.TerminalFont.SizeInPoints}pt;
                color: rgb({TerminalForeColor.R}, {TerminalForeColor.G}, {TerminalForeColor.B});
                background-color: rgb({TerminalBackColor.R}, {TerminalBackColor.G}, {TerminalBackColor.B});                
            }}
        </style>
    </head>
    <body>
        <markdown/>
    </body>
</html>";

            string body = CommonMark.CommonMarkConverter.Convert(markdown);
            
            html = html.Replace("<markdown/>", body);
            wbstatus.DocumentText = html;
        }

        public string CurrentMD = "";

        private void btnclose_Click(object sender, EventArgs e)
        {
            AppearanceManager.Close(this);
        }
    }

    public static class UpdateHelpers
    {
        /// <summary>
        /// Grabs the build date from the assembly's PE header.
        /// </summary>
        /// <param name="assembly">The assembly to probe.</param>
        /// <param name="target">The target timezone.</param>
        /// <returns>The assembly's build date.</returns>
        public static DateTime GetBuildDate(this Assembly assembly, TimeZoneInfo target = null)
        {
            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }
    }
}
