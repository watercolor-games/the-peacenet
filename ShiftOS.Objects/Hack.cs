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
