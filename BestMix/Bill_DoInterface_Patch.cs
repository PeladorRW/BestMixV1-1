using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BestMix
{
    [HarmonyPatch(typeof(Bill), "DoInterface")]
    public class Bill_DoInterface_Patch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        public static void PostFix(ref Bill __instance, ref Rect __result, float x, float y, float width, int index)
        {
            BillStack BS = __instance.billStack;
            if (BS != null)
            {
                Thing billGiver = (BS.billGiver as Thing);
                if (billGiver != null)
                {
                    if (BestMixUtility.IsValidForComp(billGiver))
                    {
                        Rect newRect = __result;
                        //GUI.BeginGroup(newRect);
                        float offset = Controller.Settings.BillBMPos; //90f;
                        Texture2D BMTex = BMBillUtility.GetBillBMTex(billGiver, __instance);
                        Rect rect = newRect;
                        rect.height = 24f; rect.width = 24f;
                        rect.x = newRect.width - (24f + offset);
                        rect.y = 0f;
                        //Rect rectBM = new Rect(newRect.width - (24f + offset), 0f, 24f, 24f);
                        Color color = Color.white;
                        if (Widgets.ButtonImage(rect, BMTex, color, color * GenUI.SubtleMouseoverColor))
                        {
                            BMBillUtility.SetBillBMVal(billGiver, __instance);
                            SoundDefOf.Click.PlayOneShotOnCamera();
                        }
                        //GUI.EndGroup();
                        __result = newRect;
                    }
                }
            }
        }
    }
}
