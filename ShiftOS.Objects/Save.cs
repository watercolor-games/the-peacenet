using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Objects
{
    //Better to store this stuff server-side so we can do some neat stuff with hacking...
    public class Save
    {
        public string Username { get; set; }
        public int Codepoints { get; set; }
        public Dictionary<string, bool> Upgrades { get; set; }
        public int StoryPosition { get; set; }
        public string Language { get; set; }
        public string MyShop { get; set; }
        public List<string> CurrentLegions { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public int Revision { get; set; }

        public string Password { get; set; }
        public string SystemName { get; set; }

        private dynamic _settings = new SettingsObject();

        public dynamic Settings
        {
            get
            {
                return _settings;
            }
        }

        public int CountUpgrades()
        {
            int count = 0;
            foreach (var upg in Upgrades)
            {
                if (upg.Value == true)
                    count++;
            }
            return count;
        }
    }

    public class SettingsObject : DynamicObject
    {
        private Dictionary<string, object> _settings = null;

        public SettingsObject()
        {
            _settings = new Dictionary<string, object>();
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _settings.Keys.ToArray();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_settings.ContainsKey(binder.Name))
            {
                result = _settings[binder.Name];
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (_settings.ContainsKey(binder.Name))
            {
                _settings[binder.Name] = value;
            }
            else
            {
                _settings.Add(binder.Name, value);
            }

            return true;
        }
    }
}
