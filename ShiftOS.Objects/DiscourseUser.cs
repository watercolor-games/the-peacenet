using Discoursistency.HTTP.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discoursistency.HTTP.Client.Models;

namespace ShiftOS.Objects
{
    public class ShiftOSAuthAgent : Discoursistency.Base.Authentication.DiscourseAuthenticationService
    {
        public ShiftOSAuthAgent(IClient client) : base(client)
        {
        }
    }

}
