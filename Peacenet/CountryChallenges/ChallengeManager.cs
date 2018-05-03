using Plex.Engine.Interfaces;
using Plex.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;

namespace Peacenet.CountryChallenges
{
    public class ChallengeManager : IEngineComponent
    {
        private List<ICountryChallenge> _challenges = new List<ICountryChallenge>();

        [Dependency]
        private Plexgate _plexgate = null;

        public void Initiate()
        {
            Logger.Log("Loading country challenges...");
            foreach(var type in ReflectMan.Types.Where(x=>x.GetInterfaces().Contains(typeof(ICountryChallenge))))
            {
                var challenge = (ICountryChallenge)_plexgate.New(type);
                _challenges.Add(challenge);
            }
            Logger.Log($"{_challenges.Count} challenges loaded.");
        }

        public ICountryChallenge[] GetChallenges(Country country)
        {
            return _challenges.Where(x => x.Country == country).ToArray();
        }
    }
}
