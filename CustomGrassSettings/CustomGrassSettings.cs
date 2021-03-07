using BepInEx;
using System.Threading;
using System.Reflection;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;

namespace CustomGrassSettings
{
    [BepInPlugin("com.lvh-it.valheim.customgrasssettings", "Custom Grass Settings", "0.1.0.0")]
    [BepInProcess("valheim.exe")]
    [BepInProcess("valheim.x86_64")]
    class CustomGrassSettings : BaseUnityPlugin
    {
        private static ManualLogSource logger;
        void Main()
        {
            logger = Logger;
            Harmony.CreateAndPatchAll(typeof(CustomGrassSettings));
        }

        [HarmonyPatch(typeof(ClutterSystem), "GeneratePatches")]
        [HarmonyPrefix]
        static bool patchGeneratePatches(bool rebuildAll, UnityEngine.Vector3 center, ref float ___m_amountScale, ref float ___m_distance, ref float ___m_grassPatchSize, ClutterSystem __instance)
        {
            if (ClutterSystem.instance == null)
            {
                return true;
            }
            ClutterSystem CS = ClutterSystem.instance;
            ___m_distance = 40f;
            ___m_grassPatchSize = 8f;
            ___m_amountScale = 1f;
            bool flag = false;
            UnityEngine.Vector2Int vegPatch = __instance.GetVegPatch(center);

            patchGeneratePatch(ClutterSystem.instance, center, vegPatch, ref flag, rebuildAll);

            int num = UnityEngine.Mathf.CeilToInt((___m_distance - ___m_grassPatchSize / 2f) / ___m_grassPatchSize);
            for (int i = 1; i <= num; i++)
            {
                Thread taOne = new Thread(new ThreadStart(() =>
                {
                    List<Thread> threads = new List<Thread>();
                    for (int j = vegPatch.x - i; j <= vegPatch.x + i; j++)
                    {
                        try
                        {
                            Thread taOnee = new Thread(new ThreadStart(() =>
                            {
                                patchGeneratePatch(CS, center, new UnityEngine.Vector2Int(j, vegPatch.y - i), ref flag, rebuildAll);
                            }));
                            threads.Add(taOnee);
                            taOnee.Start();
                        }
                        catch (System.Exception e)
                        {
                        }
                    }
                    foreach (Thread t in threads)
                    {
                        t.Join();
                    }
                }));
                Thread taTwo = new Thread(new ThreadStart(() =>
                {
                    List<Thread> threads = new List<Thread>();
                    for (int k = vegPatch.y - i + 1; k <= vegPatch.y + i - 1; k++)
                    {
                        try
                        {
                            Thread taTwoo = new Thread(new ThreadStart(() =>
                            {
                                patchGeneratePatch(CS, center, new UnityEngine.Vector2Int(vegPatch.x - i, k), ref flag, rebuildAll);
                            }));
                            threads.Add(taTwoo);
                            taTwoo.Start();
                        }
                        catch (System.Exception e)
                        {
                        }
                    }
                    foreach (Thread t in threads)
                    {
                        t.Join();
                    }
                }));
                Thread taThree = new Thread(new ThreadStart(() =>
                {
                    List<Thread> threads = new List<Thread>();
                    for (int j = vegPatch.x - i; j <= vegPatch.x + i; j++)
                    {
                        try
                        {
                            Thread taThreee = new Thread(new ThreadStart(() =>
                            {
                                patchGeneratePatch(CS, center, new UnityEngine.Vector2Int(j, vegPatch.y + i), ref flag, rebuildAll);
                            }));
                            threads.Add(taThreee);
                            taThreee.Start();
                        }
                        catch (System.Exception e)
                        {
                        }
                    }
                    foreach (Thread t in threads)
                    {
                        t.Join();
                    }
                }));
                Thread taFour = new Thread(new ThreadStart(() =>
                {
                    List<Thread> threads = new List<Thread>();
                    for (int k = vegPatch.y - i + 1; k <= vegPatch.y + i - 1; k++)
                    {
                        try
                        {
                            Thread taFourr = new Thread(new ThreadStart(() =>
                            {
                                patchGeneratePatch(CS, center, new UnityEngine.Vector2Int(vegPatch.x + i, k), ref flag, rebuildAll);
                            }));
                            threads.Add(taFourr);
                            taFourr.Start();
                        }
                        catch (System.Exception e)
                        {
                        }
                    }
                    foreach (Thread t in threads)
                    {
                        t.Join();
                    }
                }));


                taOne.Start();
                taTwo.Start();
                taThree.Start();
                taFour.Start();

                taOne.Join();
                taTwo.Join();
                taThree.Join();
                taFour.Join();
            }
            return false;
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(ClutterSystem), "GeneratePatch")]
        static void patchGeneratePatch(object instance, UnityEngine.Vector3 camPos, UnityEngine.Vector2Int p, ref bool generated, bool rebuildAll)
        {
        }

    }
}
