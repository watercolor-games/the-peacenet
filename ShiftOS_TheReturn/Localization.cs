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
    public interface ILanguageProvider
    {
        List<string> GetJSONTranscripts();
        void WriteDefaultTranscript();
        string GetCurrentTranscript();
        string[] GetAllLanguages();
    }

    public static class Localization
    {
        private static ILanguageProvider _provider = null;

        public static string[] GetAllLanguages()
        {
            if(_provider == null)
            {
                return JsonConvert.DeserializeObject<string[]>(Properties.Resources.languages);
            }
            else
            {
                return _provider.GetAllLanguages();
            }
        }

        public static void SetupTHETRUEDefaultLocals()
        {
            if (_provider == null)
            {
                var lines = Properties.Resources.strings_en;
                var path = "english.local";
                Utils.WriteAllText(Paths.GetPath(path), lines);
            }
            else
            {
                _provider.WriteDefaultTranscript();
            }
        }

        public static void SetupDefaultLocals(string lines, string path)
        {
            Utils.WriteAllText(Paths.GetPath(path), lines);

        }


        /// <summary>
        /// Takes in a string and parses localization blocks into text blocks in the current language.
        /// </summary>
        /// <example>"{CODEPOINTS}: 0" will come out as "Codepoints: 0" if the current language is english.</example>
        /// <param name="original">The string to parse</param>
        /// <returns>The parsed string.</returns>
        /// 
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
                localizationStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(Utils.ReadAllText(Paths.GetPath("english.local")));
            }

            foreach (var kv in localizationStrings)
            {
                original = original.Replace(kv.Key, kv.Value);
            }

            List<string> orphaned = new List<string>();
            if (Utils.FileExists("0:/dev_orphaned_lang.txt"))
            {
                orphaned = JsonConvert.DeserializeObject<List<string>>(Utils.ReadAllText("0:/dev_orphaned_lang.txt"));
            }


            int start_index = 0;
            int length = 0;
            bool indexing = false;

            foreach (var c in original)
            {
                if (c == '{')
                {
                    start_index = original.IndexOf(c);
                    indexing = true;
                }

                if (indexing == true)
                {
                    length++;
                    if (c == '}')
                    {
                        indexing = false;
                        string o = original.Substring(start_index, length);
                        if (!orphaned.Contains(o))
                        {
                            orphaned.Add(o);
                        }
                        start_index = 0;
                        length = 0;
                    }
                }
            }

            if (orphaned.Count > 0)
            {
                Utils.WriteAllText("0:/dev_orphaned_lang.txt", JsonConvert.SerializeObject(orphaned, Formatting.Indented));
            }

            //string original2 = Parse(original);

            string usernameReplace = "";
            string domainReplace = "";

            if (SaveSystem.CurrentSave != null)
            {
                usernameReplace = SaveSystem.CurrentSave.Username;
                domainReplace = SaveSystem.CurrentSave.SystemName;
            }

            string namespaceReplace = "";
            string commandReplace = "";

            if (TerminalBackend.latestCommmand != "" && TerminalBackend.latestCommmand.IndexOf('.') > -1)
            {
                namespaceReplace = TerminalBackend.latestCommmand.Split('.')[0];
                commandReplace = TerminalBackend.latestCommmand.Split('.')[1];
            }

            Dictionary<string, string> defaultReplace = new Dictionary<string, string>() {
                {"%username", usernameReplace},
                {"%domain", domainReplace},
                {"%ns", namespaceReplace},
                {"%cmd", commandReplace},
                {"%cp", SaveSystem.CurrentSave?.Codepoints.ToString() },
            };

            foreach (KeyValuePair<string, string> replacement in replace)
            {
                original = original.Replace(replacement.Key, Parse(replacement.Value));
            }

            foreach (KeyValuePair<string, string> replacement in defaultReplace)
            {
                original = original.Replace(replacement.Key, replacement.Value);
            }

            return original;
        }

        public static void RegisterProvider(ILanguageProvider p)
        {
            _provider = p;
        }
    }
}
