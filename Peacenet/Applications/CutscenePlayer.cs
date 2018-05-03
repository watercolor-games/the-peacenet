#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Engine.GUI;
using Plex.Engine.Cutscene;
using Microsoft.Xna.Framework;

namespace Peacenet.Applications
{
    public class CutscenePlayer : Window
    {
        private ScrollView _scroll = new ScrollView();
        private ListView _list = new ListView();

        [Dependency]
        private CutsceneManager _cutscene = null;

        public CutscenePlayer(WindowSystem _winsys) : base(_winsys)
        {
            Width = 800;
            Height = 600;
            Title = "Select Cutscene";
            SetWindowStyle(WindowStyle.DialogNoDrag);
            AddChild(_scroll);
            _scroll.AddChild(_list);

            foreach(var cutscene in _cutscene.Cutscenes)
            {
                _list.AddItem(new ListViewItem
                {
                    Tag = cutscene,
                    Value = cutscene.Name
                });
            }
            _list.Layout = ListViewLayout.List;
            _list.SelectedIndexChanged += (o, a) =>
            {
                if (_list.SelectedItem != null)
                {
                    _cutscene.Play(_list.SelectedItem.Value);
                    Close();
                }
            };
        }

        protected override void OnUpdate(GameTime time)
        {
            if (_cutscene.IsPlaying)
                Close();
            _scroll.X = 0;
            _scroll.Y = 0;
            _scroll.Width = Width;
            _scroll.Height = Height;
            _list.X = 0;
            _list.Y = 0;
            _list.Width = Width;
            base.OnUpdate(time);
        }


    }
}
#endif