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
            var modPath = Mod.GetPath().Replace("/", "\\");
            var modDirectory = new DirectoryInfo(modPath);

            var files = new List<FileInfo>();
            files.AddRange(modDirectory.GetFiles("*.png", SearchOption.AllDirectories));
            files.AddRange(modDirectory.GetFiles("*.dds", SearchOption.AllDirectories));

            foreach (var textureFile in files)
            {
                var relativePath = textureFile.FullName.Replace(modPath, "").TrimStart('\\');

                if (_allTextures.ContainsKey(relativePath))
                {
                    continue;
                }

                if (textureFile.Extension.ToLower() == ".dds")
                {
                    _allTextures[relativePath] = LoadTextureDDS_DXT1(textureFile.FullName);
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

        private static Texture2D LoadTextureDDS_DXT5(string fullPath)
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

        private static Texture2D LoadTextureDDS_DXT1(string fullPath)
        {
            var ddsBytes = File.ReadAllBytes(fullPath);

            var ddsSizeCheck = ddsBytes[4];
            if (ddsSizeCheck != 124)
                throw new Exception("Invalid DDS DXTn texture. Unable to read");  //this header byte should be 124 for DDS image files

            var height = ddsBytes[13] * 256 + ddsBytes[12];
            var width = ddsBytes[17] * 256 + ddsBytes[16];

            const int DDS_HEADER_SIZE = 128;
            var dxtBytes = new byte[ddsBytes.Length - DDS_HEADER_SIZE];
            Buffer.BlockCopy(ddsBytes, DDS_HEADER_SIZE, dxtBytes, 0, ddsBytes.Length - DDS_HEADER_SIZE);

            var texture = new Texture2D(width, height, TextureFormat.DXT1, true);
            texture.anisoLevel = 8;
            texture.filterMode = FilterMode.Trilinear;
            texture.LoadRawTextureData(dxtBytes);
            texture.Apply();

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
                throw new Exception(String.Format("NExt: Texture {0} not found", trimmedPath));
            }

            return _allTextures[trimmedPath];
        }
    }
}
