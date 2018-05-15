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

    }
}
