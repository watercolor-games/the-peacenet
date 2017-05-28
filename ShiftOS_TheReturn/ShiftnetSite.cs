using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    /// <summary>
    /// Interface for creating a Shiftnet website.
    /// </summary>
    public interface IShiftnetSite
    {
        /// <summary>
        /// Called when the page is loaded. Perform data population here.
        /// </summary>
        void Setup();

        /// <summary>
        /// Occurs when a ShiftOS skin is loaded.
        /// </summary>
        void OnSkinLoad();

        /// <summary>
        /// Occurs when a Shiftorium upgrade is installed.
        /// </summary>
        void OnUpgrade();

        /// <summary>
        /// Invoke this to navigate the parent browser to a specified Shiftnet URL.
        /// </summary>
        event Action<string> GoToUrl;

        /// <summary>
        /// Invoke this to tell the parent browser to navigate to the previous page.
        /// </summary>
        event Action GoBack;
    }

    /// <summary>
    /// Marks a shiftnet site as a fundamental, and will make it display on the homepage.
    /// </summary>
    public class ShiftnetFundamentalAttribute : Attribute
    {

    }

    /// <summary>
    /// Interface for creating a Shiftnet client.
    /// </summary>
    public interface IShiftnetClient
    {
        /// <summary>
        /// Navigates the client to a specified Shiftnet URL.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        void NavigateToUrl(string url);

        /// <summary>
        /// Refreshes the current page.
        /// </summary>
        void RefreshSite();
    }

    /// <summary>
    /// Marks this class as a Shiftnet website.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple =false)]
    public class ShiftnetSiteAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ShiftnetSiteAttribute"/> class. 
        /// </summary>
        /// <param name="url">The URL that links to this site</param>
        /// <param name="name">The name of this site</param>
        /// <param name="description">The description of this site</param>
        public ShiftnetSiteAttribute(string url, string name, string description)
        {
            Url = url;
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Gets the Shiftnet URL for this site.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Gets the name of this website.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the description of this website.
        /// </summary>
        public string Description { get; private set; }
    }
}
