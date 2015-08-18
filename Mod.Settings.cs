//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using ColossalFramework.IO;
//using ColossalFramework.Steamworks;
//using ICities;
//using NetworkExtensions.Framework;

//namespace NetworkExtensions
//{
//    public partial class Mod : IUserMod
//    {
//        public void OnSettingsUI(UIHelperBase helper)
//        {
//            AmericanRoads.config = Configuration.Deserialize(AmericanRoads.configPath);
//            if (AmericanRoads.config == null)
//            {
//                AmericanRoads.config = new Configuration();
//            }
//            AmericanRoads.SaveConfig();
//            UIHelperBase uIHelperBase = helper.AddGroup("American Roads Settings");
//            uIHelperBase.AddCheckbox("Remove optional lane arrows", AmericanRoads.config.disable_optional_arrows, new OnCheckChanged(this.EventCheckArrows));
//            uIHelperBase.AddCheckbox("Use alternate pavement texture", AmericanRoads.config.use_alternate_pavement_texture, new OnCheckChanged(this.EventCheckPavement));
//            uIHelperBase.AddCheckbox("Cracked roads?", AmericanRoads.config.use_cracked_roads, new OnCheckChanged(this.EventCheckCrack));
//            uIHelperBase.AddSlider("Crack intensity", 0.5f, 2.5f, 0.1f, AmericanRoads.config.crackIntensity, new OnValueChanged(this.EventSlideCrack));
//        }
//    }
//}
