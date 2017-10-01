using System;
using System.IO;

namespace Plex.Objects.PlexFS
{
    public static class TestProg
    {
        public static void Main(string[] args)
        {
            using (var volFobj = File.Open("test.plexfat", FileMode.OpenOrCreate))
            {
                var vol = PlexFAT.New(volFobj);
                if (vol == null)
                    Console.WriteLine("vol is null");
                if (vol.root == null)
                    Console.WriteLine("vol.root is null");
                using (var fobj = vol.root.GetSubdirectory("test").GetSubdirectory("test").OpenFile("test.dat"))
                using (var write = new BinaryWriter(fobj))
                    write.Write("Hell Yeah! PlexFAT Working");
            }
            using (var volFobj = File.Open("test.plexfat", FileMode.OpenOrCreate))
            {
                var vol = PlexFAT.FromStream(volFobj);
                foreach (string fname in vol.root.Contents)
                    Console.WriteLine(fname);
                using (var fobj = vol.root.GetSubdirectory("test").GetSubdirectory("test").OpenFile("test.dat"))
                using (var read = new BinaryReader(fobj))
                    Console.WriteLine(read.ReadString());
            }
        }
    }
}
