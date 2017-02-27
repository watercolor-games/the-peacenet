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
using ShiftOS.Objects.ShiftFS;

namespace ShiftOS.Engine
{
    public static class NotificationDaemon
    {
        public static Notification[] GetAllFromFile()
        {
            Notification[] notes = { };
            if (Utils.FileExists(Paths.GetPath("notifications.dat")))
            {
                notes = JsonConvert.DeserializeObject<Notification[]>(Utils.ReadAllText(Paths.GetPath("notifications.dat")));
            }
            return notes;
        }

        internal static void WriteNotes(Notification[] notes)
        {
            Utils.WriteAllText(Paths.GetPath("notifications.dat"), JsonConvert.SerializeObject(notes, Formatting.Indented));
        }

        public static event Action<Notification> NotificationMade;

        public static void AddNotification(NotificationType note, object data)
        {
            var lst = new List<Notification>(GetAllFromFile());
            lst.Add(new Engine.Notification(note, data));
            WriteNotes(lst.ToArray());
            NotificationMade?.Invoke(lst[lst.Count - 1]);
        }

        public static event Action NotificationRead;

        public static void MarkAllRead()
        {
            var notes = GetAllFromFile();
            for (int i = 0; i < notes.Length; i++)
                MarkRead(i);
        }

        public static void MarkRead(int note)
        {
            var notes = GetAllFromFile();
            if (note >= notes.Length || note < 0)
                throw new ArgumentOutOfRangeException("note", new Exception("You cannot mark a notification that does not exist as read."));

            notes[note].Read = true;
            WriteNotes(notes);
            NotificationRead?.Invoke();
        }

        public static int GetUnreadCount()
        {
            int c = 0;
            foreach (var note in GetAllFromFile())
                if (note.Read == false)
                    c++; //gahh I hate that programming language.
            return c;
        }

    }

    public struct Notification
    {
        public Notification(NotificationType t, object data)
        {
            Type = t;
            Data = data;
            Read = false;
            Timestamp = DateTime.Now;
        }

        public bool Read { get; set; }
        public NotificationType Type { get; set; }
        public object Data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public enum NotificationType
    {
        Generic = 0x00,
        MemoReceived = 0x10,
        MemoSent = 0x11,
        DownloadStarted = 0x20,
        DownloadComplete = 0x21,
        CodepointsReceived = 0x30,
        CodepointsSent = 0x31,
        ShopPurchase = 0x40,
        LegionInvite = 0x50,
        LegionKick = 0x51,
        LegionBan = 0x52,
        ChatBan = 0x60,
        MUDAnnouncement = 0x70,
        MUDMaintenance = 0x71,
        NewShiftOSUnstable = 0x72,
        NewShiftOSStable = 0x73,
        NewAppveyor = 0x74,
        CriticalBugwatch = 0x75,
        NewDeveloper = 0x76,
        NewShiftOSVideo = 0x77,
        NewShiftOSStream = 0x78,
        SavePurge = 0x79,
    }
}