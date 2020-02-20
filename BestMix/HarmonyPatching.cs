using System.Reflection;
using Verse;
using HarmonyLib;
using BestMix.Patches;

namespace BestMix
{
    [StaticConstructorOnStartup]
    static class HarmonyPatching
    {
        static HarmonyPatching()
        {
            var harmony = new Harmony("com.Pelador.Rimworld.BestMix");
#if DEBUG
            Harmony.DEBUG = true;
#endif
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            HarmonyPatchTool.PatchAll(harmony);
        }
    }
}
