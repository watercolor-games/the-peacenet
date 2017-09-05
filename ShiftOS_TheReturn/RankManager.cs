using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Objects;

namespace Plex.Engine
{
    public static class RankManager
    {
        private static Rank[] _ranks = null;
        private static IRankProvider _provider = null;

        public static void Init(IRankProvider provider)
        {
            _provider = provider;
            var t = new System.Threading.Thread(() =>
            {
                while (SaveSystem.CurrentSave == null)
                    System.Threading.Thread.Sleep(1000);
                ulong last_xp = 0;
                while (true)
                {
                    if(SaveSystem.CurrentSave.Experience > last_xp)
                    {
                        last_xp = SaveSystem.CurrentSave.Experience;
                        SynchronizeRank();
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            });

            t.IsBackground = true;
            t.Start();
        }

        public static void RankUpgrade(Rank rank)
        {
            SaveSystem.CurrentSave.MaxLoadedUpgrades = rank.UpgradeMax;
            if(rank.UnlockedUpgrades != null)
            {
                foreach (var upg in rank.UnlockedUpgrades)
                    Upgrades.Buy(upg, 0);
            }
            _provider.OnRankUp(rank);
        }

        public static Rank GetCurrentRank()
        {
            return _ranks.OrderBy(x => x.Experience).ToArray()[SaveSystem.CurrentSave.Rank];
        }

        public static void SynchronizeRank()
        {
            if (_ranks == null)
                _ranks = _provider.GetRanks();
            var realRanks = _ranks.OrderBy(x => x.Experience).ToArray();
            if(realRanks[SaveSystem.CurrentSave.Rank].UpgradeMax > SaveSystem.CurrentSave.MaxLoadedUpgrades)
            {
                SaveSystem.CurrentSave.MaxLoadedUpgrades = realRanks[SaveSystem.CurrentSave.Rank].UpgradeMax;
            }
            var rankLen = realRanks.Length;
            int rank = SaveSystem.CurrentSave.Rank + 1;
            while(rank < rankLen)
            {
                if(SaveSystem.CurrentSave.Experience >= realRanks[rank].Experience)
                {
                    SaveSystem.CurrentSave.Rank = rank;
                    RankUpgrade(realRanks[rank]);
                    rank++;
                }
                else
                {
                    break;
                }
            }

        }
    }

    public interface IRankProvider
    {
        Rank[] GetRanks();
        void OnRankUp(Rank rank);
    }
}
