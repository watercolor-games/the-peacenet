using Microsoft.Xna.Framework;
using Plex.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Applications
{
    [AppLauncher("Grid List Test", "Testing", "Test the new Grid List View control.")]
    public class GridListTest : Window
    {
        private ScrollView _scrollable = new ScrollView();
        private ListView _list = new GridListView();

        public GridListTest(WindowSystem _winsys) : base(_winsys)
        {
            Width = 400;
            Height = 400;
            Title = "Grid List Test";

            AddChild(_scrollable);
            _scrollable.AddChild(_list);
            _list.AutoSize = true;

            for (int i = 1; i <= 100; i++)
                _list.Items.Add(new ListViewItem($"Item {i}", null, null));
        }

        protected override void OnUpdate(GameTime time)
        {
            _scrollable.X = 15;
            _scrollable.Y = 15;
            _scrollable.Width = Width - 30;
            _scrollable.Height = Height - 30;
            _list.Width = _scrollable.Width;
            base.OnUpdate(time);
        }
    }
}
