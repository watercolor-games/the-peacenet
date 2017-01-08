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

namespace ShiftOS.Objects
{

    public class MUDMemo
    {
        public string UserFrom { get; set; }
        public string UserTo { get; set; }
        public MemoType Type { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
    }

    public class ClientSave
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public enum MemoType
    {
        Regular,
        Job,
        LegionInvite,
    }


    public class PongHighscore
    {
        public string UserName { get; set; }
        public int HighestLevel { get; set; }
        public int HighestCodepoints { get; set; }
    }

    public class GUIDRequest
    {
        public string name { get; set; }
        public string guid { get; set; }
    }

    public class OnlineUser
    {
        public string Guid { get; set; }
        public string Username { get; set; }
        public string OnlineChat { get; set; }
    }

    public class Channel
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string Topic { get; set; }
        public int MaxUsers { get; set; } //0 for unlimited users (or the MUD maximum)
        public List<Save> Users = new List<Save>();
    }


    [Serializable]
    public class ServerMessage
    {
        public string Name { get; set; }
        public string Contents { get; set; }
        public string GUID { get; set; }
    }

    //Better to store this stuff server-side so we can do some neat stuff with hacking...
    public class Save
    {
        public string Username { get; set; }
        public int Codepoints { get; set; }
        public Dictionary<string, bool> Upgrades { get; set; }
        public int StoryPosition { get; set; }
        public string Language { get; set; }

        public List<string> CurrentLegions { get; set; }

        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public int Revision { get; set; }

        public string Password { get; set; }
        public string SystemName { get; set; }

        public string DiscourseName { get; set; }

        /// <summary>
        /// If the user has entered their Discourse account into ShiftOS, this is the password they gave.
        /// 
        /// ANY developer caught abusing this property will have their dev status revoked and their account PERMANENTLY SUSPENDED. - Michael
        /// </summary>
        public string DiscoursePass { get; set; } 


        public int CountUpgrades()
        {
            int count = 0;
            foreach (var upg in Upgrades)
            {
                if (upg.Value == true)
                    count++;
            }
            return count;
        }
    }

}
