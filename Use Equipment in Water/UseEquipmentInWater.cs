using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Use_Equipment_in_Water
{
    [BepInPlugin("com.lvh-it.valheim.useequipmentinwater", "Use Equipment in Water", "0.1.0.0")]
    [BepInProcess("valheim.exe")]
    class UseEquipmentInWater : BaseUnityPlugin
    {
        private static ManualLogSource logger;

        void Main()
        {
            logger = Logger;
            Harmony.CreateAndPatchAll(typeof(UseEquipmentInWater));
        }

        

        [HarmonyPatch(typeof(Character), "IsSwiming")]
        [HarmonyPrefix]
        static bool patchIsSwim(Humanoid __instance, ref bool __result)
        {
            string callerName = (new System.Diagnostics.StackTrace()).GetFrame(2).GetMethod().Name;
            
            if ((callerName.Contains("EquipItem") || callerName.Contains("UpdateEquipment")) &&  __instance.IsPlayer())
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
