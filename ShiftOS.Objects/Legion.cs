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
    public enum LegionRole
    {
        Admin,
        Manager,
        Committed,
        Trainee,
        AwaitingInvite
    }

    public enum LegionPublicity
    {
        Public, //Will display on the 'Join Legion' page, anyone can join
        PublicInviteOnly, //Will display on the 'Join Legion' page but you must be invited
        Unlisted, //Won't display on 'Join Legion', but anyone can join
        UnlistedInviteOnly //Won't display in 'Join Legion', and admin/manager invitation is required.
    }

    public class Legion
    {
        public string Name { get; set; }
        public LegionPublicity Publicity { get; set; }
        public ConsoleColor BannerColor { get; set; }
        public string Description { get; set; }
        public string ShortName { get; set; }

        public Dictionary<string, LegionRole> Roles { get; set; }
        public Dictionary<LegionRole, string> RoleNames { get; set; }

        public UserClass Class { get; set; }
        public double RawReputation { get; set; }

        public Reputation Reputation
        {
            get
            {
                return (Reputation)((int)Math.Round(RawReputation));
            }
        }

    }
}
