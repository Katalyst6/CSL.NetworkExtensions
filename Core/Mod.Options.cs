using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColossalFramework.IO;
using ColossalFramework.Steamworks;
using ICities;
using NetworkExtensions.Framework;

namespace NetworkExtensions
{
    public partial class Mod : IUserMod
    {
        public void OnSettingsUI(UIHelperBase helper)
        {
            var uIHelperBase = helper.AddGroup("Network Extensions Options");
            var optionsChanged = false;

            foreach (var part in Parts.OrderBy(p => p.OptionsPriority))
            {
                var partLocal = part;
                var partName = part.GetSerializableName();

                if (!Options.Instance.PartsEnabled.ContainsKey(partName))
                {
                    Options.Instance.PartsEnabled[partName] = true;
                    optionsChanged = true;
                }

                uIHelperBase.AddCheckbox(
                    part.DisplayName, 
                    Options.Instance.PartsEnabled[partName], 
                    isChecked =>
                    {
                        Options.Instance.PartsEnabled[partName] = partLocal.IsEnabled = isChecked;
                        Options.Instance.Save();

                        s_netInfoBuilders = null;
                        s_netInfoModifiers = null;
                    });
            }

            if (optionsChanged)
            {
                Options.Instance.Save();
            }
        }
    }
}
