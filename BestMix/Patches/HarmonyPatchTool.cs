using System;
using System.Collections.Generic;
using HarmonyLib;
using System.Reflection;
using System.Linq;

namespace BestMix.Patches
{

    public static class HarmonyPatchTool
    {
        static bool initialized = false;
        public static void PatchAll(Harmony HMinstance)
        {
            EnsurePatchingOnlyOnce();

            try
            {
                var CustomPatchTypes = from type in Assembly.GetAssembly(typeof(HarmonyPatchTool)).GetTypes()
                                       where type.IsSubclassOf(typeof(CustomHarmonyPatch))
                                       select type;


                foreach (var type in CustomPatchTypes)
                {
                    var instance = Activator.CreateInstance(type) as CustomHarmonyPatch;
                    instance.Patch(HMinstance);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                initialized = true;
            }
        }

        static void EnsurePatchingOnlyOnce()
        {
            if (initialized)
                throw new Exception("trying to invoke PatchAll method twice!");
        }
    }
}
