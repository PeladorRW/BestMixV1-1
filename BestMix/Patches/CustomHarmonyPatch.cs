using System;
using HarmonyLib;
using System.Reflection;

namespace BestMix.Patches
{
    internal abstract class CustomHarmonyPatch
    {
        internal abstract void Patch(Harmony HMinstance);
    }
}
