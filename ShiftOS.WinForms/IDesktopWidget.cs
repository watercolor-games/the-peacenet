using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Engine;

namespace ShiftOS.WinForms
{
    /// <summary>
    /// Provides base functionality for a ShiftOS desktop widget.
    /// </summary>
    public interface IDesktopWidget
    {
        /// <summary>
        /// Performs routine setup operations to keep the widget up to date.
        /// </summary>
        void Setup();

        /// <summary>
        /// Occurs when a skin is loaded.
        /// </summary>
        void OnSkinLoad();

        /// <summary>
        /// Occurs when a Shiftorium upgrade is installed.
        /// </summary>
        void OnUpgrade();

        /// <summary>
        /// Hides this desktop widget.
        /// </summary>
        void Hide();

        /// <summary>
        /// Shows this desktop widget.
        /// </summary>
        void Show();

        /// <summary>
        /// Gets or sets the location on the desktop that this widget resides.
        /// </summary>
        Point Location { get; set; }

        /// <summary>
        /// Gets or sets this widget's size.
        /// </summary>
        Size Size { get; set; }
    }
}
