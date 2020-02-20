using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BestMix
{
    [StaticConstructorOnStartup]
    internal static class BestMix_Initializer
    {
        static BestMix_Initializer()
        {
            LongEventHandler.QueueLongEvent(new Action(BestMix_Initializer.Setup), "LibraryStartup", false, null);
        }

        public static void Setup()
        {
            List<ThingDef> thingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            if (thingDefs.Count > 0)
            {
                int num = 0;
                foreach (ThingDef thingDef in thingDefs)
                {
                    if ((IsBuildingClass(thingDef)) && (IsBillItem(thingDef)))
                    {
                        if (TryAddBestMixComp(thingDef))
                        {
                            num++;
                        }
                    }
                }
                if (num > 0)
                {
                    string Msg = "BestMix.SetupCount".Translate(num.ToString());
                    Log.Message(Msg);
                }
            }

            //dependency injection for custom region processor
            RegionProcessorSubtitution.Initialize(new RegionWork());
        }

        public static bool TryAddBestMixComp(ThingDef def)
        {
            CompProperties_BestMix compProp_BestMix = def.comps.OfType<CompProperties_BestMix>().FirstOrDefault<CompProperties_BestMix>();
            if (compProp_BestMix == null)
            {
                compProp_BestMix = new CompProperties_BestMix();
                def.comps.Add(compProp_BestMix);
                return true;
            }
            return false;
        }

        public static bool IsBuildingClass(ThingDef def)
        {
            bool bClass = false;
            if (def?.thingClass != null)
            {
                Type chkClass = def.thingClass;
                if (chkClass.IsClass)
                {
                    if (chkClass.IsSubclassOf(typeof(Building)))
                    {
                        bClass = true;
                    }
                }
            }
            return bClass;
        }

        public static bool IsBillItem(ThingDef def)
        {
            bool billItem = false;
            if (def?.inspectorTabs != null)
            {
                List<Type> inspects = def.inspectorTabs;
                if (inspects.Count > 0)
                {
                    foreach (Type inspect in inspects)
                    {
                        if (inspect == typeof(ITab_Bills))
                        {
                            billItem = true;
                        }
                    }
                }
            }
            return billItem;
        }
    }
}

