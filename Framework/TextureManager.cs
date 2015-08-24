using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ColossalFramework;
using UnityEngine;

namespace NetworkExtensions.Framework
{
    public class TextureManager : Singleton<TextureManager>
    {
        readonly IDictionary<string, Texture2D> _allTextures = new Dictionary<string, Texture2D>();

        public void FindAndLoadAllTextures()
        {
            var modPath = Mod.GetPath();
            var modDirectory = new DirectoryInfo(modPath);

            var files = new List<FileInfo>();
            files.AddRange(modDirectory.GetFiles("*.png", SearchOption.AllDirectories));
            files.AddRange(modDirectory.GetFiles("*.dds", SearchOption.AllDirectories));

            foreach (var textureFile in files)
            {
                var relativePath = textureFile.FullName.Replace(modPath, "").TrimStart(new []{'\\', '/'});

                Debug.Log(string.Format("NExt: Loading {0}", textureFile.FullName));
                Debug.Log(string.Format("NExt: Relative Path {0}", relativePath));

                if (_allTextures.ContainsKey(relativePath))
                {
                    continue;
                }

                if (textureFile.Extension.ToLower() == ".dds")
                {
                    _allTextures[relativePath] = LoadTextureDDS(textureFile.FullName);
                }
                else
                {
                    _allTextures[relativePath] = LoadTexturePNG(textureFile.FullName);
                }
            }
        }

        private static Texture2D LoadTexturePNG(string fullPath)
        {
            var texture = new Texture2D(1, 1);
            texture.LoadImage(File.ReadAllBytes(fullPath));
            texture.anisoLevel = 8;
            texture.filterMode = FilterMode.Trilinear;

            return texture;
        }

        private static Texture2D LoadTextureDDS(string fullPath)
        {
            var numArray = File.ReadAllBytes(fullPath);
            var width = BitConverter.ToInt32(numArray, 16);
            var height = BitConverter.ToInt32(numArray, 12);

            var texture = new Texture2D(width, height, TextureFormat.DXT5, true);
            var list = new List<byte>();

            for (int index = 0; index < numArray.Length; ++index)
            {
                if (index > (int)sbyte.MaxValue)
                    list.Add(numArray[index]);
            }

            texture.LoadRawTextureData(list.ToArray());
            texture.Apply();
            texture.anisoLevel = 8;
            return texture;
        }


        public Texture2D GetTexture(string path)
        {
            if (path.IsNullOrWhiteSpace())
            {
                return null;
            }

            var trimmedPath = path.Replace('\\', Path.DirectorySeparatorChar);

            if (!_allTextures.ContainsKey(trimmedPath))
            {
                throw new Exception(String.Format("NExt: Texture {0} not found", trimmedPath));
            }

            return _allTextures[trimmedPath];
        }
    }
}
