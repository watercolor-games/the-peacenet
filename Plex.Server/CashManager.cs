using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plex.Objects;

namespace Plex.Server
{
    public static class CashManager
    {
        [ServerMessageHandler("cash_deduct")]
        [SessionRequired]
        public static void CashDeduct(string session, string content, string ip)
        {
            var sessiondata = SessionManager.GrabAccount(session);
            var save = Program.GetSaveFromPrl(sessiondata.SaveID);
            int result = 0;
            if (save != null)
            {
                var args = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                string to = args["to"].ToString();
                long amount = (long)args["cash"];
                if(save.SystemDescriptor != null)
                {
                    if(save.SystemDescriptor.Cash >= amount)
                    {
                        result = 1; //We can deduct.
                        save.SystemDescriptor.Cash -= amount;
                        //Bye bye, cash.
                        //Now we determine the system to send it to.
                        var system = Program.GetSaveFromPrl(to);
                        if(system == null)
                        {
                            //we're sending it to rogue.
                            var rogue = Program.GameWorld.Rogue.NPCs[0];
                            system = rogue;
                            to = "main.rogue";
                        }
                        system.SystemDescriptor.Cash += amount;
                        if (system.SystemDescriptor.Transactions == null)
                            system.SystemDescriptor.Transactions = new List<CashTransaction>();
                        if (save.SystemDescriptor.Transactions == null)
                            save.SystemDescriptor.Transactions = new List<CashTransaction>();
                        system.SystemDescriptor.Transactions.Add(new CashTransaction
                        {
                            Amount = amount,
                            Date = DateTime.Now.ToString(),
                            From = sessiondata.SaveID,
                            To = to
                        });
                        save.SystemDescriptor.Transactions.Add(new CashTransaction
                        {
                            Amount = amount,
                            Date = DateTime.Now.ToString(),
                            From = sessiondata.SaveID,
                            To = to
                        });
                        Program.SaveWorld();
                    }
                }
            }
            Program.SendMessage(new Objects.PlexServerHeader
            {
                Message = "cash_deductresult",
                Content = result.ToString(),
                IPForwardedBy = ip,
                SessionID = session
            });
        }
    }
}
