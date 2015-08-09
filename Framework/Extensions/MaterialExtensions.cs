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
        public static Material Clone(this Material originalMaterial)
        {
            return new Material(originalMaterial);
        }

        public static void SetTextures(this Material originalMaterial, TexturesSet newTextures)
        {
            if (newTextures != null)
            {
                originalMaterial.SetTexture("_MainTex", newTextures.MainTex);

                if (newTextures.XYSMap != null)
                {
                    originalMaterial.SetTexture("_XYSMap", newTextures.XYSMap);
                }

                if (newTextures.APRMap != null)
                {
                    originalMaterial.SetTexture("_APRMap", newTextures.APRMap);
                }
            }
        }
    }
}
