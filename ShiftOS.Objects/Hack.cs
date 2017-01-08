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
    public abstract class Exploit
    {
        public void BeginExploit(string remote_user, bool isMud)
        {
            var ctx = new ExploitContext();
            SendToMUD(remote_user, "hack_getcontext");
            MessageReceived += (u, c, j) =>
            {

            };
            ThisContext = ctx;
        }

        public ExploitContext ThisContext { get; internal set; }

        public virtual void SendToMUD(string target_user, string command, string json = "")
        {
            ThisContext.IsMUDHack = false;
            if (command == "hack_getcontext")
            {
                MessageReceived?.Invoke(target_user, "context_info", ExploitContext.CreateRandom());
            }
        }

        public event MUDMessageEventHandler MessageReceived;


        public abstract void OnRun(ExploitContext ctx);
    }

    public delegate void MUDMessageEventHandler(string target_user, string command, string json);

    public class ExploitContext
    {
        public static string CreateRandom()
        {
            //We can't use JSON.NET. We must construct the JSON ourselves.
            StringBuilder jBuilder = new StringBuilder();
            jBuilder.AppendLine("{");
            jBuilder.Append("\tIsMUDHack: \"false\",");

            jBuilder.AppendLine("}");
            return jBuilder.ToString();
        }
        
        /// <summary>
        /// Gets or sets whether or not this exploit context belongs to a MUD hack session.
        /// </summary>
        public bool IsMUDHack { get; set; }

        /// <summary>
        /// Gets or sets the target username for this exploit context. Used for talking with the MUD about it.
        /// </summary>
        public string TargetUsername { get; set; }

        /// <summary>
        /// Gets or sets the target's locks.
        /// </summary>
        public List<Lock> TargetLocks { get; set; }

    }

    public abstract class Lock
    {
        public abstract bool Unlocked { get; }
        public abstract void Unlock();
    }
}
