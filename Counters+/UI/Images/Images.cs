using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomUI.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace CountersPlus.UI.Images
{
    class Images
    {
        public static Sprite Load(string name)
        {
            return UIUtilities.LoadSpriteFromResources($"CountersPlus.UI.Images.{name}.png");
        }
    }
}
