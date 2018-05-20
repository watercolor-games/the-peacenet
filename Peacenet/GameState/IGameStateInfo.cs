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

        void UnlockCountry(Country country);
        void CompleteMission(string missionID);
        void InstallPackage(string packageID);

        int UnreadEmails { get; }

        IEnumerable<EmailThread> Emails { get; }
        IEnumerable<EmailMessage> GetMessages(string threadId);
        void MarkRead(string messageId);

        event Action<string> MissionCompleted;
    }
}
