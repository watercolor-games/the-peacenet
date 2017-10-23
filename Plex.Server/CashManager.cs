using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plex.Objects;

namespace Plex.Server
{
    public static class CashManager
    {
        [ServerCommand("sendcash", "Send a specified amount of cash to another user.")]
        [RequiresArgument("id")]
        [RequiresArgument("cash")]
        public static void SendCash(Dictionary<string, object> args)
        {
            string sys = args["id"].ToString();
            long cash = Convert.ToInt64(args["cash"].ToString());
            var you = SessionManager.GrabAccount(Terminal.SessionID);
            if(you == null)
            {
                Console.WriteLine("Error: No usersession detected. Command possibly ran through server console.");
                return;
            }
            var yousys = Program.GetSaveFromPrl(you.SaveID);
            if(yousys.SystemDescriptor.Cash < cash)
            {
                Console.WriteLine("Error: Not enough cash.");
                return;
            }
            var target = Program.GetSaveFromPrl(sys);
            if(target == null)
            {
                Console.WriteLine("Error: Target system not found!");
                return;
            }
            yousys.SystemDescriptor.Cash -= cash;
            target.SystemDescriptor.Cash += cash;
            var transaction = new CashTransaction
            {
                Amount = cash,
                Date = DateTime.Now.ToString(),
                From = you.SaveID,
                To = sys
            };
            if (target.SystemDescriptor.Transactions == null)
                target.SystemDescriptor.Transactions = new List<CashTransaction>();
            if (yousys.SystemDescriptor.Transactions == null)
                yousys.SystemDescriptor.Transactions = new List<CashTransaction>();
            target.SystemDescriptor.Transactions.Add(transaction);
            yousys.SystemDescriptor.Transactions.Add(transaction);
            Console.WriteLine("${0}: {1} -> {2}", ((double)transaction.Amount) / 100, transaction.From, transaction.To);
        }


        [ServerMessageHandler( ServerMessageType.CASH_DEDUCT )]
        [SessionRequired]
        public static void CashDeduct(string session, byte[] content, BinaryReader reader, BinaryWriter writer)
        {
            var sessiondata = SessionManager.GrabAccount(session);
            var save = Program.GetSaveFromPrl(sessiondata.SaveID);
            ServerResponseType result = ServerResponseType.REQ_ERROR;
            if (save != null)
            {
                long amount = reader.ReadInt64();
                string to = reader.ReadString();
                if(save.SystemDescriptor != null)
                {
                    if(save.SystemDescriptor.Cash >= amount)
                    {
                        result =  ServerResponseType.REQ_SUCCESS; //We can deduct.
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
            writer.Write((int)result);
            writer.Write(session);
        }
    }
}
