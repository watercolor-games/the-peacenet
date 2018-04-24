using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peacenet.CountryChallenges
{
    public interface ICountryChallenge
    {
        string Name { get; }
        bool IsComplete { get; }
        string UIPercentageText { get; }
        float PercentageComplete { get; }
        Country Country { get; }
    }

    public class DummyChallengeOne : ICountryChallenge
    {
        public string Name => "Dummy challenge 1";

        public bool IsComplete => false;

        public string UIPercentageText => null;

        public float PercentageComplete => 0;

        public Country Country => Country.Elytrose;
    }
}
