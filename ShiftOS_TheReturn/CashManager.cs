using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plex.Engine
{
    public static class CashManager
    {
        private static ICashProvider _provider = null;
        
        public static void Init(ICashProvider provider)
        {
            _provider = provider;
        }

        public static bool Deduct(long amount, string to)
        {
            if (amount == 0)
                return true;
            if (_provider == null)
                return false;
            return _provider.Deduct(amount, to);
        }

        public static bool Receive(long amount, string from)
        {
            if (amount == 0)
                return true;
            if (_provider == null)
                return false;
            return _provider.Receive(amount, from);
        }

    }

    public interface ICashProvider
    {
        bool Deduct(long cents, string to);
        bool Receive(long cents, string from);

    }
}
