using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = System.Object;

namespace NetworkExtensions.Framework.Extensions
{
    public static class MaterialExtensions
    {
        public static Material Clone(this Material originalMaterial, TexturesSet newTextures = null)
        {
            var material = new Material(originalMaterial);

            if (newTextures != null)
            {
                material.SetTexture("_MainTex", newTextures.MainTex);

                if (newTextures.XYSMap != null)
                {
                    material.SetTexture("_XYSMap", newTextures.XYSMap);
                }

                if (newTextures.APRMap != null)
                {
                    material.SetTexture("_APRMap", newTextures.APRMap);
                }
            }

            return material;
        }

        public static Texture2D Clone(this Texture2D original)
        {
            var destTex = new Texture2D(original.width, original.height, original.format, false);
            destTex.anisoLevel = destTex.anisoLevel;
            destTex.filterMode = destTex.filterMode;
            destTex.hideFlags = destTex.hideFlags;
            destTex.mipMapBias = destTex.mipMapBias;
            destTex.name = destTex.name + " (Clone)";
            destTex.wrapMode = destTex.wrapMode;
            destTex.LoadRawTextureData(original.GetRawTextureData());
            destTex.Apply();

            return destTex;
        }
    }
}
