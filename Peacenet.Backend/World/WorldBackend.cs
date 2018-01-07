using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Peacenet.Backend
{
    /// <summary>
    /// Provides world management for the Peacenet.
    /// </summary>
    public class WorldBackend : IBackendComponent
    {
        private LiteCollection<InternetServiceProvider> _isps = null;

        [Dependency]
        private DatabaseHolder _db = null;

        [Dependency]
        private Backend _backend = null;

        private string[] _ispNames = new string[] { "Netcast", "Shawshank Cable", "CountryLink", "Peacenet Broadband Network"/*not an australia reference totally*/, "Bellstra", "Bohrzion Mobile", "Optopus", "Cablego"};
        private string[] _companySuffixes = new string[] { "Inc.", "Corp", "Corporation", "LLC", "Foundation", "Facility", "Software", "Games", "Medicine", "Research", "Consultants", "Bank" };
        private string[] _companyNames = new string[] { "Peacegate", "EOX Studios"/*nod to Anders Jensen*/, };

        /// <inheritdoc/>
        public void Initiate()
        {
            Logger.Log("World generator is now reading ISP table...");
            _isps = _db.Database.GetCollection<InternetServiceProvider>("worldIsps");
            _isps.EnsureIndex(x => x.Id);
            Logger.Log($"{_isps.Count()} found.");
            if (_backend.IsMultiplayer)
            {
                if (_isps.Count() != _ispNames.Length)
                {
                    Logger.Log("ISP database out of sync with server's internal namebank. Updating...");
                    foreach (var ispName in _ispNames)
                    {
                        Logger.Log($"Looking for \"{ispName.ToUpper()}\"...");
                        var existing = _isps.FindOne(x => x.Name == ispName);
                        if (existing == null)
                        {
                            Logger.Log("Not found. Creating new ISP...");
                            existing = new InternetServiceProvider
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = ispName
                            };
                            _isps.Insert(existing);
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void SafetyCheck()
        {
        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }

    internal class InternetServiceProvider
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public byte IpRange1 { get; set; }
        public byte IpRange2 { get; set; }
    }

    internal class ISPNet
    {
        public string Id { get; set; }
        public byte NetIpRange { get; set; }
        public NetworkType NetType { get; set; }
        public string Name { get; set; }
    }

    internal enum NetworkType
    {
        Player,
        NPC,
        Company
    }
}
