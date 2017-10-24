using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plex.Objects;

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
            using (var w = new ServerStream(ServerMessageType.CASH_DEDUCT))
            {
                w.Write(to);
                w.Write(amount);
                var result = w.Send();
                if (result.Message == 0x00)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
