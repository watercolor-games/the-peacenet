using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.Backend
{
    public class IPBackend : IBackendComponent
    {
        private LiteCollection<PeacenetIPAddress> _addresses = null;

        private List<PeacenetIPConnection> _connections = new List<PeacenetIPConnection>();

        [Dependency]
        private Backend _backend = null;

        [Dependency]
        private DatabaseHolder _database = null;

        [Dependency]
        private SystemEntityBackend _entityBackend = null;

        public void BreakConnection(uint from, uint to)
        {
            if (from == to)
                throw new InvalidOperationException("You cannot connect to yourself.");
            var existing = _connections.FirstOrDefault(x => x.To == to && x.From == from);
            if (existing == null)
                throw new InvalidOperationException("These systems are not connected.");
            _connections.Remove(existing);
        }

        public PeacenetIPConnection[] GetConnectionsTo(uint ipaddress)
        {
            return _connections.Where(x => x.To == ipaddress).ToArray();
        }

        public PeacenetIPConnection[] GetConnectionsFrom(uint ipaddress)
        {
            return _connections.Where(x => x.From == ipaddress).ToArray();
        }

        public void MakeConnection(uint from, uint to)
        {
            if (from == to)
                throw new InvalidOperationException("You cannot connect to yourself.");
            var existing = _connections.FirstOrDefault(x => x.To == to && x.From == from);
            if (existing != null)
                throw new InvalidOperationException("These systems are already connected.");
            _connections.Add(new PeacenetIPConnection
            {
                To = to,
                From = from
            });
            string toEntity = GrabEntity(to);
            if(toEntity != null)
            {
                var playerId = _entityBackend.GetPlayerId(toEntity);
                _backend.BroadcastToPlayer(Plex.Objects.ServerBroadcastType.SYSTEM_CONNECTED, null, playerId);
            }
        }

        private Random _random = new Random();

        public uint GetIPFromString(string iPAddress)
        {
            if (string.IsNullOrWhiteSpace(iPAddress))
                throw new ArgumentException("IP string cannot be empty.");
            if (!iPAddress.Contains("."))
                throw new FormatException();
            string[] segments = iPAddress.Split('.');
            if (segments.Length != 4)
                throw new FormatException();
            byte seg1 = Convert.ToByte(segments[0]);
            byte seg2 = Convert.ToByte(segments[1]);
            byte seg3 = Convert.ToByte(segments[2]);
            byte seg4 = Convert.ToByte(segments[3]);

            return this.CombineToUint(new byte[] { seg1, seg2, seg3, seg4 });
        }

        public void Initiate()
        {
            Logger.Log("Loading IP addresses from database...");
            _addresses = _database.Database.GetCollection<PeacenetIPAddress>("world_ips");
            _addresses.EnsureIndex(x => x.Id);
            Logger.Log($"IP address lookup complete. {_addresses.Count()} IPs found.");
            _backend.PlayerJoined += (id, user) =>
            {
                var entity = _entityBackend.GetPlayerEntityId(id);
                var ips = FetchAllIPs(entity);
                if (ips.Length == 0)
                {
                    using (var random = RandomNumberGenerator.Create())
                    {
                        byte[] ipsegments = new byte[4];
                        random.GetBytes(ipsegments);
                        while (_addresses.FindOne(x => x.Address == this.CombineToUint(ipsegments)) != null)
                        {
                            random.GetBytes(ipsegments);
                        }
                        uint ip = CombineToUint(ipsegments);
                        AllocateIPv4Address(ip, entity);
                    }
                }
            };
        }

        public PeacenetIPAddress[] FetchAllIPs(string entityId)
        {
            return _addresses.Find(x => x.EntityId == entityId).ToArray();
        }

        public void DeallocateIPv4Address(uint address)
        {
            _addresses.Delete(x => x.Address == address);
        }

        public string GrabEntity(uint address)
        {
            var existing = _addresses.FindOne(x => x.Address == address);
            if (existing != null)
                return existing.EntityId;
            return null;
        }

        public byte[] SplitToBytes(uint ipaddress)
        {
            byte seg1 = (byte)(ipaddress & 0xff);
            byte seg2 = (byte)((ipaddress >> 8) & 0xff);
            byte seg3 = (byte)((ipaddress >> 16) & 0xff);
            byte seg4 = (byte)((ipaddress >> 24) & 0xff);
            return new byte[] { seg1, seg2, seg3, seg4 };
        }

        public uint CombineToUint(byte[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length != 4)
                throw new ArgumentException($"You cannot convert a {values.Length} byte array to an unsigned integer.");
            int result = 0;
            result = values[0] + (values[1] << 8) + (values[2] << 16) + (values[3] << 24);
            return (uint)result;
        }

        public string GetIPString(uint ipaddress)
        {
            var bytes = SplitToBytes(ipaddress);
            return $"{bytes[0]}.{bytes[1]}.{bytes[2]}.{bytes[3]}";
        }

        public void AllocateIPv4Address(uint ipaddress, string entityId)
        {
            var existing = _addresses.FindOne(x => x.Address == ipaddress);
            if (existing != null)
                throw new ArgumentException("The IP address you have specified is already allocated.");
            Logger.Log($"Allocating IP {GetIPString(ipaddress)} for {entityId}");
            _addresses.Insert(new PeacenetIPAddress
            {
                Id = Guid.NewGuid().ToString(),
                EntityId = entityId,
                Address = ipaddress
            });
        }

        public void SafetyCheck()
        {
        }

        public void Unload()
        {
        }
    }

    public class PeacenetIPAddress
    {
        public string Id { get; set; }
        public uint Address { get; set; }
        public string EntityId { get; set; }
    }

    public class PeacenetIPConnection
    {
        public uint To { get; set; }
        public uint From { get; set; }
    }
}
