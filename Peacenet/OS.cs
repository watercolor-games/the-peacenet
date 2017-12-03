using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Cutscene;
using Plex.Engine;
using Plex.Engine.Saves;
using Plex.Engine.GUI;

namespace Peacenet
{
    public class OS : IEngineComponent
    {
        [Dependency]
        private CutsceneManager _cutscene = null;
        [Dependency]
        private UIManager _ui = null;
        [Dependency]
        private SaveManager _save = null;
        [Dependency]
        private WindowSystem _winmgr = null;
        [Dependency]
        private InfoboxManager _infobox = null;

        public void Initiate()
        {
        }

        public void OnReady()
        {
            if (!_save.GetValue("hasPlayedIntro", false))
            {
                _cutscene.Play("intro_00", ()=>
                {
                    _infobox.Show("Test", "Does this infobox work?", () =>
                    {
                        var lbl = new Label();
                        lbl.Text = "You played the intro cutscene!";
                        lbl.X = 30;
                        lbl.Y = 30;
                        lbl.AutoSize = true;
                        _ui.Add(lbl);
                    });
                });

            }
        }

        public void OnFrameDraw(GameTime time, GraphicsContext ctx)
        {
        }

        public void OnGameUpdate(GameTime time)
        {
        }

        public void OnKeyboardEvent(KeyboardEventArgs e)
        {
        }

        public void Unload()
        {
        }
    }
}
