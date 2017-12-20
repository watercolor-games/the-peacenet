using System;
using Plex.Objects;
using System.Collections.Generic;
using System.IO;
using Whoa;
using System.Text;
using System.Linq;
using LiteDB;
using Newtonsoft.Json;

namespace Peacenet.Backend.Saves
{
    public class SaveManager : IBackendComponent
    {
        private LiteCollection<SaveFile> _saves = null;

        private LiteCollection<SaveValue> _values = null;

        [Dependency]
        private DatabaseHolder _db = null;

        public void Initiate()
        {
            Logger.Log("Save manager is starting...");
            _saves = _db.Database.GetCollection<SaveFile>("usersaves");
            _saves.EnsureIndex(x => x.Id);
            _values = _db.Database.GetCollection<SaveValue>("usersavevalues");
            _values.EnsureIndex(x => x.Id);
            Logger.Log($"Done. {_saves.Count()} saves loaded. {_values.Count()} total values loaded.");
        }

        public bool UserHasSave(string session)
        {
            var save = _saves.FindOne(x => x.UserId == session);
            return (save != null);
        }

        public SaveFile GetSave(string session)
        {
            if (UserHasSave(session))
            {
                return _saves.FindOne(x => x.UserId == session);
            }
            var save = new SaveFile
            {
                Id = Guid.NewGuid().ToString(),
                UserId = session,
            };
            _saves.Insert(save);
            return save;
        }

        public string GetValue(string session, string key, string defaultValue)
        {
            var save = GetSave(session);

            var val = _values.FindOne(x => x.SaveId == save.Id && x.Key == key);
            if (val != null)
                return val.Value;
            val = new SaveValue
            {
                Id = Guid.NewGuid().ToString(),
                Key = key,
                SaveId = save.Id,
                Value = defaultValue,
            };
            _values.Insert(val);
            return val.Value;

        }

        public void SetValue(string session, string key, string value)
        {
            var save = GetSave(session);

            var val = _values.FindOne(x => x.SaveId == save.Id && x.Key == key);
            if (val != null)
            {
                val.Value = value;
                _values.Update(val);
                return;
            }
            val = new SaveValue
            {
                Id = Guid.NewGuid().ToString(),
                Key = key,
                SaveId = save.Id,
                Value = value,
            };
            _values.Insert(val);
        }

        public void SafetyCheck()
        {
        }

        public void Unload()
        {
        }
    }

    public class SaveFile
    {
        public string Id { get; set; }
        public string UserId { get; set; }
    }

    public class SaveValue
    {
        public string Id { get; set; }
        public string SaveId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [RequiresSession]
    public class SaveGetValueHandler : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.SAVE_GETVAL;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var savemgr = backend.GetBackendComponent<SaveManager>();
            string key = datareader.ReadString();
            string val = datareader.ReadString();
            string ret = savemgr.GetValue(session, key, val);
            datawriter.Write(ret);
            return ServerResponseType.REQ_SUCCESS;
        }
    }

    [RequiresSession]
    public class SaveSetValueHandler : IMessageHandler
    {
        public ServerMessageType HandledMessageType
        {
            get
            {
                return ServerMessageType.SAVE_SETVAL;
            }
        }

        public ServerResponseType HandleMessage(Backend backend, ServerMessageType message, string session, BinaryReader datareader, BinaryWriter datawriter)
        {
            var savemgr = backend.GetBackendComponent<SaveManager>();
            string key = datareader.ReadString();
            string val = datareader.ReadString();
            savemgr.SetValue(session, key, val);
            return ServerResponseType.REQ_SUCCESS;
        }
    }

}
