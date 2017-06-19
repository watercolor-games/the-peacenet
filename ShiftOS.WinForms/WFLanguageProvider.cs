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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShiftOS.Engine;
using ShiftOS.Objects.ShiftFS;

namespace ShiftOS.WinForms
{
    public class WFLanguageProvider : ILanguageProvider
    {
        private string resourcesPath
        {
            get
            {
                return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ShiftOS", "languages");
            }
        }

        public string[] GetAllLanguages()
        {
            if (!System.IO.Directory.Exists(resourcesPath))
            {
                System.IO.Directory.CreateDirectory(resourcesPath);
            }
            return System.IO.Directory.GetFiles(resourcesPath).Where(x => x.ToLower().EndsWith(".lang")).ToArray();
            
        }

        public string GetCurrentTranscript()
        {
            string lang = ShiftOS.Objects.UserConfig.Get().Language;
            if (string.IsNullOrWhiteSpace(lang))
            {
                lang = "english";
                var conf = Objects.UserConfig.Get();
                conf.Language = lang;
                System.IO.File.WriteAllText("servers.json", JsonConvert.SerializeObject(conf, Formatting.Indented));
            }
            string foundPath = GetAllLanguages().FirstOrDefault(x => x.ToLower().EndsWith(lang + ".lang"));
            //Update the english file.
            System.IO.File.WriteAllText(System.IO.Path.Combine(resourcesPath, "english.lang"), Properties.Resources.strings_en);
            //Update the french language pack.
            System.IO.File.WriteAllText(System.IO.Path.Combine(resourcesPath, "french.lang"), Properties.Resources.strings_fr);

            if (!System.IO.File.Exists(foundPath))
            {
                lang = "english";
                var conf = Objects.UserConfig.Get();
                conf.Language = lang;
                System.IO.File.WriteAllText("servers.json", JsonConvert.SerializeObject(conf, Formatting.Indented));
                return Properties.Resources.strings_en;
            }
            else
            {
                return System.IO.File.ReadAllText(foundPath);
            }

        }

        public string GetLanguagePath()
        {
            var lang = Objects.UserConfig.Get().Language;
            if(string.IsNullOrWhiteSpace(lang) || !System.IO.File.Exists(System.IO.Path.Combine(resourcesPath, lang + ".lang")))
            {
                lang = "english";
                var conf = Objects.UserConfig.Get();
                conf.Language = lang;
                System.IO.File.WriteAllText("servers.json", JsonConvert.SerializeObject(conf, Formatting.Indented));
                System.IO.File.WriteAllText(System.IO.Path.Combine(resourcesPath, lang + ".lang"), Properties.Resources.strings_en);
            }
            return GetAllLanguages().FirstOrDefault(x => x.ToLower().EndsWith(lang + ".lang"));
        }

        public List<string> GetJSONTranscripts()
        {
            var strings = new List<string>();
            foreach (var path in GetAllLanguages())
                strings.Add(System.IO.File.ReadAllText(path));
            return strings;
        }

        public void WriteDefaultTranscript()
        {
            if (!System.IO.Directory.Exists(resourcesPath))
                System.IO.Directory.CreateDirectory(resourcesPath);

            //Update the english file.
            System.IO.File.WriteAllText(System.IO.Path.Combine(resourcesPath, "english.lang"), Properties.Resources.strings_en);
            //Update the french language pack.
            System.IO.File.WriteAllText(System.IO.Path.Combine(resourcesPath, "french.lang"), Properties.Resources.strings_fr);
        }

        public void WriteTranscript()
        {
            System.IO.File.WriteAllText(GetLanguagePath(), GetCurrentTranscript());
        }

        private string getDefault()
        {
            return Properties.Resources.strings_en;
        }
    }
}
