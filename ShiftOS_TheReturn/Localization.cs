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

using Newtonsoft.Json;
using ShiftOS.Objects.ShiftFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Engine
{
    //define a whole bunch of things that are needed 
    public interface ILanguageProvider
    {
        List<string> GetJSONTranscripts();
        void WriteDefaultTranscript();
        void WriteTranscript();
        string GetCurrentTranscript();
        string[] GetAllLanguages();
    }

    public static class Localization
    {
        private static ILanguageProvider _provider = null;
        private static string _languageid = null;

        public static string[] GetAllLanguages()
        {
            if(_provider == null)
            {
                return JsonConvert.DeserializeObject<string[]>(Properties.Resources.languages); //collect all the languages availible
            }
            else
            {
                return _provider.GetAllLanguages(); //also collect all the languages avalible but from a specific provider this time
            }
        }

        //if no local selected, english will be loaded
        public static void SetupTHETRUEDefaultLocals()
        {
            if (_provider == null)
            {
                var lines = Properties.Resources.strings_en;
                var path = "english.local";
                Utils.WriteAllText(Paths.GetPath(path), lines);
            }
            else if (SaveSystem.CurrentSave == null)
            {
                var lines = Properties.Resources.strings_en;
                var path = "english.local";
                Utils.WriteAllText(Paths.GetPath(path), lines);
            }
            else
            {
                _provider.WriteTranscript();
            }
        }

        // ignore this not really setup of default no no zone
        public static void SetupDefaultLocals(string lines, string path)
        {
            Utils.WriteAllText(Paths.GetPath(path), lines);

        }


        // Takes in a string and parses localization blocks into text blocks in the current language.
        // example: "{CODEPOINTS}: 0" will come out as "Codepoints: 0" if the current language is english
        public static string Parse(string original)
        {
            return Parse(original, new Dictionary<string, string>());
        }


        public static string Parse(string original, Dictionary<string, string> replace)
        {
            Dictionary<string, string> localizationStrings = new Dictionary<string, string>();


            try
            {
                localizationStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(_provider.GetCurrentTranscript());
            }
            catch
            {
                localizationStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(Utils.ReadAllText(Paths.GetPath("english.local"))); //if no provider fall back to english
            }

            foreach (var kv in localizationStrings.Where(x=>original.Contains(x.Key)))
            {
                original = original.Replace(kv.Key, kv.Value); // goes through and replaces all the localization blocks
            }

            //string original2 = Parse(original);

            string usernameReplace = "";
            string domainReplace = "";

            // if the user has saved then store their username and systemname in these string variables please
            if (SaveSystem.CurrentSave != null)
            {
                try
                {
                    usernameReplace = SaveSystem.CurrentUser.Username;
                }
                catch
                {
                        usernameReplace = "user";
                }

                try
                {
                    domainReplace = SaveSystem.CurrentSave.SystemName;
                }
                catch
                {
                    domainReplace = "system";
                }
                
            }

            string namespaceReplace = "";
            string commandReplace = "";

            // if the user did a command in the terminal and it had a period in it then split it up into the part before the period and the part after and then store them into these two string variables please 
            if (TerminalBackend.latestCommmand != "" && TerminalBackend.latestCommmand.IndexOf('.') > -1)
            {
                namespaceReplace = TerminalBackend.latestCommmand.Split('.')[0];
                commandReplace = TerminalBackend.latestCommmand.Split('.')[1];
            }

            // if you see these then replace them with what you need to
            Dictionary<string, string> defaultReplace = new Dictionary<string, string>() {
                {"%username", usernameReplace},
                {"%domain", domainReplace},
                {"%ns", namespaceReplace},
                {"%cmd", commandReplace},
#if LOCALIZE_CODEPOINTS
                { "%cp", SaveSystem.CurrentSave?.Codepoints.ToString() },
#endif
            };

            // actually do the replacement
            foreach (KeyValuePair<string, string> replacement in replace.Where(x => original.Contains(x.Key)))
            {
                original = original.Replace(replacement.Key, Parse(replacement.Value));
            }

            // do the replacement but default
            foreach (KeyValuePair<string, string> replacement in defaultReplace.Where(x => original.Contains(x.Key)))
            {
                original = original.Replace(replacement.Key, replacement.Value);
            }

            return original; // returns the now replaced string
        }

        // a few things are defined here
        public static void RegisterProvider(ILanguageProvider p)
        {
            _provider = p;
        }

        public static void SetLanguageID(string id)
        {
            _languageid = id;
        }

        public static string GetLanguageID()
        {
            return _languageid;
        }
    }
}
