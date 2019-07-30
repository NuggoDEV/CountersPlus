using CustomUI.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace CountersPlus.UI.Images
{
    class Images
    {
        public static Texture LoadTexture(string name)
        { //It's not easy without Sprite cranberry.
            return UIUtilities.LoadTextureFromResources($"CountersPlus.UI.Images.{name}.png");
        }

        public static Sprite LoadSprite(string name)
        {
            return UIUtilities.LoadSpriteFromResources($"CountersPlus.UI.Images.{name}.png");
        }
    }
}
