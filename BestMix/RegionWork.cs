using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using Verse.AI;
using HarmonyLib;

namespace BestMix
{
    public class RegionWork : RegionProcessorSubtitution
    {
        protected override bool RegionProcessor(Region r)
        {
            if (BestMixUtility.BMixRegionIsInRange(r, p_billGiver, p_bill))
            {
                Predicate<Thing> BMixValidator = BestMixUtility.BestMixValidator(p_pawn, p_billGiver, p_bill);

                List<Thing> list = r.ListerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.HaulableEver));
                for (int i = 0; i < list.Count; i++)
                {
                    Thing thing = list[i];
                    if (!(BestMixUtility.BMIsForbidden(thing)))
                    {
                        if (!processedThings.Contains(thing) && ReachabilityWithinRegion.ThingFromRegionListerReachable(thing, r, PathEndMode.ClosestTouch, p_pawn)
                            && BMixValidator(thing) && (!thing.def.IsMedicine || (!(p_billGiver is Pawn))))
                        {
                            newRelevantThings.Add(thing);
                            processedThings.Add(thing);
                        }
                    }
                }
                lf_regionsProcessed++;
                if (newRelevantThings.Count > 0 && lf_regionsProcessed > lf_adjacentRegionsAvailable)
                {
                    Comparison<Thing> comparison = BestMixUtility.GetBMixComparer(p_billGiver, lf_rootCell);
                    //newRelevantThings.Sort(comparison);
                    relevantThings.AddRange(newRelevantThings);
                    relevantThings.Sort(comparison);
                    BestMixUtility.BMixDebugList(relevantThings, p_billGiver, lf_rootCell);
                    newRelevantThings.Clear();

                    if (BestMixUtility.TryFindBestMixInSet(relevantThings, p_bill, p_chosen, ingredientsOrdered))
                    {
                        lf_foundAll = true;
                        bool finishNow = BestMixUtility.BMixFinishedStatus(lf_foundAll, p_billGiver);
                        if ((lf_foundAll) && (finishNow))
                        {
                            BestMixUtility.DebugChosenList(p_billGiver, p_chosen);
                            BestMixUtility.DebugFoundAll(p_billGiver, lf_foundAll);
                            return true;
                        }
                    }
                }
            }
            BestMixUtility.DebugChosenList(p_billGiver, p_chosen);
            BestMixUtility.DebugFoundAll(p_billGiver, lf_foundAll);
            return false;
        }
    }
}

