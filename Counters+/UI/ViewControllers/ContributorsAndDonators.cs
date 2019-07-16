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
            { "xhuytox", "Big helper in bug hunting - thanks man!" },
            { "Brian", "Saving Beat Saber Modding with CustomUI" },
            { "Viscoci", "Helper code that fixed displaying text and saved my bacon" },
            { "Assistant", "Stole some Custom Avatars UI code to help with settings" },
            { "Kyle1413", "Beat Saber Utils and for Progress/Score Counter code" },
            { "Ragesaq", "Speed Counter and Spinometer idea <i>(and some bug fixing on stream)</i>" },
            { "SkyKiwiTV", "Original version checking code" },
            { "Dracrius", "Bug fixing and beta testing" },
        };

        internal static Dictionary<string, string> Donators = new Dictionary<string, string>()
        {
            { "altrewin", "Thanks for the love, and coffee. <3" },
            { "Latest", "Sorry I was late on thanking you, here you go!" },
            { "Someone", "To all the people who remain anonymous, I thank you." },
        };

    }
}
