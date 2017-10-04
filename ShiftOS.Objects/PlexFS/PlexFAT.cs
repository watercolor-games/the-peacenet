using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using static System.Math;

namespace Plex.Objects.PlexFS
{
    // I made my own FAT filesystem, with blackjack and hookers.
    // The original plan was to implement one of Microsoft's complcated
    // FAT filesystems but it turns out they're patent encumbered so I
    // never read any documentation about them.
    // The application of the Plex virtual FS is so limited that I can
    // make a less flexible filesystem design anyway. Every Plex account
    // gets 512 MB of storage space. So, I put the sector size for every
    // image at 8,192 bytes (8 KB) thus causing there to be 65,536 of
    // them and making them all addressable by a ushort.
    // The FAT is just an array of ushorts which takes up the first
    // 16 sectors (128 KB) of the volume. The sector immediately
    // following it is the start of the root directory.
    // There is no magic number because there's nowhere to put it.
    // If a file or directory is opened from multiple places at once and
    // modified, things will almost certainly go horribly wrong.
    // There is not really support for attributes. The "length" field of
    // directories will be set to -1 so that they can't be opened as
    // files.
    public class PlexFAT
    {
        private static class Helpers
        {
            public static BinaryReader MakeBR(Stream fobj)
            {
                return new BinaryReader(fobj, Encoding.UTF8, true);
            }
            public static BinaryWriter MakeBW(Stream fobj)
            {
                return new BinaryWriter(fobj, Encoding.UTF8, true);
            }
        }
        
        private const ushort FREE = 0; // unused space
        private const ushort RESERVED = 1; // the FAT
        private const ushort END = 2; // last sector of an entry
        
        private class LocalSubstream: Stream
        {
            private PlexFAT vol;
            private int lenpos; // Confusing - the position of the length of the file
            private int possec, isecpos, flen;
            private List<ushort> sectorMap;
            public override bool CanRead { get { return true; } }
            public override bool CanWrite { get { return true; } }
            public override bool CanSeek { get { return true; } }
            public override long Length { get { return flen; } }
            public override long Position { get { return possec * 8192 + isecpos; } set { Seek(value, SeekOrigin.Begin); } }
            public override void Flush() { vol.fobj.Flush(); }
            private int remInSec { get { return 8192 - isecpos; } }
            private int remInFile { get { return (int)(flen - Position); } }
            private long realPos { get { return sectorMap[possec] * 8192 + isecpos; } }
            public LocalSubstream(PlexFAT vol, ushort firstSector, int lenpos)
            {
                using (var read = Helpers.MakeBR(vol.fobj))
                {
                    this.vol = vol;
                    this.lenpos = lenpos;
                    this.possec = 0;
                    this.isecpos = 0;
                    vol.fobj.Position = lenpos;
                    this.flen = read.ReadInt32();
                    this.sectorMap = new List<ushort>();
                    ushort sec = firstSector;
                    while (sec != END)
                    {
                        if (sec == RESERVED)
                            throw new InvalidOperationException("Reached a reserved block.");
                        this.sectorMap.Add(sec);
                        sec = vol.theFAT[sec];
                    }
                }
            }
            public override int Read(byte[] buffer, int offset, int count)
            {
                int bytesToRead = count;
                while (bytesToRead > 0 && remInFile > 0)
                {
                    int block = Math.Min(remInSec, bytesToRead);
                    vol.fobj.Position = realPos;
                    vol.fobj.Read(buffer, offset, block);
                    offset += block;
                    isecpos += block;
                    bytesToRead -= block;
                    if (isecpos > 8192) // sanity check - if this is true a programmer fucked up, probably me
                        throw new InvalidOperationException("I know something is very wrong.");
                    if (isecpos == 8192)
                    {
                        possec++;
                        isecpos = 0;
                    }
                }
                return count - bytesToRead;
            }
            public override void Write(byte[] buffer, int offset, int count)
            {
                int bytesToWrite = count;
                while (bytesToWrite > 0)
                {
                    int block = Math.Min(remInSec, bytesToWrite);
                    vol.fobj.Position = realPos;
                    vol.fobj.Write(buffer, offset, block);
                    offset += block;
                    isecpos += block;
                    bytesToWrite -= block;
                    if (isecpos > 8192) // sanity check
                        throw new InvalidOperationException("I can't give everything away.");
                    if (isecpos == 8192)
                    {
                        possec++;
                        if (possec == sectorMap.Count)
                        {
                            sectorMap.Add(vol.getFreeSector());
                            vol.setFAT(sectorMap[possec - 1], sectorMap[possec]);
                            vol.setFAT(sectorMap[possec], END);
                        }
                        isecpos = 0;
                    }
                }
                if (Position > flen)
                {
                    SetLength(Position);
                }
            }
            public override long Seek(long offset, SeekOrigin origin)
            {
                long newpos = -1;
                switch (origin)
                {
                    case SeekOrigin.Begin: newpos = offset; break;
                    case SeekOrigin.Current: newpos = Position + offset; break;
                    case SeekOrigin.End: newpos = flen - offset; break;
                }
                possec = DivRem((int) newpos, 8192, out isecpos);
                return newpos;
            }
            public override void SetLength(long value)
            {
                if (flen == value)
                    return;
                int origSectors = sectorMap.Count;
                int newSectors = (int)((value + 8191) / 8192);
                if (flen > value)
                {
                    int lostSectors = origSectors - newSectors;
                    for (int i = sectorMap.Count - lostSectors; i < sectorMap.Count; i++)
                        vol.setFAT(sectorMap[i], FREE);
                    sectorMap.RemoveRange(sectorMap.Count - lostSectors, lostSectors);
                    vol.setFAT(sectorMap[sectorMap.Count - 1], END);
                }
                else
                {
                    int gainedSectors = newSectors - origSectors;
                    for (int i = 0; i < gainedSectors; i++)
                    {
                        // yuck
                        vol.setFAT(sectorMap[sectorMap.Count - 1], vol.getFreeSector());
                        sectorMap.Add(vol.theFAT[sectorMap[sectorMap.Count - 1]]);
                        vol.setFAT(sectorMap[sectorMap.Count - 1], END);
                    }
                }
                flen = (int) value;
                vol.fobj.Position = lenpos;
                using (var write = Helpers.MakeBW(vol.fobj))
                    write.Write(flen);
            }
        }
        
