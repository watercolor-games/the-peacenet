/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
        public long Codepoints { get; set; }
        public Dictionary<string, bool> Upgrades { get; set; }
        public int StoryPosition { get; set; }
        public string Language { get; set; }
        public string MyShop { get; set; }
        public List<string> CurrentLegions { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public int Revision { get; set; }

        public bool IsPatreon { get; set; }

        public string Password { get; set; }
        public bool PasswordHashed { get; set; }
        public string SystemName { get; set; }

        private dynamic _settings = new SettingsObject();

        public int ShiftnetSubscription { get; set; }

        public Guid ID { get; set; }

        public bool IsMUDAdmin { get; set; }

        public dynamic Settings
        {
            get
            {
                return _settings;
            }
        }

        public int LastMonthPaid { get; set; }
        public List<string> StoriesExperienced { get; set; }

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
            try
            {
                if (_settings.ContainsKey(binder.Name))
                {
                    _settings[binder.Name] = value;
                }
                else
                {
                    _settings.Add(binder.Name, value);
                }
            }
            catch
            {

            }

            return true;
        }
    }
}
