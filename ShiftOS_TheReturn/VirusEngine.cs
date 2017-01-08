using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ShiftOS.Objects.ShiftFS.Utils;


namespace ShiftOS.Engine
{
    public static class VirusEngine
    {
        private static List<Virus> _infections = new List<Virus>();

        public static void Add(Virus virus)
        {
            _infections.Add(virus);
        }

        public static void InfectFile(string file, string virusid)
        {
            string existing = "";

            if(probeFile(file, out existing) == true)
            {
                existing = existing.Replace("<<VIRUSENGINE_HEAD>>", "").Replace("<<VIRUSENGINE_END>>", "");

                existing += ";" + virusid;

                existing = "<<VIRUSENGINE_HEAD>>" + existing + "<<VIRUSENGINE_END>>";

                string c = ReadAllText(file);

                string temp = "";

                if(probeFile(file, out temp) == true)
                {
                    c = c.Replace(temp, existing);
                }
                else
                {
                    c = existing + c;
                }
                WriteAllText(file, c);
                return;
            }
            else
            {
                existing = "<<VIRUSENGINE_HEAD>>" + virusid + "<<VIRUSENGINE_END>>";
                string c = ReadAllText(file);
                c = existing + c;
                WriteAllText(file, c);
                return;
            }


        }

        internal static string[] FindAllVirusesInFile(string file)
        {
            string existing = "";
            if(probeFile(file, out existing) == true)
            {
                existing = existing.Replace("<<VIRUSENGINE_HEAD>>", "").Replace("<<VIRUSENGINE_END>>", "");
                return existing.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries); ;
            }
            else
            {
                return null;
            }
        }

        private static bool probeFile(string file, out string existing)
        {
            int startIndex = 0;
            int endIndex = 0;

            bool found = false;

            string contents = ReadAllText(file);

            for(int i = 0; i < contents.Length; i++)
            {
                string end = "<<VIRUSENGINE_END>>";
                try
                {
                    if (contents.Substring(i, end.Length) == end)
                    {
                        endIndex = i + end.Length;
                        found = true;
                        break;
                    }
                }
                catch { }
            }

            if (found == false) {
                existing = "<<VIRUSENGINE_HEAD>><<VIRUSENGINE_END>>";
            }
            else
            {
                existing = contents.Substring(startIndex, endIndex);
            }
            return found;
        }

        public static void Infect(string virusid)
        {
            string[] id_split = virusid.Split('.');

            foreach(var v in _infections)
            {
                if(v.Type == id_split[0])
                {
                    if(v.Signature == id_split[1])
                    {
                        if(v.ThreatLevel == Convert.ToInt32(id_split[2]))
                        {
                            var t = new Thread(new ThreadStart(() =>
                            {
                                v.Activate();
                            }));
                            t.IsBackground = true;
                            t.Start();
                            return;
                        }
                    }
                }
            }
            throw new Exception("Virus not found in the system.");
        }

        internal static void ProbeFileRaw(string filePath, out string existing)
        {
            probeFile(filePath, out existing);
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
