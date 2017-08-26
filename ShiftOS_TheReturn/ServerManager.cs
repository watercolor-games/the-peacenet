using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;
using NetSockets;
using System.Windows.Forms;
using System.Threading;
using Plex;
using static Plex.Engine.SaveSystem;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Net.NetworkInformation;
using Plex.Frontend.GraphicsSubsystem;

namespace Plex.Engine
{
    public enum DisconnectType
    {
        UserRequested,
        Error,
        EngineShutdown
    }

    /// <summary>
    /// Digital Society connection management class.
    /// </summary>
    public static class ServerManager
    {
        public static void Disconnect(DisconnectType type, string userMessage = "You have been disconnected from the server.")
        {
            UIManager.Game.IPAddress = null;
            if(type == DisconnectType.UserRequested || type == DisconnectType.Error)
            {
                UIManager.Game.FireInitialized();
                if(type == DisconnectType.Error)
                {
                    Infobox.Show("Disconnected from server.", userMessage);
                }
            }  
        }

        internal static void HandleMessage(PlexServerHeader header)
        {
            if (header.PlexUser == SaveSystem.CurrentSave.Username)
            {
                if (header.PlexSysname == SaveSystem.CurrentSave.SystemName)
                {
                    foreach (var type in ReflectMan.Types)
                    {
                        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(x => x.GetCustomAttributes(false).FirstOrDefault(y => y is ClientMessageHandlerAttribute) != null))
                        {
                            var attribute = method.GetCustomAttributes(false).FirstOrDefault(x => x is ClientMessageHandlerAttribute) as ClientMessageHandlerAttribute;
                            if(attribute.ID == header.Message)
                            {
                                method.Invoke(null, new[] { header.Content, header.IPForwardedBy });
                            }
                        }
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ClientMessageHandlerAttribute : Attribute
    {
        public ClientMessageHandlerAttribute(string id)
        {
            ID = id;
        }

        public string ID { get; private set; }
    }
}