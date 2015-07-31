using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ColossalFramework;
using UnityEngine;

namespace NetworkExtensions.Framework
{
    public class TextureManager
    {
        private static TextureManager s_instance;
        public static TextureManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new TextureManager();
                }

                return s_instance;
            }
        }

        readonly IDictionary<string, Texture2D> _allTextures = new Dictionary<string, Texture2D>();

        public void FindAndLoadAllTextures()
        {
            var modPath = Mod.GetPath().Replace("/", "\\");
            var modDirectory = new DirectoryInfo(modPath);

            foreach (var textureFile in modDirectory.GetFiles("*.png", SearchOption.AllDirectories))
            {
                var relativePath = textureFile.FullName.Replace(modPath, "").TrimStart('\\');

                if (_allTextures.ContainsKey(relativePath))
                {
                    continue;
                }

                if (textureFile.FullName.ToLower().Contains(".dxt"))
                {
                    _allTextures[relativePath] = LoadTextureDXT(textureFile.FullName);
                }
                else
                {
                    _allTextures[relativePath] = LoadTexture(textureFile.FullName);
                }
            }
        }

        private static Texture2D LoadTexture(string fullPath)
        {
            var texture = new Texture2D(1, 1);
            texture.LoadImage(File.ReadAllBytes(fullPath));
            texture.anisoLevel = 8;
            texture.filterMode = FilterMode.Trilinear;

            return texture;
        }

        private static Texture2D LoadTextureDXT(string fullPath)
        {
            var texture = new Texture2D(1, 1, TextureFormat.DXT1, false);
            texture.LoadImage(File.ReadAllBytes(fullPath));
            texture.anisoLevel = 8;
            texture.filterMode = FilterMode.Trilinear;

            return texture;
        }

        public Texture2D GetTexture(string path)
        {
            if (path.IsNullOrWhiteSpace())
            {
                return null;
            }

            var trimmedPath = path.Replace("/", "\\");

            if (!_allTextures.ContainsKey(trimmedPath))
            {
                throw new Exception(string.Format("NExt: Texture {0} not found", trimmedPath));
            }

            return _allTextures[trimmedPath];
        }
    }
}
