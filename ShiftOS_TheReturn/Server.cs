using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Objects;

namespace ShiftOS.Engine
{
    public interface Server
    {
        /// <summary>
        /// Occurs when someone sends a message to the server.
        /// </summary>
        /// <param name="msg">The message from the client.</param>
        void MessageReceived(ServerMessage msg);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class ServerAttribute : Attribute
    {
        public ServerAttribute(string name, int port)
        {
            Name = name;
            Port = port;
        }


        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the port of the server.
        /// </summary>
        public int Port { get; }

    }
}
