using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Backend.Email
{
    public class EmailProvider : IBackendComponent
    {
        [Dependency]
        private SystemEntityBackend _entity = null;

        [Dependency]
        private DatabaseHolder _database = null;

        [Dependency]
        private Backend _backend = null;

        [Dependency]
        private IPBackend _ip = null;

        private LiteCollection<EmailAddress> _addresses = null;
        private LiteCollection<Email> _emails = null;
        private LiteCollection<ReadEmail> _readEmails = null;

        private string _postmasterId = null;

        public void Initiate()
        {
            //Set up email addresses.
            _addresses = _database.Database.GetCollection<EmailAddress>("emailaddresses");
            _addresses.EnsureIndex(x => x.Id);

            //Set up the mailboxes.
            _emails = _database.Database.GetCollection<Email>("emails");
            _emails.EnsureIndex(x => x.Id);

            //Set up the list of read emails.
            _readEmails = _database.Database.GetCollection<ReadEmail>("reademails");
            _readEmails.EnsureIndex(x => x.Id);

            //Purge invalid addresses and messages.
            PurgeInvalidEmails();

            //Spawn (or retrieve) the postmaster server's entity. This allows the service to be hacked, and also allows us to map an IP address and domain name to it so we can get "postmaster@serenitymail.net" as its email address, for example, and have all automated postmaster messages sent by that address.
            Logger.Log("Setting up email entity...");
            _postmasterId = _entity.SpawnNPCEntity("SerenityMail Communications Postmaster", "The postmaster of SerenityMail Communications' email system. This server handles sending and receiving of email messages for SerenityMail email accounts and contains the mailbox of all email accounts on the service.");
            Logger.Log("SerenityMail postmaster entity ID: " + _postmasterId);

            //Figure out if we already have an IP address.
            var ips = _ip.FetchAllIPs(_postmasterId);
            if (ips.Length == 0)
            {
                Logger.Log("Allocating IP address for the postmaster.");
                var ip = _ip.NextIP();
                _ip.AllocateIPv4Address(ip, _postmasterId);
                Logger.Log($"Postmaster IP address is {_ip.GetIPString(ip)}.");
            }

            //TODO: In-game DNS for that IP.

            //Determine if we have a "postmaster" email address.
            var address = _addresses.FindOne(x => x.Entity == _postmasterId);
            if(address != null)
            {
                //Allocate it.
                address = new EmailAddress
                {
                    Id = Guid.NewGuid().ToString(),
                    Entity = _postmasterId,
                    Name = "serenity-postmaster"
                };
                _addresses.Insert(address);
            }

            //Set up "Player Joined" events.
            _backend.PlayerJoined += (id, player) =>
            {
                var entity = _entity.GetPlayerEntityId(id);
                if(entity != null)
                {
                    var pAddress = _addresses.FindOne(x => x.Entity == entity);
                    if(pAddress != null)
                    {
                        pAddress.Name = player.username;
                        _addresses.Update(pAddress);
                    }
                    else
                    {
                        pAddress = new EmailAddress
                        {
                            Id = Guid.NewGuid().ToString(),
                            Entity = entity,
                            Name = player.username
                        };
                        _addresses.Insert(pAddress);
                        AddEmail(_postmasterId, entity, "Welcome to SerenityMail!", $@"Welcome to your new SerenityMail mailbox!

SerenityMail is the leading email commubications service of The Peacenet. In fact, we're pretty sure we're the only one out there. We thank you for using us to power your mailbox!

Your email address is: {player.username}@serenitymail.net

When you receive a new email message, we will push a notification to your Peacegate OS and you can open your inbox to read it. You can send messages to other people in the Peacenet if you have their email address. They will be notified when you send a message to them. If we can't deliver the message, we will let you know.

Thanks,
SerenityMail Postmaster");
                    }
                    DispatchUnreadCount(id, _emails.Find(x => x.ToEntity == entity && (_readEmails.FindOne(y => y.EmailId == x.Id) == null)).Count());
                }
            };
        }



        private void PurgeInvalidEmails()
        {
            Logger.Log("Purging emails with invalid entity IDs...");
            int purged = _emails.Delete(x => _entity.GetEntity(x.FromEntity) == null || _entity.GetEntity(x.ToEntity) == null);
            Logger.Log($"{purged} emails purged from database.");
            Logger.Log($"{_readEmails.Delete(x => _entity.GetEntity(x.Entity) == null)} \"read email\" entries purged for missing entities.");
            Logger.Log("Purging invalid email addresses.");
            Logger.Log($"{_addresses.Delete(x => _entity.GetEntity(x.Entity) == null)} address(es) removed.");
            
        }

        public void AddEmail(string from, string to, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new InvalidOperationException("Subject line can't be empty.");
            if (string.IsNullOrWhiteSpace(message))
                throw new InvalidOperationException("Message text can't be empty.");
            if (_entity.GetEntity(from) == null || _entity.GetEntity(to) == null)
                throw new InvalidOperationException("An invalid entity ID was specified for either the 'From' value or the 'To' value.");
            var email = new Email
            {
                Id = Guid.NewGuid().ToString(),
                Message = message,
                Subject = subject,
                FromEntity = from,
                ToEntity = to,
                Timestamp = DateTime.UtcNow
            };
            _emails.Insert(email);
            var player = _entity.GetPlayerId(to);
            if(player != null)
            {
                DispatchEmailReceived(player, from, subject);
                DispatchUnreadCount(player, _emails.Find(x => x.ToEntity == to && (_readEmails.FindOne(y => y.EmailId == x.Id) == null)).Count());
            }
        }

        private void DispatchUnreadCount(string player, int count)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms, Encoding.UTF8))
                {
                    writer.Write(player);
                    writer.Write(count);
                    writer.Flush();
                    _backend.BroadcastToPlayer(Plex.Objects.ServerBroadcastType.EmailsUnread, ms.ToArray(), player);
                }
            }
        }


        private void DispatchEmailReceived(string player, string from, string subject)
        {
            var entity = _entity.GetEntity(from);
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms, Encoding.UTF8))
                {
                    writer.Write(player);
                    writer.Write(entity.DisplayName);
                    writer.Write(subject);
                    writer.Flush();
                    _backend.BroadcastToPlayer(Plex.Objects.ServerBroadcastType.EmailReceived, ms.ToArray(), player);
                }
            }
        }

        public void SafetyCheck()
        {
            PurgeInvalidEmails();
        }

        public void Unload()
        {
        }
    }
}
