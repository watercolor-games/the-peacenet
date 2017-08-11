using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;

namespace Plex.Frontend
{
    class PayloadFunc
    {
        public static void DoHackFunction(int function)
        {
            switch (function)
            {
                default:
                    break;
                case 1:
                    Hacking.CurrentHackable.DoConnectionTimeout = false;
                    break;
                case 2:
                    new System.Threading.Thread(() =>
                    {
                        Console.WriteLine("FTP File Puller - Version 1.01");
                        foreach (var loot in Hacking.CurrentHackable.ServerFTPLoot)
                        {
                            var bytes = Hacking.GetLootBytes(loot.ID);
                            System.Threading.Thread.Sleep(4 * bytes.Length);
                            string localPath = "0:/home/documents/" + loot.LootName;
                            int count = 0;
                            while (Objects.ShiftFS.Utils.FileExists(localPath))
                            {
                                count++;
                                string truename = loot.LootName.Insert(loot.LootName.LastIndexOf('.'), $"-{count}");
                                localPath = $"0:/home/documents/{truename}";
                            }
                            Console.WriteLine($" --> from {Hacking.CurrentHackable.Data.SystemName}:21/home/documents/{loot.LootName} to {localPath}: {bytes.Length} bytes written");
                            Objects.ShiftFS.Utils.WriteAllBytes(localPath, bytes);
                        }
                        Console.WriteLine("Disconnecting from server...");
                        Hacking.EndHack();
                        TerminalBackend.SetShellOverride("");
                    }).Start();
                    break;
            }
        }
    }
}
