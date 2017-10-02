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
                if (vol.Root == null)
                    Console.WriteLine("vol.Root is null");
                using (var fobj = vol.Root.GetSubdirectory("test", OpenMode.OpenOrCreate).GetSubdirectory("test", OpenMode.OpenOrCreate).OpenFile("test.dat", OpenMode.OpenOrCreate))
                using (var write = new BinaryWriter(fobj))
                    write.Write("Hell Yeah! PlexFAT Working");
            }
            using (var volFobj = File.Open("test.plexfat", FileMode.OpenOrCreate))
            {
                var vol = PlexFAT.FromStream(volFobj);
                foreach (string fname in vol.Root.Contents)
                    Console.WriteLine(fname);
                using (var fobj = vol.Root.GetSubdirectory("test").GetSubdirectory("test").OpenFile("test.dat"))
                using (var read = new BinaryReader(fobj))
                    Console.WriteLine(read.ReadString());
                vol.Root.GetSubdirectory("test").GetSubdirectory("test").Delete("test.dat");
                Console.WriteLine("this should fail:");
                using (var fobj = vol.Root.GetSubdirectory("test").GetSubdirectory("test").OpenFile("test.dat"))
                using (var read = new BinaryReader(fobj))
                    Console.WriteLine(read.ReadString().Replace("Yeah", "Nah").Replace("Working", "Not Working..."));
            }
        }
    }
}