        public class Directory
        {
            internal Directory()
            {
            }
            
            internal PlexFAT vol;
            internal Dictionary<string, Tuple<ushort, int>> entries;
            internal ushort firstSector;
            
            public static Directory FromVol(PlexFAT vol)
            {
                var ret = new Directory();
                ret.vol = vol;
                ret.entries = new Dictionary<string, Tuple<ushort, int>>();
                ushort curSector = (ushort)(vol.fobj.Position / 8192);
                ret.firstSector = curSector;
                using (var read = Helpers.MakeBR(vol.fobj))
                {
                    int i = 0;
                    while (true)
                    {
                        int entpos = (int) vol.fobj.Position;
                        byte[] fnameRaw = read.ReadBytes(250);
                        int fnameLength;
                        for (fnameLength = 0; fnameLength < 250 && fnameRaw[fnameLength] != 0; fnameLength++)
                        {
                        }
                        string fname = Encoding.UTF8.GetString(fnameRaw, 0, fnameLength);
                        ushort firstSector = read.ReadUInt16();
                        if (fnameLength != 0)
                            ret.entries.Add(fname, Tuple.Create(firstSector, entpos));
                        vol.fobj.Position += 4; // don't read length here
                        i++;
                        if (i == 32) // 32 entries fit in one sector
                        {
                            if (vol.theFAT[curSector] == END)
                                break;
                            i = 0;
                            curSector = vol.theFAT[curSector];
                        }
                    }
                }
                return ret;
            }
            
            // validates name & converts to UTF8
            private byte[] getNameBytes(string fname)
            {
                if (String.IsNullOrEmpty(fname) || fname == "." || fname == ".." || fname.Contains(((char) 0).ToString()) || fname.Contains("/"))
                    throw new IOException("The entry name is invalid.");
                byte[] ret = Encoding.UTF8.GetBytes(fname);
                if (ret.Length > 250)
                    throw new IOException("The entry name is too long.");
                if (entries.ContainsKey(fname))
                    throw new IOException("An entry with that name already exists.");
                return ret;
            }
            
