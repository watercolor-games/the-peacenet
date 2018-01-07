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
    /// A menu item which can contain child menu items.
    /// </summary>
    public class MenuItem : Window
    {
        private MenuHitbox _hitbox = new MenuHitbox();
        private Label _label = new Label();
        private PictureBox _picture = new PictureBox();
        private List<MenuItem> _children = new List<MenuItem>();
        private MenuItem _parent = null;
        private bool _enabled = true;
        private int _presentX, _presentY = 0;

        /// <summary>
        /// Occurs when the menu item is activated.
        /// </summary>
        public event EventHandler Activated;
        
        /// <summary>
        /// Gets whether the menu itself is open
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return _open;
            }
        }

        internal void CloseFromChild()
        {
            if(_open)
            {
                Hide();
                _parent?.CloseFromChild();
                _open = false;
            }
        }

        /// <inheritdoc/>
        public override bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        /// <summary>
        /// Gets the parent menu item
        /// </summary>
        public MenuItem ParentMenuItem
        {
            get
            {
                return _parent;
            }
        }

        /// <summary>
        /// Gets the menu item's picture box.
        /// </summary>
        public PictureBox PictureBox
        {
            get
            {
                return _picture;
            }
        }

        /// <summary>
        /// Gets the menu item's label.
        /// </summary>
        public Label Label
        {
            get
            {
                return _label;
            }
        }

        /// <inheritdoc/>
        public override void Show(int x = -1, int y = -1)
        {
            _open = true;
            base.Show(x, y);
        }

        /// <inheritdoc/>
        public override void Hide()
        {
            _open = false;
            base.Hide();
        }

        /// <summary>
        /// Gets the menu item's hitbox.
        /// </summary>
        public Hitbox Hitbox
        {
            get
            {
                return _hitbox;
            }
        }

        private bool _open = false;

        /// <summary>
        /// Gets or sets the menu item's text.
        /// </summary>
        public string Text
        {
            get
            {
                return _label.Text;
            }
            set
            {
                _label.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the menu item's image.
        /// </summary>
        public Texture2D Image
        {
            get
            {
                return _picture.Texture;
            }
            set
            {
                _picture.Texture = value;
            }
        }

        internal void HideRecursive()
        {
            foreach (var child in _children)
                if(child.IsOpen)
                    child.HideRecursive();
            if(IsOpen)
                Hide();
        }

        internal void HideChildren()
        {
            foreach (var child in _children)
                if(child.IsOpen)
                    child.HideRecursive();
        }

        /// <inheritdoc/>
        public MenuItem(WindowSystem _winsys) : base(_winsys)
        {
            SetWindowStyle(WindowStyle.NoBorder);
            _hitbox.Click += (o, a) =>
            {
                if(_children.Count > 0)
                {
                    if (_open == false)
                    {
                        _parent?.HideChildren();
                        Show(_presentX, _presentY);
                        _open = true;
                    }
                    else
                    {
                        HideRecursive();
                        _open = false;
                    }
                }
                else
                {
                    Activated?.Invoke(this, EventArgs.Empty);
                    this._parent.CloseFromChild();
                }
            };
        }

        /// <summary>
        /// Add a menu item to this menu
        /// </summary>
        /// <param name="item">The item to add</param>
        public void AddItem(MenuItem item)
        {
            if (_children.Contains(item))
                return;
            _children.Add(item);
        }

        /// <summary>
        /// Remove a menu item from this menu
        /// </summary>
        /// <param name="item">The menu item to remove</param>
        public void RemoveItem(MenuItem item)
        {
            if (!_children.Contains(item))
                return;
            _children.Remove(item);
            RemoveChild(item.Label);
            RemoveChild(item.PictureBox);
            RemoveChild(item.Hitbox);

        }

        /// <summary>
        /// Clear all items from the menu
        /// </summary>
        public void ClearItems()
        {
            while (_children.Count > 0)
                RemoveItem(_children[0]);
        }

        /// <inheritdoc/>
        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
            Theme.DrawControlBG(gfx, 2, 2, 18, Height-4);
        }

        /// <inheritdoc/>
        protected override void OnUpdate(GameTime time)
        {
            int margin = 2;
            int imageWidth = 16;
            int contentWidth = 100;
            if(_children.Count>0)
                contentWidth = _children.OrderBy(x=>x.Label.Width).Last().Label.Width;
            int contentHeight = 0;
            if (_children.Count > 0)
                contentHeight = (_children.Count) * (Math.Max(_children.First().Label.Height, _children.First().PictureBox.Height));

            Width = (margin * 2) + imageWidth + 4 + contentWidth+50;
            Height = (margin * 2) + contentHeight;

            int currentY = 2;
            foreach (var item in _children)
            {
                if (item.Hitbox.Parent != this)
                    AddChild(item.Hitbox);
                if (item.Label.Parent != this)
                    AddChild(item.Label);
                if (item.PictureBox.Parent != this)
                    AddChild(item.PictureBox);
                item.Label.AutoSize = true;
                item.PictureBox.AutoSize = false;
                item.PictureBox.Width = 16;
                item.PictureBox.Height = 16;

                item.PictureBox.X = 2;
                item.Label.Y = currentY;
                item.Label.X = item.PictureBox.X + item.PictureBox.Width + 4;
                item.PictureBox.Y = item.Label.Y + ((item.Label.Height - item.PictureBox.Height) / 2);
                item.Hitbox.Width = Width-4;
                item.Hitbox.Height = (Math.Max(item.PictureBox.Height, item.Label.Height))-4;
                item.Hitbox.X = 2;
                item.Hitbox.Y = item.Label.Y+2;
                item._parent = this;
                item._presentX = Parent.X + Width;
                item._presentY = Parent.Y + currentY;

                currentY += item.Hitbox.Height+4;
            }

            _hitbox.Visible = _enabled;
            _label.Opacity = (_enabled) ? 1 : 0.5F;
            _picture.Opacity = (_enabled) ? 1 : 0.5F;

        }

    }
}
