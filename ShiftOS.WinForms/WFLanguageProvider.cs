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
        public string[] GetAllLanguages()
        {
            return JsonConvert.DeserializeObject<string[]>(Properties.Resources.languages);
        }

        public string GetCurrentTranscript()
        {
            switch (SaveSystem.CurrentSave.Language)
            {
                case "deutsch - in beta":
                    return Properties.Resources.strings_de;
                default:
                    return getDefault();
                    
            }
        }

        public List<string> GetJSONTranscripts()
        {
            var strings = new List<string>();
            strings.Add(Properties.Resources.strings_en);
            strings.Add(Properties.Resources.strings_de);
            return strings;
        }

        public void WriteDefaultTranscript()
        {
            Utils.WriteAllText(Paths.GetPath("english.local"), getDefault());
        }

        private string getDefault()
        {
            return Properties.Resources.strings_en;
        }
    }
}