            private void addEntry(string fname, ushort firstSector, int flen)
            {
                byte[] namebytes = getNameBytes(fname);
                ushort curSector = this.firstSector;
                vol.fobj.Position = this.firstSector * 8192;
                int i = 0;
                while (vol.fobj.ReadByte() != 0)
                {
                    vol.fobj.Position += 255;
                    i++;
                    if (i == 32) // end of sector
                    {
                        if (vol.theFAT[curSector] == END)
                        {
                            vol.setFAT(curSector, vol.getFreeSector());
                            curSector = vol.theFAT[curSector];
                            vol.setFAT(curSector, END);
                            vol.fobj.Position = curSector * 8192;
                            using (var write = Helpers.MakeBW(vol.fobj))
                                write.Write(new byte[8192]);
                            vol.fobj.Position -= 8191; // the last byte will be moved past later
                            break;
                        }
                        curSector = vol.theFAT[curSector];
                        
                        vol.fobj.Position = curSector * 8192;
                        i = 0;
                    }
                }
                vol.fobj.Position -= 1;
                entries.Add(fname, Tuple.Create(firstSector, (int) vol.fobj.Position));
                using (var write = Helpers.MakeBW(vol.fobj))
                {
                    write.Write(namebytes);
                    write.Write(new byte[250 - namebytes.Length]);
                    write.Write(firstSector);
                    write.Write(flen);
                }
            }
            
            public bool Exists(string fname)
            {
                return entries.ContainsKey(fname);
            }
            
            public IEnumerable<string> Contents { get { return entries.Keys; } }
            
            public void Delete(string fname)
            {
				if (!entries.ContainsKey(fname))
					throw new IOException($"'{fname}' does not exist.");
                var entry = entries[fname];
                vol.fobj.Position = entry.Item2;
                using (var write = Helpers.MakeBW(vol.fobj))
                {
                    write.Write(new byte[252]);
                    using (var read = Helpers.MakeBR(vol.fobj))
                        if (read.ReadInt32() < 0) // directory
                        {
                            vol.fobj.Position = entry.Item1 * 8192;
                            var subdir = Directory.FromVol(vol);
                            foreach (string sname in subdir.Contents)
                                subdir.Delete(sname);
                        }
                    ushort sec = entry.Item1;
                    while (sec != END)
                    {
						ushort next = vol.theFAT[sec];
                        vol.setFAT(sec, FREE);
                        sec = next;
                    }
                }
                entries.Remove(fname);
            }
            
            public void Rename(string oldName, string newName)
            {
                if (oldName == newName)
                    return;
                if (!entries.ContainsKey(oldName))
                    throw new IOException($"'{oldName}' does not exist.");
                if (entries.ContainsKey(newName))
                    Delete(newName);
                byte[] namebytes = getNameBytes(newName);
                vol.fobj.Position = entries[oldName].Item2;
                using (var write = Helpers.MakeBW(vol.fobj))
                {
                    write.Write(Encoding.UTF8.GetBytes(newName));
                    write.Write(new byte[250 - namebytes.Length]);
                }
            }
            
            public void Move(string fname, Directory destDir, string destName)
            {
                // Validate the destination name.
                getNameBytes(destName);
                
                if (ReferenceEquals(this, destDir))
                {
                    Rename(fname, destName);
                    return;
                }
                if (!ReferenceEquals(this.vol, destDir.vol))
                    throw new NotSupportedException("Directory.Move() cannot move an entry across volumes.");
                if (destDir.Exists(destName))
                    destDir.Delete(destName);
                vol.fobj.Position = entries[fname].Item2;
                using (var write = Helpers.MakeBW(vol.fobj))
                {
                    write.Write(new byte[252]);
                    int flen;
                    using (var read = Helpers.MakeBR(vol.fobj))
                        flen = read.ReadInt32();
                    destDir.addEntry(destName, firstSector, flen);
                    if (flen < 0)
                    {
                        vol.fobj.Position = entries[fname].Item1 * 8192 + 250;
                        write.Write(this.firstSector);
                    }
                }
                this.entries.Remove(fname);
            }
            
            public Directory GetSubdirectory(string dname, OpenMode mode = OpenMode.Open)
            {
                if (entries.ContainsKey(dname))
                {
                    vol.fobj.Position = entries[dname].Item2 + 252;
                    using (var read = Helpers.MakeBR(vol.fobj))
                        if (read.ReadInt32() >= 0)
                            throw new IOException($"'{dname}' is a file.");
                    vol.fobj.Position = entries[dname].Item1 * 8192;
                    return Directory.FromVol(vol);
                }
                else if (mode == OpenMode.OpenOrCreate)
                {
                    var ret = new Directory();
                    ret.vol = vol;
                    ret.entries = new Dictionary<string, Tuple<ushort, int>>();
                    ret.firstSector = vol.getFreeSector();
                    
                    // write parent directory
                    using (var write = Helpers.MakeBW(vol.fobj))
                    {
                        vol.fobj.Position = ret.firstSector * 8192;
                        write.Write(Encoding.UTF8.GetBytes(".."));
                        write.Write(new byte[248]);
                        write.Write(this.firstSector);
                        write.Write(-1);
                        
                        // zero out the rest of the sector
                        write.Write(new byte[7936]);
                    }
                    
                    vol.setFAT(ret.firstSector, END);
                    
                    addEntry(dname, ret.firstSector, -1);
                    
                    return ret;
                }
                else
                    throw new DirectoryNotFoundException($"'{dname}' does not exist.");
            }
            
