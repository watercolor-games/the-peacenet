using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using cutscene = Plex.Engine.Cutscene;
namespace Peacenet.Cutscene
{
    public class PrologueCutscene : cutscene.Cutscene
    {
        private SoundEffectInstance _bgm = null;

        public override string Name => "m00_predeath";

        public override void Load(ContentManager content)
        {
            _bgm = content.Load<SoundEffect>("Audio/Cutscene/Intro/Predeath").CreateInstance();

        }

        public override void OnPlay()
        {
            _bgm.Play();
        }

        public override void Update(GameTime time)
        {
            if (_bgm.State != SoundState.Playing)
                NotifyFinished();


            base.Update(time);
        }
    }
}
