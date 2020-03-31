using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;

namespace BestMix
{
    [HarmonyPatch(typeof(Bill), "DoInterface")]
    class Bill_DoInterface_transpiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo triggerInfo = AccessTools.Method(typeof(GUI), "EndGroup");

            MethodInfo rectInfo = AccessTools.Method(typeof(Bill_DoInterface_transpiler), "newRect", new[] { (typeof(Rect)), (typeof(Bill)), (typeof(BillStack)) });

            foreach (CodeInstruction i in instructions)
            {
                if (i.opcode == OpCodes.Ldfld && i.operand == triggerInfo)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, rectInfo);
                    yield return i;
                }
                else
                    yield return i;
            }
        }

        public static Rect newRect(Rect RectBM, Rect rect, Bill bill, BillStack billStack) => Rect rectBM = BMBillUtility.MakeBillBMIcon(rect, this, billStack);
    }
}