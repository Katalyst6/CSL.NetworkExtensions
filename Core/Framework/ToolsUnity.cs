using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NetworkExtensions.Framework
{
    public static class ToolsUnity
    {
        public static void Compare<T>(T unityObj, T otherUnityObj)
             where T : Object
        {
            Debug.Log(string.Format("NExt: ----->  Comparing {0} with {1}", unityObj.name, otherUnityObj.name));

            var fields = typeof(T).GetAllFieldsFromType();

            foreach (var f in fields)
            {
                var newValue = f.GetValue(unityObj);
                var oldValue = f.GetValue(otherUnityObj);

                if (!Equals(newValue, oldValue))
                {
                    Debug.Log(string.Format("Value {0} not equal (N-O) ({1},{2})", f.Name, newValue, oldValue));
                }
            }
        }

        public static void ListMembers<T>(T unityObj)
            where T : Object
        {
            Debug.Log(string.Format("NExt: ----->  Listing {0}", unityObj.name));

            var fields = typeof(T).GetAllFieldsFromType();

            foreach (var f in fields)
            {
                var value = f.GetValue(unityObj);
                Debug.Log(string.Format("Member name \"{0}\" value is \"{1}\"", f.Name, value));
            }
        }

        //=========================================================================
        // Methods created by petrucio -> http://answers.unity3d.com/questions/238922/png-transparency-has-white-borderhalo.html
        //
        // Copy the values of adjacent pixels to transparent pixels color info, to
        // remove the white border artifact when importing transparent .PNGs.
        public static void FixTransparency(this Texture2D texture)
        {
            Color32[] pixels = texture.GetPixels32();
            int w = texture.width;
            int h = texture.height;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int idx = y * w + x;
                    Color32 pixel = pixels[idx];
                    if (pixel.a == 0)
                    {
                        bool done = false;
                        if (!done && x > 0) done = TryAdjacent(ref pixel, pixels[idx - 1]);        // Left   pixel
                        if (!done && x < w - 1) done = TryAdjacent(ref pixel, pixels[idx + 1]);        // Right  pixel
                        if (!done && y > 0) done = TryAdjacent(ref pixel, pixels[idx - w]);        // Top    pixel
                        if (!done && y < h - 1) done = TryAdjacent(ref pixel, pixels[idx + w]);        // Bottom pixel
                        pixels[idx] = pixel;
                    }
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();
        }

        private static bool TryAdjacent(ref Color32 pixel, Color32 adjacent)
        {
            if (adjacent.a == 0) return false;

            pixel.r = adjacent.r;
            pixel.g = adjacent.g;
            pixel.b = adjacent.b;
            return true;
        }
        //=========================================================================

        public static UITextureAtlas LoadToolThumbnails(string thumbnailsName, string thumbnailsPath)
        {
            var thumbnailAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            thumbnailAtlas.padding = 0;
            thumbnailAtlas.name = thumbnailsName;

            var shader = Shader.Find("UI/Default UI Shader");
            if (shader != null) thumbnailAtlas.material = new Material(shader);



            var texture = AssetManager.instance.GetTexture(thumbnailsPath);
            texture.FixTransparency();

            thumbnailAtlas.material.mainTexture = texture;

            const int iconW = 109;
            const int iconH = 100;

            const int textureW = iconW * 5;
            const int textureH = 100;


            string[] ts = { "", "Disabled", "Focused", "Hovered", "Pressed" };
            for (int x = 0; x < ts.Length; ++x)
            {
                var sprite = new UITextureAtlas.SpriteInfo
                {
                    name = string.Format(thumbnailsName.ToUpper() + "{0}", ts[x]),
                    region = new Rect(
                        (float)(x * iconW) / textureW, 0f,
                        (float)(iconW) / textureW, (float)(iconH) / textureH),
                    texture = new Texture2D(iconW, iconH, TextureFormat.ARGB32, false)
                };

                thumbnailAtlas.AddSprite(sprite);
            }

            return thumbnailAtlas;
        }

        private static UITextureAtlas s_thumbnailAtlas = null;

        public static UITextureAtlas LoadMenuThumbnails()
        {
            if (s_thumbnailAtlas != null)
            {
                return s_thumbnailAtlas;
            }

            var thumbnailAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            thumbnailAtlas.padding = 0;
            thumbnailAtlas.name = "AdditionnalSubBar";

            var shader = Shader.Find("UI/Default UI Shader");
            if (shader != null) thumbnailAtlas.material = new Material(shader);

            const string PATH = @"Menus\Textures\AdditionnalSubBar.png";

            const string BASE = "SubBarButtonBase";
            const string ROADS_SMALL_HV = "SubBar" + Menus.AdditionnalMenus.ROADS_SMALL_HV;

            var versions = new[] { "", "Disabled", "Focused", "Hovered", "Pressed" };


            var texture = AssetManager.instance.GetTexture(PATH);
            texture.FixTransparency();

            thumbnailAtlas.material.mainTexture = texture;

            var x = 1;
            var y = 1;

            const int TEXTURE_W = 292;
            const int TEXTURE_H = 50;



            // Base -------------------------------------------------------------------------------
            const int BASE_ICON_W = 58;
            const int BASE_ICON_H = 25;

            foreach (var t in versions)
            {
                var sprite = new UITextureAtlas.SpriteInfo
                {
                    name = string.Format(BASE + "{0}", t),
                    region = new Rect(
                        (float)(x) / TEXTURE_W,
                        (float)(y) / TEXTURE_H,
                        (float)(BASE_ICON_W) / TEXTURE_W,
                        (float)(BASE_ICON_H) / TEXTURE_H),
                    texture = new Texture2D(BASE_ICON_W, BASE_ICON_H, TextureFormat.ARGB32, false)
                };

                thumbnailAtlas.AddSprite(sprite);

                x += BASE_ICON_W;
            }

            x = 1;
            y += BASE_ICON_H + 1;



            // RoadsSmallHV -----------------------------------------------------------------------
            const int ICON_W = 32;
            const int ICON_H = 22;

            foreach (var t in versions)
            {
                var sprite = new UITextureAtlas.SpriteInfo
                {
                    name = string.Format(ROADS_SMALL_HV + "{0}", t),
                    region = new Rect(
                        (float)(x) / TEXTURE_W,
                        (float)(y) / TEXTURE_H,
                        (float)(ICON_W) / TEXTURE_W,
                        (float)(ICON_H) / TEXTURE_H),
                    texture = new Texture2D(ICON_W, ICON_H, TextureFormat.ARGB32, false)
                };

                thumbnailAtlas.AddSprite(sprite);

                x += ICON_W;
            }

            s_thumbnailAtlas = thumbnailAtlas;

            return s_thumbnailAtlas;
        }

        public static UITextureAtlas LoadInfoTooltip(string infoTooltipName, string infoTooltipPath)
        {
            var infoTooltipAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            infoTooltipAtlas.padding = 0;
            infoTooltipAtlas.name = infoTooltipName;

            var shader = Shader.Find("UI/Default UI Shader");
            if (shader != null) infoTooltipAtlas.material = new Material(shader);

            var texture = AssetManager.instance.GetTexture(infoTooltipPath);

            infoTooltipAtlas.material.mainTexture = texture;

            const int ittW = 535;
            const int ittH = 150;

                var sprite = new UITextureAtlas.SpriteInfo
                {
                    name = string.Format(infoTooltipName.ToUpper()),
                    region = new Rect(0f, 0f, 1f, 1f),
                    texture = new Texture2D(ittW, ittH, TextureFormat.ARGB32, false)
                };

                infoTooltipAtlas.AddSprite(sprite);

            return infoTooltipAtlas;
        }
    }
}
