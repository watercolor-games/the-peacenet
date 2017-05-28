using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Objects
{
    public enum Reputation
    {
        Saint = 5,
        Moral = 4,
        Trustworthy = 3,
        WellKnown = 2,
        Respected = 1,
        Neutral = 0,
        Disrespected = -1,
        Criminal = -2,
        Untrustworthy = -3,
        Immoral = -4,
        Outcast = -5
    }

    public enum UserClass
    {
        /// <summary>
        /// The user has no class.
        /// </summary>
        None = 0,

        /// <summary>
        /// Skinners, otherwise known as "Shifters" due to their excessive use of the Shifter application, like to customize ShiftOS to look like other operating systems or even have an entirely different UI. They gain heaps of codepoints from it, and like to sell their skins for even more Codepoints.
        /// </summary>
        Skinner = 1,

        /// <summary>
        /// Hackers are notorious for taking down large groups and individuals of which have many useful documents and Codepoints on their system. Hackers enjoy the rush of typing malicious commands into their terminals and seeing how they affect their target.
        /// </summary>
        Hacker = 2,
        /// <summary>
        /// Much like hackers, investigators are skilled with a terminal and breaching systems, but they don't do it directly for monetary gain. They will search a target's system for any files and clues that may lead to them being guilty of a crime within the digital society. Unlike Hackers, Investigators mostly have higher reputations in society, and go after those with lower reputations.
        /// </summary>
        Investigator = 3,
        /// <summary>
        /// Explorers like to venture the vast regions of the multi-user domain and Shiftnet looking for secrets, hidden tools and software, and finding the hidden truths behind their screen. Explorers don't always know how to hack, but if it involves finding a secret about ShiftOS, they will do it. They typically do not have malicious intent.
        /// </summary>
        Explorer = 4,
        /// <summary>
        /// Safety Activists are skilled with exploitation and hacking, but they only go after the worst there is in the multi-user domain. Crime rings, large hacker groups, you name it. Their primary goal is keeping the multi-user domain safe.
        /// </summary>
        SafetyActivist = 5,
        /// <summary>
        /// Penetration testers go hand-in-hand with Safety Activists. They go after the good guys, but rather than attacking them, they alert them that an exploit was found in their service and that this exploit should be fixed. They are a gray subject though - you never know if you are dealing with a genuine pen-tester or a hacker skilled with social engineering. Be careful.
        /// </summary>
        PenetrationTester = 6,
        /// <summary>
        /// Collectors go well with Explorers - however, Collectors are the ones who open shops. They like to find rare objects and sell them for Codepoints.
        /// </summary>
        Collector = 7,
        /// <summary>
        /// Programmers are the ones who write applications and services for ShiftOS and the multi-user domain. Depending on the code that they write, they can be seen as either morally wrong sentiences or morally correct sentiences, it's up to their decisions.
        /// </summary>
        Programmer = 8
    }
}
