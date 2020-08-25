using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CountersPlus.Utils
{
    public class ImagesUtility
    {
        private static Material _noGlowMaterial = null;
        public static Material NoGlowMaterial
        {
            get
            {
                if (!_noGlowMaterial)
                    _noGlowMaterial = new Material(Resources.FindObjectsOfTypeAll<Material>().First(m => m.name == "UINoGlow"));
                return _noGlowMaterial;
            }
        }

        private static Sprite _blankSprite = null;
        public static Sprite BlankSprite
        {
            get
            {
                if (!_blankSprite)
                    _blankSprite = Sprite.Create(Texture2D.blackTexture, new Rect(), Vector2.zero);
                return _blankSprite;
            }
        }

        public static Texture2D LoadTextureRaw(byte[] file)
        {
            if (file.Count() > 0)
            {
                Texture2D Tex2D = new Texture2D(2, 2);
                Tex2D.LoadImage(file);
                return Tex2D;
            }
            return null;
        }

        public static Texture2D LoadTextureFromFile(string FilePath)
        {
            if (File.Exists(FilePath))
                return LoadTextureRaw(File.ReadAllBytes(FilePath));

            return null;
        }

        public static Texture2D LoadTextureFromResources(string resourcePath)
        {
            return LoadTextureRaw(GetResource(Assembly.GetCallingAssembly(), resourcePath));
        }

        public static Sprite LoadSpriteRaw(byte[] image, float PixelsPerUnit = 100.0f)
        {
            return LoadSpriteFromTexture(LoadTextureRaw(image), PixelsPerUnit);
        }

        public static Sprite LoadSpriteFromTexture(Texture2D SpriteTexture, float PixelsPerUnit = 100.0f)
        {
            if (SpriteTexture)
                return Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), Vector2.one * 0.5f, PixelsPerUnit);
            return null;
        }

        public static Sprite LoadSpriteFromFile(string FilePath, float PixelsPerUnit = 100.0f)
        {
            return LoadSpriteFromTexture(LoadTextureFromFile(FilePath), PixelsPerUnit);
        }

        public static Sprite LoadSpriteFromResources(string resourcePath, float PixelsPerUnit = 100.0f)
        {
            return LoadSpriteRaw(GetResource(Assembly.GetCallingAssembly(), resourcePath), PixelsPerUnit);
        }

        public static Sprite LoadSpriteFromExternalAssemblyResources(Assembly assembly, string resourcePath, float pixelsPerUnit = 100f)
        {
            return LoadSpriteRaw(GetResource(assembly, resourcePath), pixelsPerUnit);
        }

        public static byte[] GetResource(Assembly asm, string ResourceName)
        {
            System.IO.Stream stream = asm.GetManifestResourceStream(ResourceName);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }

        public static void PrintHierarchy(Transform transform, string spacing = "|-> ")
        {
            spacing = spacing.Insert(1, "  ");
            var tempList = transform.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                Console.WriteLine($"{spacing}{child.name}");
                PrintHierarchy(child, "|" + spacing);
            }
        }
    }
}
