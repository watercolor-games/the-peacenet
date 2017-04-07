using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Objects.Unite
{
    public class Download
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Changelog { get; set; }
        public string DownloadUrl { get; set; }
        public bool Obsolete { get; set; }
        public DateTime PostDate { get; set; }
        public string ReleasedBy { get; set; }
        public string DevUpdateId { get; set; }
        public string ScreenshotUrl { get; set; }
        public bool IsStable { get; set; }
    }
}
