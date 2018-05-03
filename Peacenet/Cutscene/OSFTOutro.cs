using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cutscene = Plex.Engine.Cutscene;

namespace Peacenet.Cutscene
{
    public class OSFTOutro : cutscene.Cutscene
    {
        public override string Name => "sto_osft_outro";

        private SoundEffectInstance _osftIntro = null;

        public override void Load(ContentManager content)
        {
            _osftIntro = content.Load<SoundEffect>("Audio/SentienceTriesOut/11").CreateInstance();
            base.Load(content);
        }

        public override void OnPlay()
        {
            _osftIntro.Play();
            base.OnPlay();
        }

        public override void Update(GameTime time)
        {
            if (_osftIntro.State != SoundState.Playing)
                NotifyFinished();
            base.Update(time);
        }
    }

}
