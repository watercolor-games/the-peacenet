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
        float GameCompletion { get; }
        float Reputation { get; }

        bool AlertFalling { get; }

        bool TutorialCompleted { get; set; }

        bool IsMissionComplete(string missionID);
        bool IsCountryUnlocked(Country country);
        bool IsPackageInstalled(string packageID);
        
        
    }
}
