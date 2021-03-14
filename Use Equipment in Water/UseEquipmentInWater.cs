using BepInEx;
using BepInEx.Configuration;
//using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;

namespace Use_Equipment_in_Water
{
    [BepInPlugin("com.lvh-it.valheim.useequipmentinwater", "Use Equipment in Water", "0.2.2.0")]
    [BepInProcess("valheim.exe")]
    [BepInProcess("valheim.x86_64")]
    class UseEquipmentInWater : BaseUnityPlugin
    {
        //Used for debugging
        //
        //private static ManualLogSource logger;
        private List<ConfigEntry<bool>> configEntries = new List<ConfigEntry<bool>>();
        private static List<string> deniedItems = new List<string>();
        private Items itemList = new Items();

        void Main()
        {
            //Used for debugging
            //
            //logger = Logger;

            configEntries.Add(Config.Bind("ItemsToAllow", "allowAxes", true, "Allow axes to be used in Water"));
            configEntries.Add(Config.Bind("ItemsToAllow", "allowBows", true, "Allow bows to be used in Water"));
            configEntries.Add(Config.Bind("ItemsToAllow", "allowAtgeirs", true, "Allow atgeirs to be used in Water"));
            configEntries.Add(Config.Bind("ItemsToAllow", "allowKnives", true, "Allow knives to be used in Water"));
            configEntries.Add(Config.Bind("ItemsToAllow", "allowMaces", true, "Allow maces to be used in Water"));
            configEntries.Add(Config.Bind("ItemsToAllow", "allowShields", true, "Allow shields to be used in Water"));
            configEntries.Add(Config.Bind("ItemsToAllow", "allowSledge", true, "Allow sledge to be used in Water"));
            configEntries.Add(Config.Bind("ItemsToAllow", "allowSpears", true, "Allow spears to be used in Water"));
            configEntries.Add(Config.Bind("ItemsToAllow", "allowSwords", true, "Allow swords to be used in Water"));
            configEntries.Add(Config.Bind("ItemsToAllow", "allowClub", true, "Allow club to be used in Water"));
            configEntries.Add(Config.Bind("ItemsToAllow", "allowTorch", true, "Allow torch to be used in Water"));
            configEntries.Add(Config.Bind("ItemsToAllow", "allowPickaxes", true, "Allow pickaxes to be used in Water"));
            configEntries.Add(Config.Bind("ItemsToAllow", "allowCultivator", true, "Allow cultivator to be used in Water"));
            configEntries.Add(Config.Bind("ItemsToAllow", "allowFishingRod", true, "Allow fishing rod to be used in Water"));
            configEntries.Add(Config.Bind("ItemsToAllow", "allowHammer", true, "Allow hammer to be used in Water"));
            configEntries.Add(Config.Bind("ItemsToAllow", "allowHoe", true, "Allow hoe to be used in Water"));

            foreach (ConfigEntry<bool> confEntry in configEntries)
            {
                if (!confEntry.Value)
                {
                    List<string> stringList = itemList.GetType().GetField(confEntry.Definition.Key).GetValue(itemList) as List<string>;
                    foreach (string a in stringList)
                    {
                        deniedItems.Add(a);
                    }
                }
            }

            Harmony.CreateAndPatchAll(typeof(UseEquipmentInWater));
        }

        //Used this to get all the item name strings
        //
        /*[HarmonyPatch(typeof(ObjectDB), "UpdateItemHashes")]
        [HarmonyPostfix]
        static void patchUpdateItemHashes(ObjectDB __instance)
        {
                foreach(GameObject go in __instance.m_items)
                {
                    ItemDrop goItem = go.GetComponent<ItemDrop>();
                    logger.LogInfo(goItem.m_itemData.m_shared.m_name);
                }
        }*/

        //Used for debugging
        //
        //static string lastCallerNames = "";

        [HarmonyPatch(typeof(Character), "IsSwiming")]
        [HarmonyPrefix]
        static bool patchIsSwim(ref bool __result, Humanoid __instance, float ___m_swimTimer)
        {
            if (___m_swimTimer < 0.5f)
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                string callerNames = "";
                for (int x = 2; x < st.FrameCount && x < 10; x++)
                {
                    callerNames += st.GetFrame(x).GetMethod().FullDescription() + "-";
                }

                //Used for debugging
                //
                /*if (!callerNames.Contains("OnAnimatorIK") && !callerNames.Contains("UpdateStats") && !callerNames.Contains("RandomMovement"))
                {
                    if (callerNames != lastCallerNames)
                    {
                        lastCallerNames = callerNames;
                        logger.LogInfo(callerNames);
                    }
                }*/

                if (__instance.IsPlayer() && (callerNames.Contains("UpdateEquipment") || callerNames.Contains("EquipItem")))
                {
                    if (
                       (__instance.GetRightItem() != null && deniedItems.Contains(__instance.GetRightItem().m_shared.m_name))
                    || (__instance.GetLeftItem() != null && deniedItems.Contains(__instance.GetLeftItem().m_shared.m_name))
                    )
                    {
                        if (!__instance.IsOnGround())
                        {
                            __instance.HideHandItems();
                        }
                    }
                    __result = false;
                    return false;
                }

            }
            return true;
        }


        //This empty "useless" HarmonyPatch fixed incompatiblity with other mods like KatEdition. I dont understand why this would fix anything but it does
        [HarmonyPatch(typeof(Humanoid), "EquipItem")]
        [HarmonyPrefix]
        static void patchEquipItem()
        {
        }

        //This empty HarmonyPatch is just a precaution for possibly existing incompatiblity cases like the ones with the above EquipItem HarmonyPatch
        [HarmonyPatch(typeof(Humanoid), "UpdateEquipment")]
        [HarmonyPrefix]
        static void patchUpdateEquipment()
        {
        }
    }
}

