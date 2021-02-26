using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;

namespace Use_Equipment_in_Water
{
    [BepInPlugin("com.lvh-it.valheim.useequipmentinwater", "Use Equipment in Water", "0.2.0.0")]
    [BepInProcess("valheim.exe")]
    class UseEquipmentInWater : BaseUnityPlugin
    {
        private static ManualLogSource logger;
        private List<ConfigEntry<bool>> configEntries = new List<ConfigEntry<bool>>();
        private static List<string> deniedItems = new List<string>();
        private Items itemList = new Items();

        void Main()
        {
            logger = Logger;

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

        [HarmonyPatch(typeof(Humanoid), "EquipItem")]
        [HarmonyPrefix]
        static bool patchEquipItem(Humanoid __instance, ref ItemDrop.ItemData item)
        {
            if (__instance.IsSwiming() && __instance.IsPlayer() && deniedItems.Contains(item.m_shared.m_name))
            {
                return false;
            }
            return true;
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

        [HarmonyPatch(typeof(Humanoid), "UpdateEquipment")]
        [HarmonyPrefix]
        static bool patchUpdateEquipment(Humanoid __instance)
        {
            if (__instance.IsSwiming() && __instance.IsPlayer()
                && ((__instance.GetRightItem() != null && deniedItems.Contains(__instance.GetRightItem().m_shared.m_name))
                || (__instance.GetLeftItem() != null && deniedItems.Contains(__instance.GetLeftItem().m_shared.m_name))))
            {
                __instance.HideHandItems();
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(Character), "IsSwiming")]
        [HarmonyPrefix]
        static bool patchIsSwim(Humanoid __instance, ref bool __result)
        {
            string callerName = (new System.Diagnostics.StackTrace()).GetFrame(2).GetMethod().Name;

            //Used this for testing
            //
            /*if (__instance.IsPlayer() && callerName.Contains("Equip"))
            {
                logger.LogInfo(callerName);
            }*/

            if (!callerName.Contains("patchEquipItem") && !callerName.Contains("patchUpdateEquipment") && (callerName.Contains("EquipItem") || callerName.Contains("UpdateEquipment")) && __instance.IsPlayer())
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
