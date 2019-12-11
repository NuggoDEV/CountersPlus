using UnityEngine;
using CountersPlus.Utils;

namespace CountersPlus.UI.Images
{
    class Images
    {
        public static Texture2D LoadTexture(string name)
        {
            return UIUtilities.LoadTextureFromResources($"CountersPlus.UI.Images.{name}.png");
        }

        public static Sprite LoadSprite(string name)
        {
            return UIUtilities.LoadSpriteFromResources($"CountersPlus.UI.Images.{name}.png");
        }
    }
}
