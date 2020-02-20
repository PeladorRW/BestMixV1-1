using System;
using System.Collections.Generic;
using HarmonyLib;

using Verse;
using RimWorld;

using BestMix.Patches;

namespace BestMix
{
    public abstract class RegionProcessorSubtitution
    {
        #region static methods and fields
        public static RegionProcessorSubtitution singleton;
        public static readonly string FetchLocalFieldsMethodName = nameof(FetchLocalFields);
        public static readonly string FetchStaticFieldsMethodName = nameof(FetchStaticFields);
        public static readonly string UpdateDataName = nameof(UpdateData);

        public static void Initialize(RegionProcessorSubtitution instance)
        {
            if (singleton != null)
                throw new Exception("RegionProcessorSubtitution should be initialized once! you're calling initializer more than once.");

            singleton = instance;
        }

        #endregion



        protected List<ThingCount> chosenIngThings { get; set; } // 바뀜
        protected IntRange ReCheckFailedBillTicksRange { get; } // 안바뀜
        protected string MissingMaterialsTranslated { get; } // 안바뀜
        protected List<Thing> relevantThings { get; set; } // 바뀔수도
        protected HashSet<Thing> processedThings { get; set; } // 바뀔수도
        protected List<Thing> newRelevantThings { get; set; } // 바뀔수도
        protected List<IngredientCount> ingredientsOrdered { get; set; } // 바뀜
        protected List<Thing> tmpMedicine { get; set; } // 필요없음
        protected int lf_adjacentRegionsAvailable { get; set; }
        protected int lf_regionsProcessed { get; set; }
        protected IntVec3 lf_rootCell { get; set; }
        protected Bill p_bill { get; set; }
        protected Pawn p_pawn { get; set; }
        protected Thing p_billGiver { get; set; }
        protected List<ThingCount> p_chosen { get; set; }
        protected bool lf_foundAll { get; set; }


        //class option
        protected virtual bool ApplyToParameter { get; } = true;
        protected virtual bool CopyOnOverride { get; } = false;
        //protected WorkGiver_DoBill.DefCountList availableCounts { get; set; }

        protected RegionProcessorSubtitution()
        {

        }

        // called by reflection, connected by Patch_WorkGiver_DoBill
        // lf = local field, p = parameter
        private void FetchStaticFields(List<ThingCount> _chosenIngThings,
                                       List<Thing> _relevantThings,
                                       HashSet<Thing> _processedThings,
                                       List<Thing> _newRelevantThings,
                                       List<IngredientCount> _ingredientsOrdered)
        {
            //get by harmony
            this.chosenIngThings = _chosenIngThings;
            this.relevantThings = _relevantThings;
            this.processedThings = _processedThings;
            this.newRelevantThings = _newRelevantThings;
            this.ingredientsOrdered = _ingredientsOrdered;
        }

        // called by reflection, connected by Patch_WorkGiver_DoBill
        private void FetchLocalFields(int lf_adjacentRegionsAvailable,
                                      int lf_regionsProcessed,
                                      IntVec3 lf_rootCell,
                                      Bill p_bill,
                                      Pawn p_pawn,
                                      Thing p_billGiver,
                                      List<ThingCount> p_chosen,
                                      bool lf_foundAll)
        {
            //local fields  
            this.lf_adjacentRegionsAvailable = lf_adjacentRegionsAvailable;
            this.lf_regionsProcessed = lf_regionsProcessed;
            this.lf_rootCell = lf_rootCell;
            this.p_bill = p_bill;
            this.p_pawn = p_pawn;
            this.p_billGiver = p_billGiver;
            this.p_chosen = p_chosen;
            this.lf_foundAll = lf_foundAll;
        }

        protected abstract bool RegionProcessor(Region reg);

        // called by reflection, connected by Patch_WorkGiver_DoBill
        private void UpdateData(ref Bill bill,
                                ref Pawn pawn,
                                ref Thing billGiver,
                                ref List<ThingCount> chosen,
                                ref bool foundAll)
        {
            if (ApplyToParameter)
            {
                bill = this.p_bill;
                pawn = this.p_pawn;
                billGiver = this.p_billGiver;
                chosen = this.p_chosen;
                foundAll = this.lf_foundAll;
            }
        }
    }
}
