using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountersPlus.UI.ViewControllers
{
    class ContributorsAndDonators
    {
        internal static Dictionary<string, string> Contributors = new Dictionary<string, string>()
        {
            { "Moon", "Bug fixing and code optimization" },
            { "Shoko84", "Bug fixing" },
            { "xhuytox", "Big early bug hunter - thanks man!" },
            { "Brian", "CustomUI, which makes this UI possible" },
            { "Viscoci", "Text helper & YURFit UI code" },
            { "monkeymanboy", "Custom Campaign's UI code" },
            { "Assistant", "Custom Avatars UI code" },
            { "Kyle1413", "BS Utils, Counters code" },
            { "Ragesaq", "Speed Counter and Spinometer idea" },
            { "SkyKiwiTV", "Original version checking code" },
            { "Dracrius", "Bug fixing and beta testing" },
            { "guyyst", "Cut's \"Separate Saber Counts\" option." },
            { "Steven", "Revitalized Custom Counters" },
            { "Prspiri", "Internal code refactoring" },
        };

        internal static Dictionary<string, string> Donators = new Dictionary<string, string>()
        {
            { "altrewin", "Thanks for the love, and coffee. <3" },
            { "Latest", "Sorry I was late on thanking you, here you go!" },
            { "Jackz", "C# is better." },
            { "Cade", "Thanks for the donation! I'm always here if you have issues." },
            { "Someone", "To all the people who remain anonymous, I thank you." },
        };

    }
}
