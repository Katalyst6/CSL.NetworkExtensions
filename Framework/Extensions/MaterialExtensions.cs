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
    }
}
