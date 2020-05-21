using HarmonyLib;
using Multiplayer.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Verse;

namespace BestMix
{
    [StaticConstructorOnStartup]
    static class MultiplayerSupport
    {
        static Harmony harmony = new Harmony("rimworld.pelador.bestmix.multiplayersupport");

        static MultiplayerSupport()
        {
            if (!MP.enabled) return;

            //SyncMethods
            MP.RegisterSyncMethod(typeof(BMBillUtility), nameof(BMBillUtility.SetBMixBillMode));
            MP.RegisterSyncMethod(typeof(CompBestMix), nameof(CompBestMix.SetBMixMode));

            // Add all Methods where there is Rand calls here
            var methods = new[] {
            AccessTools.Method(typeof(BestMixUtility), nameof(BestMixUtility.RNDFloat))
        };
            foreach (var method in methods)
            {
                FixRNG(method);
            }
        }

        static void FixRNG(MethodInfo method)
        {
            harmony.Patch(method,
                prefix: new HarmonyMethod(typeof(MultiplayerSupport), nameof(FixRNGPre)),
                postfix: new HarmonyMethod(typeof(MultiplayerSupport), nameof(FixRNGPos))
            );
        }

        static void FixRNGPre() => Rand.PushState(Find.TickManager.TicksAbs);
        static void FixRNGPos() => Rand.PopState();
    }
}
