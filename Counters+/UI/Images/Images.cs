using CustomUI.Utilities;
using UnityEngine;

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
