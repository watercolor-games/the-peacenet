using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Plex.Engine
{
    public static class CashManager
    {
        private static bool? cash_deduct_state = null;

        [ClientMessageHandler("cash_deductresult"), AsyncExecution]
        public static void DeductResult(string content, string ip)
        {
            cash_deduct_state = (content == "1") ? true : false; //boy I love redundancy
            //fuck my life
        }

        public static bool Deduct(long amount, string to)
        {
            if (amount == 0)
                return true;
            cash_deduct_state = null;
            ServerManager.SendMessage("cash_deduct", JsonConvert.SerializeObject(new
            {
                cash = amount,
                to = to
            }));
            while (cash_deduct_state == null)
                Thread.Sleep(10);
            return (bool)cash_deduct_state;
        }
    }
}
