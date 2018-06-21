using Microsoft.Xna.Framework.Graphics;
using Peacenet.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.GameState
{
    public interface IGameStateInfo
    {
        void StartGame();
        void EndGame();

        Country CurrentCountry { get; }

        void SwitchCountry(Country country);

        IEnumerable<string> UpgradeIDs { get; }
        bool IsUpgradeInstalled(string upgradeID);
        int UpgradeSlotCount { get; }
        bool EnableUpgrade(string upgradeID);
        bool DisableUpgrade(string upgradeID);
        Upgrade GetUpgradeInfo(string upgradeID);

        int SkillLevel { get; }
        float SkillLevelPercentage { get; }
        int TotalXP { get; }

        void AddXP(int xp);

        float AlertLevel { get; }
        float GameCompletion { get; set; }
        float Reputation { get; set; }

        bool AlertFalling { get; }

        bool TutorialCompleted { get; set; }

        bool IsMissionComplete(string missionID);
        bool IsCountryUnlocked(Country country);
        bool IsPackageInstalled(string packageID);
        bool SentienceHacked(string id);

        IEnumerable<Connection> ActiveConnections { get; }

        Sentience Highlighted { get; }

        void Highlight(string hostname);

        Texture2D CountryTexture { get; }

        void CompleteMission(string missionID);
        void InstallPackage(string packageID);
        bool StartConnection(string host);
        bool EndConnection(string host);


        Sentience Player { get; }
        IEnumerable<Sentience> SingularSentiences { get; }
        IEnumerable<Faction> Factions { get; }
        IEnumerable<Sentience> GetSentiences(Faction faction);

        int UnreadEmails { get; }

        IEnumerable<EmailThread> Emails { get; }
        IEnumerable<EmailMessage> GetMessages(string threadId);
        void MarkRead(string messageId);

        event Action<string> MissionCompleted;
    }

    public struct Upgrade
    {
        public string Id;
        public string Name;
        public string Description;
        public int MinSkillLevel;
        public string[] Dependencies;
        
        public Upgrade(string id, string name, string desc, int minSkillLevel, params string[] dependencies)
        {
            Id = id;
            Name = name;
            Description = desc;
            MinSkillLevel = minSkillLevel;
            Dependencies = dependencies;
        }

        public static Upgrade Empty
        {
            get
            {
                return new Upgrade("", "", "", 0, null);
            }
        }
    }
}
