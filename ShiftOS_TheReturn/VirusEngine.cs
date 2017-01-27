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
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static ShiftOS.Objects.ShiftFS.Utils;


namespace ShiftOS.Engine
{
    public static class VirusEngine
    {
        public static void InfectFile(string file, string virusid)
        {
            var infected = new List<string>();
            var hData = GetHeaderText(file);

            if (hData == "")
            {
                infected.Add(virusid);
            }
            else
            {
                infected = JsonConvert.DeserializeObject<List<string>>(hData);
                if (!infected.Contains(virusid))
                    infected.Add(virusid);
            }

            SetHeaderText(file, JsonConvert.SerializeObject(infected));
        }

        public static void DisinfectFile(string file, int threatlevel)
        {
            var infected = new List<string>();
            var hData = GetHeaderText(file);

            if (hData != "")
            {
                infected = JsonConvert.DeserializeObject<List<string>>(hData);
                for (int i = 0; i < infected.Count; i++)
                {
                    string[] splitID = infected[i].Split('.');
                    int th = Convert.ToInt32(splitID[splitID.Length - 1]);
                    if (th <= threatlevel)
                    {
                        infected.RemoveAt(i);
                    }
                }
            }

            SetHeaderText(file, JsonConvert.SerializeObject(infected));
        }

        internal static string[] FindAllVirusesInFile(string file)
        {
            string hdata = GetHeaderText(file);
            return (hdata != "") ? new string[0] : JsonConvert.DeserializeObject<string[]>(hdata);
        }

    }

    public abstract class Virus
    {
        /// <summary>
        /// Inject the virus into system memory by running it.
        /// </summary>
        public abstract void Activate();

        /// <summary>
        /// Terminate the virus.
        /// </summary>
        public abstract void Deactivate();

        public abstract int ThreatLevel { get; }

        public abstract string Signature { get; }

        public abstract string Type { get; }
    }
}
