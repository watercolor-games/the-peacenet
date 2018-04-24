using Plex.Engine.Saves;
using Peacenet.Server;
using Plex.Engine;
using Plex.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet
{
    public class CountryManager : IEngineComponent
    {
        [Dependency]
        private SaveManager _save = null;

        [Dependency]
        private AsyncServerManager _server = null;

        private Country _currentCountry = Country.Elytrose;

        public event Action CountryChanged;

        public void Initiate()
        {
        }

        public Country CurrentCountry
        {
            get { return _currentCountry; }
            set
            {
                if (_currentCountry == value)
                    return;
                if (!IsCountryUnlocked(value))
                    return;
                _currentCountry = value;
                CountryChanged?.Invoke();
            }
        }

        public bool IsCountryUnlocked(Country country)
        {
            if (_server.Connected == false)
                return false;
            if (_server.IsMultiplayer)
                return true;
            return _save.GetValue<bool>($"country_{(int)country}_unlocked", (country == Country.Elytrose) ? true : false);
        }

        public void UnlockCountry(Country country)
        {
            if (_server.Connected == false)
                return;
            if (_server.IsMultiplayer)
                return;
            _save.SetValue<bool>($"country_{(int)country}_unlocked", true);
        }
    }

    public enum Country
    {
        Valkerie,
        Mejionde,
        Oglowia,
        Riogan,
        Velacrol,
        Sikkim,
        Elytrose
    }
}
