using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Theming;
namespace Plex.Engine.GUI
{
    public class ComboBox : Control, IListControl
    {
        private ListBox _chooser = null;
        private ScrollView _chooserHolder = null;
        private TextControl _display = null;
        private int _itemsShownAtOnce = 20;

        private List<object> _objects = new List<object>();
        private int _selectedIndex = 0;

        public event Action SelectedItemChanged;

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }

            set
            {
                value = MathHelper.Clamp(value, -1, _objects.Count - 1);
                if (_objects.Count == 0)
                    value = -1;
                if (value == _selectedIndex)
                    return;
                _selectedIndex = value;
                SelectedItemChanged?.Invoke();
            }
        }

        public object SelectedItem
        {
            get
            {
                if (_objects.Count > 0)
                {
                    if (_selectedIndex >= 0 && _selectedIndex < _objects.Count)
                        return _objects[_selectedIndex];
                }
                return null;
            }
        }

        public ComboBox()
        {
            _display = new TextControl();
            _chooserHolder = new ScrollView();
            _chooser = new ListBox();
            _chooserHolder.Visible = false;


            AddControl(_display);
            _chooserHolder.AddControl(_chooser);

            Click += OnClick;
            _display.Click += OnClick;

            _chooser.SelectedIndexChanged += () =>
            {
                if (_chooserHolder.Visible)
                {
                    SelectedIndex = _chooser.SelectedIndex;
                    OnClick();
                }
            };
        }

        protected override void OnLayout(Microsoft.Xna.Framework.GameTime gameTime)
        {
            int pad = 50;
            int margin = 2;

            MinHeight = Math.Max(MinHeight, 26);
            MinWidth = Math.Max(MinWidth, 100);

            string text = "This box is empty.";
            if (_objects.Count > 0)
            {
                if (_selectedIndex > -1 && _selectedIndex < _objects.Count)
                {
                    if (_objects[_selectedIndex] == null)
                        text = "<null>";
                    else
                        text = _objects[_selectedIndex].ToString();
                }
            }

            _display.Text = text;
            _display.AutoSize = true;

            if (AutoSize)
            {
                _display.MaxWidth = (MaxWidth - pad) - (margin * 2);
                _display.MaxHeight = (MaxHeight) - (margin * 2);

                Width = (margin * 2) + _display.Width + pad;
                Height = (margin * 2) + _display.Height;
            }
            else
            {
                _display.MaxWidth = (Width - pad) - (margin * 2);
                _display.MaxHeight = (Height) - (margin * 2);

            }

            _display.X = margin;
            _display.Y = (Height - _display.Height) / 2;

            var pt = PointToScreen(0, Height);
            _chooserHolder.X = pt.X;
            _chooserHolder.Y = pt.Y;
            _chooserHolder.Width = Width;
            _chooserHolder.Height = _chooser.ItemHeight * Math.Min(_itemsShownAtOnce, _objects.Count);

            _chooser.X = 0;
            _chooser.Y = 0;
            _chooser.AutoSize = true;
            _chooser.MinHeight = _chooserHolder.Height;
            _chooser.MinWidth = _chooserHolder.Width;
            _chooser.MaxWidth = _chooserHolder.Width;

            if (_chooserHolder.Visible == true)
            {
                _chooserHolder.Layout(gameTime);
                _chooserHolder.Invalidate();
                UIManager.BringToFront(_chooserHolder);
            }
        }

        private void _resetChooserItems()
        {
            _chooser.ClearItems();
            foreach (var item in _objects)
            {
                string text = "<null>";
                if (item != null)
                    text = item.ToString();
                _chooser.AddItem(text);
            }
            _chooser.SelectedIndex = _selectedIndex;
            _chooser.RequireTextRerender();
            _chooser.Invalidate();
        }

        private void OnClick()
        {
            if (_chooserHolder.Visible == true)
            {
                UIManager.StopHandling(_chooserHolder, false);
                _chooserHolder.Visible = false;
            }
            else
            {
                _resetChooserItems();
                UIManager.AddTopLevel(_chooserHolder);
                _chooserHolder.Visible = true;
                _chooserHolder.RecalculateScrollHeight();
            }
        }

        protected override void OnPaint(GraphicsSubsystem.GraphicsContext gfx, Microsoft.Xna.Framework.Graphics.RenderTarget2D target)
        {
            Theming.ButtonState state = ButtonState.Idle;
            if (ContainsMouse)
                state = ButtonState.MouseHover;
            if (MouseLeftDown || _display.MouseLeftDown || _chooserHolder.Visible)
                state = ButtonState.MouseDown;
            ThemeManager.Theme.DrawButtonBackground(gfx, 0, 0, Width, Height, state);

            int arrowSize = 24;
            int arrowLeft = (Width - 2) - arrowSize;
            int arrowtop = (Height - arrowSize) / 2;
            ThemeManager.Theme.DrawArrow(gfx, arrowLeft, arrowtop, arrowSize, arrowSize, state, ArrowDirection.Bottom);
        }

        public void ClearItems()
        {
            _objects.Clear();
            SelectedIndex = -1;
            if (_chooserHolder.Visible)
                OnClick();
        }

        public void AddItem(object item)
        {
            if (_objects.Contains(item))
               return;
            _objects.Add(item);
        }

        public void RemoveItem(object item)
        {
            if (!_objects.Contains(item))
                return;
            _objects.Remove(item);
            SelectedIndex = SelectedIndex;
        }

        public void RemoveItemAt(int index)
        {
            if (_objects.Count == 0)
                return;
            if (index >= 0 && index < _objects.Count)
            {
                _objects.RemoveAt(index);
                SelectedIndex = SelectedIndex;
            }
        }
    }
}
