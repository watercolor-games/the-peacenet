using LiteDB;
using Newtonsoft.Json;
using Plex.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Peacenet.Backend
{
    public class ReputationManager : IBackendComponent
    {
        [Dependency]
        private DatabaseHolder _database = null;

        private LiteCollection<GovernmentAlertState> _gvtAlertStates = null;
        
        [Dependency]
        private SystemEntityBackend _entityBackend = null;

        [Dependency]
        private Backend _backend = null;

        private Timer _updateTimer = null;

        public void Initiate()
        {
            Logger.Log("The Government is reviewing its database on targeted sentiences...");
            _gvtAlertStates = _database.Database.GetCollection<GovernmentAlertState>("world_gvtalert");
            _gvtAlertStates.EnsureIndex(x => x.Id);
            _entityBackend.EntitySpawned += (id, entity) =>
            {
                var alert = _gvtAlertStates.FindOne(x => x.EntityId == id);
                if (alert == null)
                {
                    alert = new GovernmentAlertState
                    {
                        Id = Guid.NewGuid().ToString(),
                        EntityId = id,
                        AlertLevel = 0
                    };
                    _gvtAlertStates.Insert(alert);
                }
            };
            _backend.PlayerJoined += (id, player) =>
            {
                var entity = _entityBackend.GetPlayerEntityId(id);
                var alert = _gvtAlertStates.FindOne(x => x.EntityId == entity);
                if (alert == null)
                {
                    alert = new GovernmentAlertState
                    {
                        Id = Guid.NewGuid().ToString(),
                        EntityId = entity,
                        AlertLevel = 0
                    };
                    _gvtAlertStates.Insert(alert);
                }


            };
            _updateTimer = new Timer(2000);
            _updateTimer.AutoReset = true;
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _updateTimer.Start();
        }

        public void Dissipate(string entity)
        {
            var state = _gvtAlertStates.FindOne(x => x.EntityId == entity);
            if (state == null)
                return;
            if (state.AlertLevel > 0)
                state.IsDissipating = true;
            _gvtAlertStates.Update(state);
            var playerId = _entityBackend.GetPlayerId(state.EntityId);
            if (playerId != null)
            {
                dispatchAlertLevel(playerId, state.AlertLevel, state.IsDissipating);
            }

        }

        private void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach(var state in _gvtAlertStates.Find(x=>x.IsDissipating))
            {
                state.AlertLevel -= 0.025;
                if(state.AlertLevel < 0)
                {
                    state.AlertLevel = 0;
                    state.IsDissipating = false;
                    Logger.Log($"Entity {state.EntityId} has left Government Alert.");
                }
                _gvtAlertStates.Update(state);
                var playerId = _entityBackend.GetPlayerId(state.EntityId);
                if(playerId != null)
                {
                    dispatchAlertLevel(playerId, state.AlertLevel, state.IsDissipating);
                }
            }
            foreach(var state in _gvtAlertStates.Find(x=>x.IsDissipating==false))
            {
                if(state.AlertLevel>0)
                {
                    state.TicksToDissipation++;
                    if(state.TicksToDissipation>=30)
                    {
                        state.TicksToDissipation = 0;
                        state.IsDissipating = true;
                        var playerId = _entityBackend.GetPlayerId(state.EntityId);
                        if (playerId != null)
                        {
                            dispatchAlertLevel(playerId, state.AlertLevel, state.IsDissipating);
                        }
                    }
                }
                else
                {
                    state.TicksToDissipation = 0;
                }
                _gvtAlertStates.Update(state);
            }
        }

        public void IncreaseAlertValue(string entity, AlertChangeValue value)
        {
            var state = GetGovernmentAlertStatus(entity);
            if (state == null)
                return;
            switch (value)
            {
                case AlertChangeValue.Minimal:
                    state.AlertLevel = Clamp(state.AlertLevel + 0.05, 0, 1);
                    break;
                case AlertChangeValue.Half:
                    state.AlertLevel = Clamp(state.AlertLevel + 0.1, 0, 1);
                    break;
                case AlertChangeValue.Full:
                    state.AlertLevel = Clamp(state.AlertLevel + 0.2, 0, 1);
                    break;
            }
            state.IsDissipating = false;
            _gvtAlertStates.Update(state);
            var playerid = _entityBackend.GetPlayerId(state.EntityId);
            if (playerid != null)
            {
                dispatchAlertLevel(playerid, state.AlertLevel, state.IsDissipating);
            }
        }

        public void DecreaseAlertValue(string entity, AlertChangeValue value)
        {
            var state = GetGovernmentAlertStatus(entity);
            if (state == null)
                return;
            switch (value)
            {
                case AlertChangeValue.Minimal:
                    state.AlertLevel = Clamp(state.AlertLevel - 0.05, 0, 1);
                    break;
                case AlertChangeValue.Half:
                    state.AlertLevel = Clamp(state.AlertLevel - 0.1, 0, 1);
                    break;
                case AlertChangeValue.Full:
                    state.AlertLevel = Clamp(state.AlertLevel - 0.2, 0, 1);
                    break;
            }
            state.IsDissipating = false;
            _gvtAlertStates.Update(state);
            var playerid = _entityBackend.GetPlayerId(state.EntityId);
            if (playerid != null)
            {
                dispatchAlertLevel(playerid, state.AlertLevel, state.IsDissipating);
            }
        }



        private double Clamp(double value, double min, double max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public void SetAlertLevel(string entity, double level)
        {
            if (level < 0)
                level = 0;
            if (level > 1)
                level = 1;
            var state = _gvtAlertStates.FindOne(x => x.EntityId == entity);
            if (state == null)
                return;
            state.AlertLevel = level;
            state.IsDissipating = false;
            if (state.AlertLevel <= 0 && state.IsDissipating)
                state.IsDissipating = false;
            _gvtAlertStates.Update(state);
            var playerId = _entityBackend.GetPlayerId(state.EntityId);
            if (playerId != null)
            {
                dispatchAlertLevel(playerId, state.AlertLevel, state.IsDissipating);
            }
        }

        private void dispatchAlertLevel(string id, double alert, bool dissipating)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms, Encoding.UTF8))
                {
                    writer.Write(id);
                    writer.Write(alert);
                    writer.Write(dissipating);
                    writer.Flush();
                    _backend.BroadcastToPlayer(Plex.Objects.ServerBroadcastType.GovernmentAlert, ms.ToArray(), id);
                }
            }
        }

        public GovernmentAlertState GetGovernmentAlertStatus(string entity)
        {
            return _gvtAlertStates.FindOne(x => x.EntityId == entity);
        }

        public void SafetyCheck()
        {
        }

        public void Unload()
        {
            _updateTimer.Stop();
        }
    }

    public enum AlertChangeValue
    {
        Minimal,
        Half,
        Full
    }

    public class GovernmentAlertState
    {
        public string Id { get; set; }
        public string EntityId { get; set; }
        public double AlertLevel { get; set; }
        public bool IsDissipating { get; set; }
        public int TicksToDissipation { get; set; }
    }

