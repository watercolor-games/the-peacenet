using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    public interface IShiftnetSite
    {
        void Setup();
        void OnSkinLoad();
        void OnUpgrade();

        event Action<string> GoToUrl;
        event Action GoBack;
    }

    /// <summary>
    /// Marks a shiftnet site as a fundamental, and will make it display on the homepage.
    /// </summary>
    public class ShiftnetFundamentalAttribute : Attribute
    {

    }

    public interface IShiftnetClient
    {
        void NavigateToUrl(string url);
        void RefreshSite();
    }

    public class ShiftnetSiteAttribute : Attribute
    {
        public ShiftnetSiteAttribute(string url, string name, string description)
        {
            Url = url;
            Name = name;
            Description = description;
        }

        public string Url { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
    }
}
