using System.IO;
using ColossalFramework.IO;
using ColossalFramework.Steamworks;
using ICities;
using UnityEngine;

#if DEBUG
using Debug = NetworkExtensions.Framework.Debug;
#endif

namespace NetworkExtensions
{
    public partial class Mod : IUserMod
    {
        public const string PATH_NOT_FOUND = "NOT_FOUND";

        private static string s_path = null;
        public static string GetPath()
        {
            if (s_path == null)
            {
                s_path = CheckForPath();

                if (s_path != PATH_NOT_FOUND)
                {
                    Debug.Log("NExt: Mod path " + s_path);
                }
                else
                {
                    Debug.Log("NExt: Path not found");
                }
            }

            return s_path;
        }

        private static string CheckForPath()
        {
            // 1. Check Local path (CurrentUser\Appdata\Local\Colossal Order\Cities_Skylines\Addons\Mods)
            var localPath = Path.Combine(DataLocation.modsPath, "NetworkExtensions");
            Debug.Log(string.Format("NExt: Exist={0} DataLocation.modsPath={1}", Directory.Exists(localPath), localPath));

            if (Directory.Exists(localPath))
            {
                return localPath;
            }

            // 2. Check Steam
            foreach (var mod in Steam.workshop.GetSubscribedItems())
            {
                if (mod.AsUInt64 == WORKSHOP_ID)
                {
                    var workshopPath = Steam.workshop.GetSubscribedItemPath(mod);
                    Debug.Log(string.Format("NExt: Exist={0} WorkshopPath={1}", Directory.Exists(workshopPath), workshopPath));
                    if (Directory.Exists(workshopPath))
                    {
                        return workshopPath;
                    }
                }
            }

            // 3. Check Cities Skylines files folder
            var csFolderPath = Path.Combine(Path.Combine(DataLocation.gameContentPath, "Mods"), "NetworkExtensions");
            Debug.Log(string.Format("NExt: Exist={0} DataLocation.gameContentPath={1}", Directory.Exists(csFolderPath), csFolderPath));
            if (Directory.Exists(csFolderPath))
            {
                return csFolderPath;
            }

            return PATH_NOT_FOUND;
        }
    }
}
