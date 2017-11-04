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
    [Obsolete("Cash management should be done on the server.")]
    public static class CashManager
    {
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
