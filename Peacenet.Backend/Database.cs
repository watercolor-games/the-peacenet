using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace Peacenet.Backend
{
    public class DatabaseHolder : IBackendComponent
    {
        [Dependency]
        private Backend _backend = null;

        private string _dbPath = null;

        private LiteDatabase _db = null;

        private readonly string[] _asciidb = new[]
        {
"         NMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN",
"    /mmmmMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMmmmm/",
"  ohmMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMmho",
"  hMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMh",
"  hMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMh",
"  hMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMh",
"  sdNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNds  ",
"   `oMMNNMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMNNMMo`   ",
"  hMMMd  NMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN  dMMMh",
"  hMMMMNNMMMMy``````````````````````yMMMMNNMMMMh",
"  hMMMMMMMMMMNddddddddddddddddddddddNMMMMMMMMMMh",
"  hMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMh",
"  /ohMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMho/",
"  :/yMMMMyymMMMMMMMMMMMMMMMMMMMMMMMMMMmyyMMMMy/:  ",
"  hMMMMMM:-hMMMMMMddddddddddddddMMMMMMh-:MMMMMMh",
"  hMMMMMMMMMMMMMMM:````````````:MMMMMMMMMMMMMMMh",
"  hMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMh  ",
"   `oMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMo`   ",
"  sdo-/MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM/-ods",
"  hMmhhMMMMs:::/MMMMMMMMMMMMMMMMMM/:::sMMMMhhmMh",
"  hMMMMMMMMhssssMMMMMMMMMMMMMMMMMMsssshMMMMMMMMh",
"  hMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMh",
"  ohmMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMmho",
"    /mmmmMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMmmmm/",
"         NMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMN"
        };

        public LiteDatabase Database
        {
            get
            {
                return _db;
            }
        }

        public void Initiate()
        {
            _dbPath = Path.Combine(_backend.RootDirectory, "server.db");
            Logger.Log("LITEDB DATABASE BACKEND FOR PEACENET SERVER");
            Logger.Log("-------------------------------------------");
            Logger.Log("");
            foreach(var line in _asciidb)
            {
                Logger.Log(line);
            }
            Logger.Log("");
            Logger.Log("Provides a single storage area for all your data. No SQL whatsoever.");
            Logger.Log("Starting database...");
            _db = new LiteDatabase(_dbPath);
        }

        public void SafetyCheck()
        {
        }

        public void Unload()
        {
            Logger.Log("Database says bye bye.");
            _db.Dispose();
        }
    }
}
