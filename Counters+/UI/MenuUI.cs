using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomUI.BeatSaber;

namespace CountersPlus.UI
{
    public class MenuUI
    {
        static CustomMenu settings;
        static CustomListViewController viewController;
        public static void CreateUI()
        {
            settings = BeatSaberUI.CreateCustomMenu<CustomMenu>("Counters+");
            viewController = BeatSaberUI.CreateViewController<CustomListViewController>();
        }
    }
}
