using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ColossalFramework;
using UnityEngine;
using UnityExtension;

namespace NetworkExtensions.Framework
{
    public class AssetManager : Singleton<AssetManager>
    {
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
                        _allTextures[relativePath] = LoadTextureDDS(assetFile.FullName);
                        break;

                    case ".png":
                        _allTextures[relativePath] = LoadTexturePNG(assetFile.FullName);
                        break;

                    case ".obj":
                        _allMeshes[relativePath] = LoadMesh(assetFile.FullName);
                        break;
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

        private static Mesh LoadMesh(string fullPath)
        {
            var mesh = new Mesh();
            using (var fileStream = File.Open(fullPath, FileMode.Open))
            {
                mesh.LoadOBJ(OBJLoader.LoadOBJ(fileStream));
            }
            mesh.Optimize();

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

        // Test that for mesh Import
        //public static void TestFromBoFormer()
        //{
        //    // the default assets import path
        //    var path = Path.Combine(DataLocation.addonsPath, "Import");
        //    var modelName = "Test.fbx";

        //    // load model
        //    var importer = new SceneImporter();
        //    importer.filePath = Path.Combine(path, modelName);
        //    importer.importSkinMesh = true;
        //    var importedModel = importer.Import();

        //    // load textures
        //    AssetImporterTextureLoader.LoadTextures(null, importedModel, results, path, modelName, false);
        //}
    }
}
