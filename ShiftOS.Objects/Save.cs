/*
 * Project: Plex
 * 
 * Copyright (c) 2017 Watercolor Games. All rights reserved. For internal use only.
 * 






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
using System.Threading;
using Whoa;

namespace Plex.Objects
{
    //Better to store this stuff server-side so we can do some neat stuff with hacking...
    public class Save
    {
		[Order]
        public List<ViralInfection> ViralInfections { get; set; }
        
        [Order]
        public bool MusicEnabled = true;
        [Order]
        public bool SoundEnabled = true;
        [Order]
        public int MusicVolume = 100;
        
        [Order]
        public string Username = "user";
        
        [Order]
        public bool IsSandbox = false;
        
        [Order]
        public ulong Experience { get; set; }
        
        [Order]
        public Dictionary<string, bool> Upgrades { get; set; }
        [Order]
        public int StoryPosition { get; set; }
        [Order]
        public string Language { get; set; }
        [Order]
        public string SystemName { get; set; }
        [Order]
        public int ShiftnetSubscription { get; set; }
        [Order]
        public Guid ID { get; set; }
        [Order]
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

        /// <summary>
        /// DO NOT MODIFY THIS. EVER. YOU WILL BREAK THE STORYLINE. Let the engine do it's job. 
        /// </summary>
        [Order]
        public string PickupPoint { get; set; }
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

    public class ViralInfection
    {
		[Order]
        public string ID { get; set; }
        [Order]
        public int ThreatLevel { get; set; }
    }

    public class PlexSystem
    {
        public Save SystemDescriptor { get; set; }
        public ShiftFS.Directory RootDirectory { get; set; }
        public DateTime LastAccessed { get; set; }

        public string SavePath { get; set; }
    }
}