#if DEBUG
    public class GovernmentAlertTestCommand : ITerminalCommand
    {
        public string Name => "governmentalert";

        public string Description => "Manage your current Government Alert status.";

        [Dependency]
        private ReputationManager _rep = null;

        [Dependency]
        private SystemEntityBackend _entity = null;

        public IEnumerable<string> UsageStrings
        {
            get
            {
                yield return "dissipate";
                yield return "set <rawlevel>";
                yield return "increase";
                yield return "decrease";
                yield return "status";
            }
        }

        public void Run(Backend backend, ConsoleContext console, string sessionid, Dictionary<string, object> args)
        {
            string entity = _entity.GetPlayerEntityId(sessionid);

            if ((bool)args["status"])
            {
                var status = _rep.GetGovernmentAlertStatus(entity);
                console.WriteLine("Government Alert status for " + entity + ":");
                console.WriteLine(JsonConvert.SerializeObject(status, Formatting.Indented));
            }
            else if ((bool)args["set"])
            {
                float level = -1;
                if(float.TryParse(args["<rawlevel>"].ToString(), out level)==false)
                {
                    console.WriteLine("Error: Alert level must be a valid floating-point number between 0 and 1.");
                    return;
                }
                if(level < 0 || level > 1)
                {
                    console.WriteLine("Error: Alert level must be a valid floating-point number between 0 and 1.");
                    return;
                }
                console.WriteLine("Updating your Alert Level... Peacegate should start doing stuff immediately.");
                _rep.SetAlertLevel(entity, level);
            }
            else if((bool)args["dissipate"])
            {
                _rep.Dissipate(entity);
            }
            else if((bool)args["increase"])
            {
                _rep.IncreaseAlertValue(entity, AlertChangeValue.Full);
            }
            else if((bool)args["decrease"])
            {
                _rep.DecreaseAlertValue(entity, AlertChangeValue.Full);
            }
        }
    }
#endif
}
