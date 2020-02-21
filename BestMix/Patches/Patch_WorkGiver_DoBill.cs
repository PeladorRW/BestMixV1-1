using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

using HarmonyLib;

using RimWorld;
using Verse;

namespace BestMix.Patches
{
    class Patch_WorkGiver_DoBill : CustomHarmonyPatch
    {
        internal override void Patch(Harmony HMinstance)
        {
            try
            {
                var original = AccessTools.Method(typeof(WorkGiver_DoBill), "TryFindBestBillIngredients");
                var transpiler = AccessTools.Method(typeof(Patch_WorkGiver_DoBill), "Transpiler_TryFindBestBillIngredients");
                HMinstance.Patch(original, null, null, new HarmonyMethod(transpiler));
            }
            catch (Exception ex)
            {
                Log.Error($"Exception while patching WorkGiver_DoBill !\n{ex}");
            }
        }

        static IEnumerable<CodeInstruction> Transpiler_TryFindBestBillIngredients(IEnumerable<CodeInstruction> instructions)
        {
            Type workGiverType = typeof(WorkGiver_DoBill);
            FieldInfo RegionProcessorSubtitutionSingleton = AccessTools.Field(typeof(RegionProcessorSubtitution), nameof(RegionProcessorSubtitution.singleton));
            var LdvirtftnMethodBase = AccessTools.Method(typeof(RegionProcessorSubtitution), "RegionProcessor");
            var RegionProcessorType = AccessTools.TypeByName("RegionProcessor"); // hidden type
            var RegionProcessorPointerCtor = AccessTools.Constructor(RegionProcessorType, new Type[] { typeof(object), typeof(IntPtr) });
            //does nameof() can make an error? IDK
            MethodInfo FetchLocalFields = AccessTools.Method(typeof(RegionProcessorSubtitution), RegionProcessorSubtitution.FetchLocalFieldsMethodName);
            MethodInfo FetchStaticFields = AccessTools.Method(typeof(RegionProcessorSubtitution), RegionProcessorSubtitution.FetchStaticFieldsMethodName);
            MethodInfo UpdateData = AccessTools.Method(typeof(RegionProcessorSubtitution), RegionProcessorSubtitution.UpdateDataName);
            //h = hidden type
            var c__DisplayClass20_0 = AccessTools.FirstInner(workGiverType, type => type.Name.Contains("<>c__DisplayClass20_0"));
            var h_adjacentRegionsAvailable = AccessTools.Field(c__DisplayClass20_0, "adjacentRegionsAvailable");
            var h_pawn = AccessTools.Field(c__DisplayClass20_0, "pawn");
            var h_regionsProcessed = AccessTools.Field(c__DisplayClass20_0, "regionsProcessed");
            var h_rootCell = AccessTools.Field(c__DisplayClass20_0, "rootCell");
            var h_foundAll = AccessTools.Field(c__DisplayClass20_0, "foundAll");
            var h_bill = AccessTools.Field(c__DisplayClass20_0, "bill");
            var h_billGiver = AccessTools.Field(c__DisplayClass20_0, "billGiver");
            var h_chosen = AccessTools.Field(c__DisplayClass20_0, "chosen");
            //sf = static field
            var sf_chosenIngThings = AccessTools.Field(workGiverType, "chosenIngThings");
            var sf_relevantThings = AccessTools.Field(workGiverType, "relevantThings");
            var sf_processedThings = AccessTools.Field(workGiverType, "processedThings");
            var sf_newRelevantThings = AccessTools.Field(workGiverType, "newRelevantThings");
            var sf_ingredientsOrdered = AccessTools.Field(workGiverType, "ingredientsOrdered");

            List<CodeInstruction> insts = instructions.ToList();
            int instsLength = insts.Count;
            for (int i = 0; i < instsLength; i++)
            {
                var inst = insts[i];
                #region data field fetcher patch section
                bool CodeEntry = i < instsLength - 2 && i > 0 && inst.opcode == OpCodes.Ldloc_0 && insts[i + 1].opcode == OpCodes.Ldftn && insts[i - 1].opcode == OpCodes.Call;
                if (CodeEntry)
                { // entering IL_0217, ldloc.0 line 2205, before ldftn instance bool RimWorld.WorkGiver_DoBill/'<>c__DisplayClass20_0'::'<TryFindBestBillIngredients>b__3'(class Verse.Region)
                    #region local data field fetcher section
                    yield return new CodeInstruction(OpCodes.Ldsfld, RegionProcessorSubtitutionSingleton);

                    //prepare for Fetchdata method parameters
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, h_adjacentRegionsAvailable); // index 0

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, h_regionsProcessed); // index 1

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, h_rootCell); // index 2

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, h_bill); // index 3

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, h_pawn); // index 4

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, h_billGiver); // index 5

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, h_chosen); // index 6

                    yield return new CodeInstruction(OpCodes.Ldc_I4_0); // index 7

                    yield return new CodeInstruction(OpCodes.Callvirt, FetchLocalFields);
                    #endregion

                    #region static data field fetcher section
                    yield return new CodeInstruction(OpCodes.Ldsfld, RegionProcessorSubtitutionSingleton);

                    //prepare for FetchStaticFields parameters
                    yield return new CodeInstruction(OpCodes.Ldsfld, sf_chosenIngThings);
                    yield return new CodeInstruction(OpCodes.Ldsfld, sf_relevantThings);
                    yield return new CodeInstruction(OpCodes.Ldsfld, sf_processedThings);
                    yield return new CodeInstruction(OpCodes.Ldsfld, sf_newRelevantThings);
                    yield return new CodeInstruction(OpCodes.Ldsfld, sf_ingredientsOrdered);

                    yield return new CodeInstruction(OpCodes.Callvirt, FetchStaticFields);
                    #endregion

                    #region Creating new RegionProcessor
                    yield return new CodeInstruction(OpCodes.Ldsfld, RegionProcessorSubtitutionSingleton);
                    yield return new CodeInstruction(OpCodes.Dup);
                    yield return new CodeInstruction(OpCodes.Ldvirtftn, LdvirtftnMethodBase);
                    yield return new CodeInstruction(OpCodes.Newobj, RegionProcessorPointerCtor);
                    #endregion
                    i += 2;
                    continue; // next line is IL_0223, stloc.1, line 2208
                }
                #endregion

                if (inst.opcode == OpCodes.Call && inst.operand != null && inst.operand.ToString().Contains("BreadthFirstTraverse"))
                { // entering IL_01C6 call, line 1881
                    yield return inst;

                    yield return new CodeInstruction(OpCodes.Ldsfld, RegionProcessorSubtitutionSingleton);

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldflda, h_bill); // index 3

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldflda, h_pawn); // index 4

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldflda, h_billGiver); // index 5

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldflda, h_chosen); // index 6

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldflda, h_foundAll);

                    yield return new CodeInstruction(OpCodes.Callvirt, UpdateData);
                    continue;
                }

                yield return inst;
            }
        }
    }
}

//self note to aid further coding if needed.
/*
1) ldvirtftn으로 함수 포인터를 스택에 적재
2) newobj RegionProcessor::.ctor를 통해서 RegionProcessor 객체 생성 -> 스택 적재
인스턴스 객체 호출하기
스택(상)
callvirt -> 중요! call 아님, callvirt 사용해야함
parameter
parameter
...
...
parameter
ldfld / ldsfld 등으로 인스턴스 스택에 적재
스택(하)
ldvirtftn 말고 ldftn 을 사용했어야함...
*/


