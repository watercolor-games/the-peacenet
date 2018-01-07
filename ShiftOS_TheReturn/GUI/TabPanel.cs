using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Graphics;

namespace Plex.Engine.GUI
{
    /// <summary>
    /// A control capable of displaying its children in separate clickable tabs.
    /// </summary>
    public class TabPanel : Control
    {
        private int _selectedPage = -1;

        private bool _needsLayout = true;

        private ScrollView _scroller = null;

        private List<TabPage> _pages = new List<TabPage>();

        /// <summary>
        /// Retrieves all the pages of this tab panel.
        /// </summary>
        public TabPage[] Pages
        {
            get
            {
                return _pages.ToArray();
            }
        }

        /// <inheritdoc/>
        public override void AddChild(Control child)
        {
            bool makeActive = Pages.Length == 0;
            if (!(child is TabPage))
                throw new InvalidOperationException("Child element must be a TabPage.");
            base.AddChild((child as TabPage).Button);
            _pages.Add(child as TabPage);
            (child as TabPage).Button.WidthChanged += Button_WidthChanged;
            (child as TabPage).Button.HeightChanged += Button_WidthChanged;
            child.WidthChanged += Button_WidthChanged;
            child.HeightChanged += Button_WidthChanged;

            (child as TabPage).Activated += TabPanel_Activated;
            _needsLayout = true;
            if(makeActive)
            {
                (child as TabPage).Active = true;
            }
            else
            {
                (child as TabPage).Active = false;
            }
        }

        private void Button_WidthChanged(object sender, EventArgs e)
        {
            _needsLayout = true;
        }

        private void TabPanel_Activated(object sender, EventArgs e)
        {
            if(_selectedPage != -1)
            {
                Pages[_selectedPage].Active = false;
            }
            _selectedPage = Array.IndexOf(Pages, sender);
            _needsLayout = true;
        }

        /// <inheritdoc/>
        public override void RemoveChild(Control child)
        {
            if (!(child is TabPage))
                throw new InvalidOperationException("Child element must be a TabPage.");
            base.RemoveChild((child as TabPage).Button);
            _pages.Remove(child as TabPage);
            (child as TabPage).Activated -= TabPanel_Activated;
            (child as TabPage).Button.WidthChanged -= Button_WidthChanged;
            (child as TabPage).Button.HeightChanged -= Button_WidthChanged;
            child.WidthChanged -= Button_WidthChanged;
            child.HeightChanged -= Button_WidthChanged;
            if (Pages.Length == 0)
            {
                _selectedPage = -1;
            }
            else
            {
                if (_selectedPage >= Pages.Length)
                {
                    _selectedPage = 0;
                    Pages[_selectedPage].Active = true;
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            if (_needsLayout)
            {
                if(_scroller == null)
                {
                    _scroller = new ScrollView();
                    base.AddChild(_scroller);
                }
                _scroller.Clear();
                int buttonBarHeight = 6;
                int buttonBarY = 3;
                int buttonBarX = 3;
                foreach(var page in Pages)
                {
                    if(page.Button.X + page.Button.Width > (Width - 6))
                    {
                        buttonBarY += page.Button.Height;
                        buttonBarX = 3;
                    }
                    page.Button.Y = buttonBarY;
                    page.Button.X = buttonBarX;
                    buttonBarX += page.Button.Width + 3;
                    buttonBarHeight = Math.Max(buttonBarHeight, buttonBarY + page.Button.Height+3);
                    page.Visible = page.Active;
                }
                if(_selectedPage != -1)
                {
                    var page = Pages[_selectedPage];
                    page.Width = Width - 6;
                    page.AutoSize = true;
                    _scroller.AddChild(page);
                }
                _scroller.X = 3;
                _scroller.Y = buttonBarHeight;
                _scroller.Height = (Height - buttonBarHeight) - 3;
                _needsLayout = false;
            }
            base.OnUpdate(time);
        }
    }

    /// <summary>
    /// A <see cref="Panel"/> suitable for use in a <see cref="TabPanel"/> control.  
    /// </summary>
    public class TabPage : Panel
    {
        private TabButton _tabButton = new TabButton();
        
        /// <summary>
        /// Retrieves the associated <see cref="TabButton"/> for this page 
        /// </summary>
        public TabButton Button
        {
            get
            {
                return _tabButton;
            }
        }

        /// <summary>
        /// Gets or sets the name of this tab.
        /// </summary>
        public string Name
        {
            get
            {
                return _tabButton.Text;
            }
            set
            {
                _tabButton.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon of this tab.
        /// </summary>
        public Texture2D Image
        {
            get
            {
                return _tabButton.Image;
            }
            set
            {
                if (value == null)
                    _tabButton.ShowImage = false;
                else
                    _tabButton.ShowImage = true;
                _tabButton.Image = value;
            }
        }

        /// <summary>
        /// Gets or sets whether this tab is active.
        /// </summary>
        public bool Active
        {
            get
            {
                return _tabButton.Active;
            }
            set
            {
                _tabButton.Active = value;
            }
        }

        /// <summary>
        /// Occurs when the tab is activated.
        /// </summary>
        public event EventHandler Activated;
        /// <summary>
        /// Occurs when the tab is deactivated.
        /// </summary>
        public event EventHandler Deactivated;

        /// <summary>
        /// Creates a new instance of the <see cref="TabPage"/> control. 
        /// </summary>
        public TabPage()
        {
            AutoSize = true;
            _tabButton.Activated += (o, a) =>
            {
                Activated?.Invoke(this, EventArgs.Empty);
            };
            _tabButton.Deactivated += (o, a) =>
            {
                Deactivated?.Invoke(this, EventArgs.Empty);
            };
        }
    }

    /// <summary>
    /// A special <see cref="Button"/> useful for the tabs of a <see cref="TabPanel"/>.  
    /// </summary>
    public sealed class TabButton : Button
    {
        private bool _active = false;

        /// <summary>
        /// Gets or sets whether the tab button is active or not
        /// </summary>
        public bool Active
        {
            get
            {
                return _active;
            }
            set
            {
                if (_active == value)
                    return;
                _active = value;
                Invalidate(true);
                if (_active)
                    Activated?.Invoke(this, EventArgs.Empty);
                else
                    Deactivated?.Invoke(this, EventArgs.Empty);

            }
        }

        /// <summary>
        /// Occurs when the tab button is activated.
        /// </summary>
        public event EventHandler Activated;

        /// <summary>
        /// Occurs when the tab button is deactivated.
        /// </summary>
        public event EventHandler Deactivated;

        /// <summary>
        /// Creates a new instance of the <see cref="TabButton"/> control. 
        /// </summary>
        public TabButton() : base()
        {
            Click += (o, a) =>
            {
                Active = true;
            };
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Themes.UIButtonState state = Themes.UIButtonState.Idle;
            if (ContainsMouse || _active)
                state = Themes.UIButtonState.Hover;
            if (LeftMouseState == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                state = Themes.UIButtonState.Pressed;
            Theme.DrawButton(gfx, Text, Image, state, ShowImage, ImageRect, TextRect);
        }

    }
}
