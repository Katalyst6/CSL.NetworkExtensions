using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetworkExtensions.Framework.Extensions
{
    public static class MaterialExtensions
    {
        public static Material Clone(this Material originalMaterial, TexturesSet newTextures, bool lodTextures = false)
        {
            var material = UnityEngine.Object.Instantiate(originalMaterial);

            if (!lodTextures)
            {
                {
                    material.ModifyTexture("_MainTex", newTextures.MainTex);
                }

                if (newTextures.APRMap != null)
                {
                    material.ModifyTexture("_APRMap", newTextures.APRMap);
                }

                if (newTextures.XYSMap != null)
                {
                    material.ModifyTexture("_XYSMap", newTextures.XYSMap);
                }
            }
            else
            {
                material.SetTexture("_MainTex", newTextures.MainTex);
                material.SetTexture("_APRMap", newTextures.APRMap);
                material.SetTexture("_XYSMap", newTextures.XYSMap);
            }

            return material;
        }

        private static void ModifyTexture(this Material material, string propertyName, Texture2D newTexture)
        {
            var currentTexture = material.GetTexture(propertyName) as Texture2D;

            if (currentTexture == null)
            {
                return;
            }

            var needCompression = currentTexture.format == TextureFormat.DXT1 ||
                                  currentTexture.format == TextureFormat.DXT5;

            if (!needCompression)
            {
                needCompression = newTexture.format != TextureFormat.DXT1 &&
                                  newTexture.format != TextureFormat.DXT5;
            }

            if (needCompression)
            {
                newTexture.Compress(false);
            }

            material.SetTexture(propertyName, newTexture);
        }

        private static int s_texId = 0;
        private static Texture Clone(this Texture originalTexture, Texture2D newTexture)
        {
            var originalTexture2D = (Texture2D)originalTexture;

            //UnityEngine.Object.Instantiate(originalTexture as Texture2D);
            var texture = new Texture2D(originalTexture.width, originalTexture.height, originalTexture2D.format, true);
            texture.name = "Tex" + s_texId++;

            //if (newTexture != null)
            //{
                //texture.SetPixels32(newTexture.GetPixels32());
                //texture.Apply();
                
                for (var mip = 0; mip < newTexture.mipmapCount; ++mip)
                {
                    var cols = newTexture.GetPixels32(mip);
                    texture.SetPixels32(cols, mip);
                }

                // actually apply all SetPixels32, don't recalculate mip levels
                texture.Apply();
            //}


            return texture;
        }

        private static Texture DefaultClone(this Texture originalTexture)
        {
            var texture = UnityEngine.Object.Instantiate(originalTexture);
            texture.name = "Tex" + s_texId++;

            return texture;
        }
    }
}
