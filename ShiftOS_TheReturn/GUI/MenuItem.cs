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
    public class MenuItem : Window
    {
        private MenuHitbox _hitbox = new MenuHitbox();
        private Label _label = new Label();
        private PictureBox _picture = new PictureBox();
        private List<MenuItem> _children = new List<MenuItem>();
        private MenuItem _parent = null;
        private bool _enabled = true;
        private int _presentX, _presentY = 0;

        public event EventHandler Activated;
        
        public bool IsOpen
        {
            get
            {
                return _open;
            }
        }

        public void CloseFromChild()
        {
            if(_open)
            {
                Hide();
                _parent?.CloseFromChild();
                _open = false;
            }
        }

        public bool Enabled
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

        public MenuItem ParentMenuItem
        {
            get
            {
                return _parent;
            }
        }

        public PictureBox PictureBox
        {
            get
            {
                return _picture;
            }
        }

        public Label Label
        {
            get
            {
                return _label;
            }
        }

        public override void Show(int x = -1, int y = -1)
        {
            _open = true;
            base.Show(x, y);
        }

        public override void Hide()
        {
            _open = false;
            base.Hide();
        }

        public Hitbox Hitbox
        {
            get
            {
                return _hitbox;
            }
        }

        private bool _open = false;

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

        public void HideRecursive()
        {
            foreach (var child in _children)
                if(child.IsOpen)
                    child.HideRecursive();
            if(IsOpen)
                Hide();
        }

        public void HideChildren()
        {
            foreach (var child in _children)
                if(child.IsOpen)
                    child.HideRecursive();
        }

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

        public void AddItem(MenuItem item)
        {
            if (_children.Contains(item))
                return;
            _children.Add(item);
        }

        public void RemoveItem(MenuItem item)
        {
            if (!_children.Contains(item))
                return;
            _children.Remove(item);
            RemoveChild(item.Label);
            RemoveChild(item.PictureBox);
            RemoveChild(item.Hitbox);

        }

        public void ClearItems()
        {
            while (_children.Count > 0)
                RemoveItem(_children[0]);
        }

        protected override void OnPaint(GameTime time, GraphicsContext gfx)
        {
            Theme.DrawControlDarkBG(gfx, 0, 0, Width, Height);
            Theme.DrawControlBG(gfx, 2, 2, 18, Height-4);
        }

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
