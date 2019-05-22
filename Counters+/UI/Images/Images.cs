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
        public static Texture Load(string name)
        { //It's not easy without Sprite cranberry.
            return UIUtilities.LoadTextureFromResources($"CountersPlus.UI.Images.{name}.png");
        }
    }
}