            public Stream OpenFile(string fname, OpenMode mode = OpenMode.Open)
            {
                if (!entries.ContainsKey(fname))
                {
                    if (mode == OpenMode.OpenOrCreate)
                    {
                        ushort firstSector = vol.getFreeSector();
                        vol.setFAT(firstSector, END);
                        vol.fobj.Position = firstSector * 8192;
                        using (var write = Helpers.MakeBW(vol.fobj))
                            write.Write(new byte[8192]);
                        addEntry(fname, firstSector, 0);
                    }
                    else
                        throw new FileNotFoundException($"'{fname}' does not exist.");
                }
                vol.fobj.Position = entries[fname].Item2 + 252;
                using (var read = Helpers.MakeBR(vol.fobj))
                    if (read.ReadInt32() < 0)
                        throw new IOException($"'{fname}' is a directory.");
                
                return new LocalSubstream(vol, entries[fname].Item1, entries[fname].Item2 + 252);
            }
            
            public EntryType TypeOf(string fname)
            {
                if (!entries.ContainsKey(fname))
                    return EntryType.NONEXISTENT;
                vol.fobj.Position = entries[fname].Item2 + 252;
                using (var read = Helpers.MakeBR(vol.fobj))
                    return read.ReadInt32() < 0 ? EntryType.DIRECTORY : EntryType.FILE;
            }
            
            public int SizeOf(string fname)
            {
                int ret;
                vol.fobj.Position = entries[fname].Item2 + 252;
                using (var read = Helpers.MakeBR(vol.fobj))
                    ret = read.ReadInt32();
                if (ret < 0)
                    throw new IOException("Cannot take the length of a directory.");
                return ret;
            }
        }
        
        private PlexFAT()
        {
        }
        
        private Stream fobj;
        private ushort[] theFAT;
        
        public Directory Root { get; private set; }
        
        public static PlexFAT FromStream(Stream fobj)
        {
            var ret = new PlexFAT();
            using (var read = Helpers.MakeBR(fobj))
            {
                // The FAT is an array of 65,536 ushorts. You don't get
                // much flexibility in read types in C# so I read bytes
                // and BlockCopy them to the array.
                ret.fobj = fobj;
                ret.theFAT = new ushort[65536];
                Buffer.BlockCopy(read.ReadBytes(131072), 0, ret.theFAT, 0, 131072);
            }
            ret.Root = Directory.FromVol(ret);
            return ret;
        }
        
        public static PlexFAT New(Stream fobj)
        {
            var ret = new PlexFAT();
            using (var write = Helpers.MakeBW(fobj))
            {
                ret.fobj = fobj;
                ret.theFAT = new ushort[65536];
                
                // FAT itself
                for (int i = 0; i < 16; i++)
                {
                    write.Write(RESERVED);
                    ret.theFAT[i] = RESERVED;
                }
                
                // root directory FAT entry
                write.Write(END);
                ret.theFAT[16] = END;
                
                ret.Root = new Directory()
                {
                    vol = ret,
                    entries = new Dictionary<string, Tuple<ushort, int>>(),
                    firstSector = 16
                };
                
                // zero out rest of FAT and root dir
                write.Write(new byte[139230]); 
            }
            return ret;
        }
        
        private void setFAT(ushort index, ushort val)
        {
            theFAT[index] = val;
            fobj.Position = index * 2;
            using (var write = Helpers.MakeBW(fobj))
                write.Write(val);
        }
        
        private ushort getFreeSector()
        {
            ushort ret;
            // the first 17 sectors are guaranteed to be used
            // so don't waste time checking them
            for (ret = 17; theFAT[ret] != FREE; ret++)
                if (ret == 65535)
                    throw new IOException("There are no free sectors on the volume.");
            return ret;
        }
    }
}

