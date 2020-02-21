using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace BestMix
{
    [HarmonyPatch(typeof(BillStack), "DoListing")]
    public static class Patch_BillStack_DoListing
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!(Controller.Settings.AllowBMBillMaxSet))
            {
                yield break;
            }
            int newMax = 125;
            AccessTools.Property(typeof(BillStack), "Count").GetGetMethod();
            List<CodeInstruction> instructionList = instructions.ToList<CodeInstruction>();
            bool found = false;
            int num;
            for (int i = 0; i < instructionList.Count; i = num)
            {
                CodeInstruction codeInstruction = instructionList[i];
                if (instructionList[i].opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(instructionList[i].operand) == 15)
                {
                    found = true;
                    instructionList[i].operand = newMax;
                }
                yield return codeInstruction;
                num = i + 1;
            }
            if (!found)
            {
                Log.Error("BestMix.MaxBillFail".Translate()); // BestMix could not change the number of max bills for benches, this will be due to a mod conflict.
            }
            else
            {
                Log.Message("BestMix.MaxBillDone".Translate(newMax.ToString()));  // BestMix changed the maximum number of bills for billgivers to {newMax}.
            }
            yield break;
        }
    }
}