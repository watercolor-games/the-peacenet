using Plex.Engine;
using Plex.Engine.Interfaces;
using Plex.Engine.Saves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Email
{
    public class ClientEmailProvider : IEngineComponent
    {
        private Queue<Email> _incomingQueue = new Queue<Email>();

        [Dependency]
        private OS _os = null;

        [Dependency]
        private SaveManager _save = null;

        public void Initiate()
        {
            _os.SessionStart += () =>
            {
                if(!_save.GetValue("dev.hasBeenWelcomed", false))
                {
                    _save.SetValue("dev.hasBeenWelcomed", true);
                    Enqueue("devs@peacegate-os.net", "Welcome to Peacegate OS.", @"Hello. We are sending you this email because you have just successfully installed Peacegate OS.

Peacegate has many useful command-line and GUI programs you can use, and there are many more in the PPM (Peacenet Package Manager) that you can download for free. Simply open your Peacegate Menu or type 'help' in your Terminal to see exactly what you can do! Explore! The environment is your playground.

We thank you for choosing Peacegate as your primary Peacenet access gateway, and hope you have a wonderful time using it.

 - Peacegate devs :)");
                }
            };
        }

        public int Incoming => _incomingQueue.Count;

        public void Enqueue(string from, string subject, string message, string mission = null)
        {
            _incomingQueue.Enqueue(new Email(from, subject, message, mission));
        }

        public Email Dequeue()
        {
            if (_incomingQueue.Count > 0)
                return _incomingQueue.Dequeue();
            return null;
        }

        public class Email
        {
            public string Subject { get; private set; }
            public string From { get; private set; }
            public string Message { get; private set; }
            public string MissionID { get; private set; }

            public Email(string from, string subject, string message, string mission)
            {
                MissionID = mission;
                Message = message;
                From = from;
                Subject = subject;
            }
        }
    }
}
