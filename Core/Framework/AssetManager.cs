using System;
using System.Collections.Generic;
using System.IO;
using ColossalFramework;
using ObjUnity3D;
using UnityEngine;

namespace NetworkExtensions.Framework
{
    public class AssetManager : Singleton<AssetManager>
    {
#if DEBUG
        private readonly ICollection<Texture2D> _specialTextures = new List<Texture2D>();
        public ICollection<Texture2D> SpecialTextures { get { return _specialTextures; } }
#endif

        private readonly IDictionary<string, Texture2D> _allTextures = new Dictionary<string, Texture2D>();
        private readonly IDictionary<string, Mesh> _allMeshes = new Dictionary<string, Mesh>();

        public void FindAndLoadAllTextures()
        {
            var modPath = Mod.GetPath();
            var modDirectory = new DirectoryInfo(modPath);

            var files = new List<FileInfo>();
            files.AddRange(modDirectory.GetFiles("*.png", SearchOption.AllDirectories));
            files.AddRange(modDirectory.GetFiles("*.dds", SearchOption.AllDirectories));
            files.AddRange(modDirectory.GetFiles("*.obj", SearchOption.AllDirectories));

            foreach (var assetFile in files)
            {
                var relativePath = assetFile.FullName.Replace(modPath, "").TrimStart(new []{'\\', '/'});

                if (_allTextures.ContainsKey(relativePath))
                {
                    continue;
                }

                switch (assetFile.Extension.ToLower())
                {
                    case ".dds":
                        _allTextures[relativePath] = LoadTextureDDS(assetFile.FullName, assetFile.Name);
                        break;

                    case ".png":
                        _allTextures[relativePath] = LoadTexturePNG(assetFile.FullName, assetFile.Name);
                        break;

                    case ".obj":
                        _allMeshes[relativePath] = LoadMesh(assetFile.FullName, assetFile.Name);
                        break;
                }
            }
        }

        private static Texture2D LoadTexturePNG(string fullPath, string textureName)
        {
            var texture = new Texture2D(1, 1);
            texture.name = Path.GetFileNameWithoutExtension(textureName);
            texture.LoadImage(File.ReadAllBytes(fullPath));
            texture.anisoLevel = 8;
            texture.filterMode = FilterMode.Trilinear;
            texture.Apply();
            return texture;
        }

        private static Texture2D LoadTextureDDS(string fullPath, string textureName)
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
            texture.anisoLevel = 8;
            texture.name = Path.GetFileNameWithoutExtension(textureName);
            texture.Apply();
            return texture;
        }

        private static Mesh LoadMesh(string fullPath, string meshName)
        {
            var mesh = new Mesh();
            using (var fileStream = File.Open(fullPath, FileMode.Open))
            {
                mesh.LoadOBJ(OBJLoader.LoadOBJ(fileStream));
            }
            mesh.Optimize();
            mesh.name = Path.GetFileNameWithoutExtension(meshName);

            return mesh;
        }

        public Texture2D GetTexture(string path)
        {
            if (path.IsNullOrWhiteSpace())
            {
                return null;
            }

            var trimmedPath = path
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);

            if (!_allTextures.ContainsKey(trimmedPath))
            {
                throw new Exception(String.Format("NExt: Texture {0} not found", trimmedPath));
            }

            return _allTextures[trimmedPath];
        }

        public Mesh GetMesh(string path)
        {
            if (path.IsNullOrWhiteSpace())
            {
                return null;
            }

            var trimmedPath = path
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);

            if (!_allMeshes.ContainsKey(trimmedPath))
            {
                throw new Exception(String.Format("NExt: Mesh {0} not found", trimmedPath));
            }

            return _allMeshes[trimmedPath];
        }
    }
}
