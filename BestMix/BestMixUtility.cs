using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.AI;

namespace BestMix
{
    public class BestMixUtility
    {
        public static string ProtElectricStat = "StuffPower_Armor_Electric";

        public static bool BMixRegionIsInRange(Region r, Thing billGiver, Bill bill)
        {
            if (!(Controller.Settings.IncludeRegionLimiter))
            {
                return true;
            }

            if (!(IsValidForComp(billGiver)))
            {
                return true;
            }

            CompBestMix compBMix = billGiver.TryGetComp<CompBestMix>();
            if (compBMix != null)
            {
                //if (compBMix.CurMode == "DIS")
                if (BMBillUtility.UseBMixMode(compBMix, billGiver, bill) == "DIS")
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            /*
            List<IntVec3> cells = r?.Cells.ToList<IntVec3>();
            if ((cells != null) && (cells.Count > 0))
            {
                foreach (IntVec3 cell in cells)
                {
                    if (((float)((cell - billGiver.Position).LengthHorizontalSquared)) < ((float)(bill.ingredientSearchRadius * bill.ingredientSearchRadius)))
                    {
                        return true;
                    }
                }
            }
            */

            // optimised to region corners
            Map map = billGiver?.Map;
            if (map != null)
            {
                RegionGrid regions = map?.regionGrid; // *
                if (regions != null)
                {
                    IntVec3 chkcell = IntVec3.Zero;
                    for (int i = 0; i < 4; i++)
                    {
                        switch (i)
                        {
                            case 0: chkcell = new IntVec3(r.extentsClose.minX, 0, r.extentsClose.minZ); break;
                            case 3: chkcell = new IntVec3(r.extentsClose.minX, 0, r.extentsClose.maxZ); break;
                            case 2: chkcell = new IntVec3(r.extentsClose.maxX, 0, r.extentsClose.minZ); break;
                            case 1: chkcell = new IntVec3(r.extentsClose.maxX, 0, r.extentsClose.maxZ); break;
                        }
                        //if (chkcell.GetRegion(map) == r)
                        if (regions.GetRegionAt_NoRebuild_InvalidAllowed(chkcell) == r) // * More direct check
                        {
                            float scaleToCell = ((float)(chkcell - billGiver.Position).LengthHorizontalSquared);
                            float scaleSearchRadius = ((float)(bill.ingredientSearchRadius * bill.ingredientSearchRadius));
                            if (Controller.Settings.UseRadiusLimit)
                            {
                                float RadiusLimit = ((float)(Controller.Settings.RadiusLimit));
                                float scaleLimit = (RadiusLimit * RadiusLimit);
                                if (scaleLimit < scaleSearchRadius)
                                {
                                    scaleSearchRadius = scaleLimit;
                                }
                            }
                            if (scaleToCell <= scaleSearchRadius)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static bool BMixFinishedStatus(bool foundAll, Thing billGiver, Bill bill)
        {
            if (foundAll)
            {
                if (billGiver is Pawn p)
                {
                    return true;
                }
                if (IsValidForComp(billGiver))
                {
                    //if (billGiver.TryGetComp<CompBestMix>().CurMode == "DIS")
                    CompBestMix CBM = billGiver.TryGetComp<CompBestMix>();
                    if (CBM != null)
                    {
                        if (BMBillUtility.UseBMixMode(CBM, billGiver, bill) == "DIS")
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsValidForComp(Thing thing)
        {
            if ((Controller.Settings.AllowBestMix) && (thing is Building bChk) && ((thing as Building).def.inspectorTabs.Contains(typeof(ITab_Bills))))
            {
                if ((!(Controller.Settings.AllowMealMakersOnly)) || ((Controller.Settings.AllowMealMakersOnly) && ((thing as Building).def.building.isMealSource)))
                {
                    return true;
                }
            }
            return false;
        }

        public static float RNDFloat()
        {
            return Rand.Range(1f, 9999f);
        }

        public static bool IsCEActive()
        {
            string CEModName = "Combat Extended";
            if (ModLister.HasActiveModWithName(CEModName))
            {
                return true;
            }
            return false;
        }

        public static List<string> BMixModes()
        {
            List<string> list = new List<string>();
            list.AddDistinct("DIS");
            list.AddDistinct("DTR");
            list.AddDistinct("HPT");
            list.AddDistinct("RHP");
            list.AddDistinct("VLC");
            list.AddDistinct("VLE");
            list.AddDistinct("TMP");
            list.AddDistinct("FRZ");
            list.AddDistinct("RND");
            list.AddDistinct("BIT");
            if (Controller.Settings.useStock)
            {
                list.AddDistinct("MST");
                list.AddDistinct("LST");
            }
            list.AddDistinct("BTY");
            list.AddDistinct("UGY");
            list.AddDistinct("HVY");
            list.AddDistinct("LGT");
            list.AddDistinct("FLM");
            list.AddDistinct("PTB");
            list.AddDistinct("PTS");
            list.AddDistinct("PTH");
            if (IsCEActive())
            {
                list.AddDistinct("PTE");
            }
            list.AddDistinct("INH");
            list.AddDistinct("INC");
            list.AddDistinct("WSP");
            list.AddDistinct("WBT");
            return list;
        }

        public static string GetBMixIconPath(string BMixMode)
        {
            string BMixIconPath = "UI/BestMix/";

            switch (BMixMode)
            {
                case "DIS": BMixIconPath += "Nearest"; break;
                case "DTR": BMixIconPath += "Expiry"; break;
                case "HPT": BMixIconPath += "Damaged"; break;
                case "RHP": BMixIconPath += "Robust"; break;
                case "VLC": BMixIconPath += "Cheapest"; break;
                case "VLE": BMixIconPath += "Value"; break;
                case "TMP": BMixIconPath += "Warmest"; break;
                case "FRZ": BMixIconPath += "Coldest"; break;
                case "RND": BMixIconPath += "Random"; break;
                case "BIT": BMixIconPath += "Fraction"; break;
                case "MST": BMixIconPath += "StockMost"; break;
                case "LST": BMixIconPath += "StockLeast"; break;
                case "BTY": BMixIconPath += "Beauty"; break;
                case "UGY": BMixIconPath += "Duckling"; break;
                case "HVY": BMixIconPath += "Heaviest"; break;
                case "LGT": BMixIconPath += "Lightest"; break;
                case "FLM": BMixIconPath += "Ignition"; break;
                case "PTB": BMixIconPath += "ProtectBlunt"; break;
                case "PTS": BMixIconPath += "ProtectSharp"; break;
                case "PTH": BMixIconPath += "ProtectHeat"; break;
                case "PTE": BMixIconPath += "ProtectElectric"; break;
                case "INH": BMixIconPath += "InsulateHeat"; break;
                case "INC": BMixIconPath += "InsulateCold"; break;
                case "WSP": BMixIconPath += "Sharpest"; break;
                case "WBT": BMixIconPath += "Bluntest"; break;
                default: BMixIconPath += "Nearest"; break;
            }

            return BMixIconPath;
        }

        public static string GetBMixModeDisplay(string BMixMode)
        {
            string ModeDisplay;
            switch (BMixMode)
            {
                case "DIS": ModeDisplay = "BestMix.ModeDistanceDIS".Translate(); break;
                case "DTR": ModeDisplay = "BestMix.ModeDaysToRotDTR".Translate(); break;
                case "HPT": ModeDisplay = "BestMix.ModeHealthHPT".Translate(); break;
                case "RHP": ModeDisplay = "BestMix.ModeHealthRHP".Translate(); break;
                case "VLC": ModeDisplay = "BestMix.ModeValueVLC".Translate(); break;
                case "VLE": ModeDisplay = "BestMix.ModeValueVLE".Translate(); break;
                case "RND": ModeDisplay = "BestMix.ModeRandomRND".Translate(); break;
                case "TMP": ModeDisplay = "BestMix.ModeTemperatureTMP".Translate(); break;
                case "FRZ": ModeDisplay = "BestMix.ModeTemperatureFRZ".Translate(); break;
                case "BIT": ModeDisplay = "BestMix.ModeFractionBIT".Translate(); break;
                case "MST": ModeDisplay = "BestMix.ModeStockMST".Translate(); break;
                case "LST": ModeDisplay = "BestMix.ModeStockLST".Translate(); break;
                case "BTY": ModeDisplay = "BestMix.ModeBeautyBTY".Translate(); break;
                case "UGY": ModeDisplay = "BestMix.ModeBeautyUGY".Translate(); break;
                case "HVY": ModeDisplay = "BestMix.ModeMassHVY".Translate(); break;
                case "LGT": ModeDisplay = "BestMix.ModeMassLGT".Translate(); break;
                case "FLM": ModeDisplay = "BestMix.ModeFlammableFLM".Translate(); break;
                case "PTB": ModeDisplay = "BestMix.ModeProtectPTB".Translate(); break;
                case "PTS": ModeDisplay = "BestMix.ModeProtectPTS".Translate(); break;
                case "PTH": ModeDisplay = "BestMix.ModeProtectPTH".Translate(); break;
                case "PTE": ModeDisplay = "BestMix.ModeProtectPTE".Translate(); break;
                case "INH": ModeDisplay = "BestMix.ModeInsulateINH".Translate(); break;
                case "INC": ModeDisplay = "BestMix.ModeInsulateINC".Translate(); break;
                case "WSP": ModeDisplay = "BestMix.ModeWeaponWSP".Translate(); break;
                case "WBT": ModeDisplay = "BestMix.ModeWeaponWBT".Translate(); break;
                default: ModeDisplay = "BestMix.ModeDistanceDIS".Translate(); break;
            }
            return ModeDisplay;
        }

        public static Comparison<Thing> GetBMixComparer(Thing billGiver, IntVec3 rootCell, Bill bill, out bool rnd)
        {
            rnd = false;
            string BMixMode = "DIS";
            bool BMixDebugBench = false;

            if (IsValidForComp(billGiver))
            {
                CompBestMix compBM = billGiver.TryGetComp<CompBestMix>();
                if (compBM != null)
                {
                    //BMixMode = compBM.CurMode;
                    BMixMode = BMBillUtility.UseBMixMode(compBM, billGiver, bill);
                    BMixDebugBench = compBM.BMixDebug;
                }
            }

            Comparison<Thing> comparison = null;
            switch (BMixMode)
            {
                case "DIS":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (t1.Position - rootCell).LengthHorizontalSquared;
                        float value = (t2.Position - rootCell).LengthHorizontalSquared;
                        return (num.CompareTo(value));
                    };
                    break;
                case "DTR":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float maxdtr = 72000000;
                        float t1dtr = maxdtr;
                        CompRottable t1comp = t1.TryGetComp<CompRottable>();
                        if (t1comp != null)
                        {
                            t1dtr = t1comp.TicksUntilRotAtCurrentTemp;
                        }
                        float t2dtr = maxdtr;
                        CompRottable t2comp = t2.TryGetComp<CompRottable>();
                        if (t2comp != null)
                        {
                            t2dtr = t2comp.TicksUntilRotAtCurrentTemp;
                        }
                        float num = (t1dtr);
                        float value = (t2dtr);
                        return (num.CompareTo(value));
                    };
                    break;
                case "HPT":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = 0f;
                        if (t2.def.useHitPoints)
                        {
                            num = (((float)(t2.MaxHitPoints - t2.HitPoints)) / ((float)(Math.Max(1, t2.MaxHitPoints))));
                        }
                        float value = 0f;
                        if (t1.def.useHitPoints)
                        {
                            value = (((float)(t1.MaxHitPoints - t1.HitPoints)) / ((float)(Math.Max(1, t1.MaxHitPoints))));
                        }
                        return (num.CompareTo(value));
                    };
                    break;
                case "RHP":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = 0f;
                        if (t2.def.useHitPoints)
                        {
                            num = (((float)(t2.HitPoints)) / ((float)(Math.Max(1, t2.MaxHitPoints))));
                        }
                        float value = 0f;
                        if (t1.def.useHitPoints)
                        {
                            value = (((float)(t1.HitPoints)) / ((float)(Math.Max(1, t1.MaxHitPoints))));
                        }
                        return (num.CompareTo(value));
                    };
                    break;
                case "VLC":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (0f - t2.MarketValue);
                        float value = (0f - t1.MarketValue);
                        return (num.CompareTo(value));
                    };
                    break;
                case "VLE":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.MarketValue;
                        float value = t1.MarketValue;
                        return (num.CompareTo(value));
                    };
                    break;
                case "TMP":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.AmbientTemperature;
                        float value = t1.AmbientTemperature;
                        return (num.CompareTo(value));
                    };
                    break;
                case "FRZ":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (0f - t2.AmbientTemperature);
                        float value = (0f - t1.AmbientTemperature);
                        return (num.CompareTo(value));
                    };
                    break;
                case "BIT":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (((float)(t2.def.stackLimit)) / ((float)(Math.Max(1, t2.stackCount))));
                        float value = (((float)(t1.def.stackLimit)) / ((float)(Math.Max(1, t1.stackCount))));
                        return (num.CompareTo(value));
                    };
                    break;
                case "MST":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = GetStockAmount(t2, billGiver, rootCell, bill);
                        float value = GetStockAmount(t1, billGiver, rootCell, bill);
                        return (num.CompareTo(value));
                    };
                    break;
                case "LST":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = GetStockAmount(t1, billGiver, rootCell, bill);
                        float value = GetStockAmount(t2, billGiver, rootCell, bill);
                        return (num.CompareTo(value));
                    };
                    break;
                case "RND":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = RNDFloat();
                        float value = RNDFloat();
                        return (num.CompareTo(value));
                    };
                    rnd = true;
                    break;
                case "BTY":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (t2.GetStatValue(StatDefOf.Beauty));
                        if ((t2.def?.stuffProps != null) && (t2.def?.stuffProps?.statOffsets != null))
                        {
                            if (t2.def.stuffProps.statOffsets.StatListContains(StatDefOf.Beauty))
                            {
                                num += t2.def.stuffProps.statOffsets.GetStatOffsetFromList(StatDefOf.Beauty);
                            }
                        }
                        float value = (t1.GetStatValue(StatDefOf.Beauty));
                        if ((t1.def?.stuffProps != null) && (t1.def?.stuffProps?.statOffsets != null))
                        {
                            if (t1.def.stuffProps.statOffsets.StatListContains(StatDefOf.Beauty))
                            {
                                value += t1.def.stuffProps.statOffsets.GetStatOffsetFromList(StatDefOf.Beauty);
                            }
                        }
                        return (num.CompareTo(value));
                    };
                    break;
                case "UGY":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (0f - (t2.GetStatValue(StatDefOf.Beauty)));
                        if ((t2.def?.stuffProps != null) && (t2.def?.stuffProps?.statOffsets != null))
                        {
                            if (t2.def.stuffProps.statOffsets.StatListContains(StatDefOf.Beauty))
                            {
                                num -= t2.def.stuffProps.statOffsets.GetStatOffsetFromList(StatDefOf.Beauty);
                            }
                        }
                        float value = (0f - (t1.GetStatValue(StatDefOf.Beauty)));
                        if ((t1.def?.stuffProps != null) && (t1.def?.stuffProps?.statOffsets != null))
                        {
                            if (t1.def.stuffProps.statOffsets.StatListContains(StatDefOf.Beauty))
                            {
                                value -= t1.def.stuffProps.statOffsets.GetStatOffsetFromList(StatDefOf.Beauty);
                            }
                        }
                        return (num.CompareTo(value));
                    };
                    break;
                case "HVY":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.Mass);
                        float value = t1.GetStatValue(StatDefOf.Mass);
                        return (num.CompareTo(value));
                    };
                    break;
                case "LGT":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (0f - t2.GetStatValue(StatDefOf.Mass));
                        float value = (0f - t1.GetStatValue(StatDefOf.Mass));
                        return (num.CompareTo(value));
                    };
                    break;
                case "FLM":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.Flammability);
                        float value = t1.GetStatValue(StatDefOf.Flammability);
                        return (num.CompareTo(value));
                    };
                    break;
                case "PTB":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.StuffPower_Armor_Blunt);
                        float value = t1.GetStatValue(StatDefOf.StuffPower_Armor_Blunt);
                        return (num.CompareTo(value));
                    };
                    break;
                case "PTS":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.StuffPower_Armor_Sharp);
                        float value = t1.GetStatValue(StatDefOf.StuffPower_Armor_Sharp);
                        return (num.CompareTo(value));
                    };
                    break;
                case "PTH":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.StuffPower_Armor_Heat);
                        float value = t1.GetStatValue(StatDefOf.StuffPower_Armor_Heat);
                        return (num.CompareTo(value));
                    };
                    break;
                case "PTE":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = 0f;
                        float value = 0f;
                        StatDef protElectric = DefDatabase<StatDef>.GetNamed(ProtElectricStat, false);
                        if (protElectric != null)
                        {
                            num = t2.GetStatValue(protElectric);
                            value = t1.GetStatValue(protElectric);
                        }
                        return (num.CompareTo(value));
                    };
                    break;
                case "INH":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.StuffPower_Insulation_Heat);
                        float value = t1.GetStatValue(StatDefOf.StuffPower_Insulation_Heat);
                        return (num.CompareTo(value));
                    };
                    break;
                case "INC":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.StuffPower_Insulation_Cold);
                        float value = t1.GetStatValue(StatDefOf.StuffPower_Insulation_Cold);
                        return (num.CompareTo(value));
                    };
                    break;
                case "WSP":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.SharpDamageMultiplier);
                        float value = t1.GetStatValue(StatDefOf.SharpDamageMultiplier);
                        return (num.CompareTo(value));
                    };
                    break;
                case "WBT":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.BluntDamageMultiplier);
                        float value = t1.GetStatValue(StatDefOf.BluntDamageMultiplier);
                        return (num.CompareTo(value));
                    };
                    break;
                default:
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (t1.Position - rootCell).LengthHorizontalSquared;
                        float value = (t2.Position - rootCell).LengthHorizontalSquared;
                        return (num.CompareTo(value));
                    };
                    break;
            }

            return comparison;
        }

        public static float GetStockAmount(Thing t, Thing billGiver, IntVec3 root, Bill bill)
        {
            float num = 0f;
        
            List<Thing> things = t.Map.listerThings.ThingsOfDef(t.def);
            if (things.Count > 0)
            {
                foreach (Thing thing in things)
                {
                    if (!(BMStockInRadius(thing, billGiver, bill)))
                    {
                        continue;
                    }
                    if (Controller.Settings.inStorage)
                    {
                        if (!(thing.IsInValidStorage()))
                        {
                            continue;
                        }
                    }
                    if (BMIsForbidden(thing))
                    {
                        continue;
                    }
                    if ((thing.IsBurning()) || (thing.Fogged()))
                    {
                        continue;
                    }

                    num += thing.stackCount;
                }
            }
            
            return num;
        }


        // Debug
        internal static void BMixDebugList(List<Thing> list, Thing billGiver, IntVec3 rootCell, Bill bill)
        {
            if ((Prefs.DevMode) && (Controller.Settings.DebugMaster) && (Controller.Settings.DebugSort))
            {
                bool ignore = (Controller.Settings.DebugIgnore);
                if (IsValidForComp(billGiver))
                {
                    CompBestMix compBMix = billGiver.TryGetComp<CompBestMix>();
                    if (compBMix != null)
                    {
                        if (compBMix.BMixDebug)
                        {
                            if (list.Count > 0)
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    Thing thing = list[i];
                                    string debugMsg = MakeDebugString(i, thing, billGiver, rootCell, bill, BMBillUtility.UseBMixMode(compBMix, billGiver, bill));
                                    Log.Message(debugMsg, ignore);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static void DebugChosenList(Thing billGiver, List<ThingCount> p_chosen)
        {
            if ((Prefs.DevMode) && (Controller.Settings.DebugMaster) && (Controller.Settings.DebugChosen))
            {
                bool ignore = (Controller.Settings.DebugIgnore);
                if (IsValidForComp(billGiver))
                {
                    CompBestMix compBestMix = billGiver.TryGetComp<CompBestMix>();
                    if ((compBestMix != null) && (compBestMix.BMixDebug))
                    {
                        if (p_chosen != null)
                        {
                            if (p_chosen.Count > 0)
                            {
                                Log.Message((billGiver.Label + ", " + "BestMix.Chosen".Translate() + ":"), ignore);
                                foreach (ThingCount TC in p_chosen)
                                {
                                    Thing thing = TC.Thing;
                                    int count = TC.Count;
                                    Log.Message((thing.Label + " , " + count.ToString()), ignore);
                                }
                            }
                            else
                            {
                                Log.Message((billGiver.Label + ", " + "BestMix.NoChosen".Translate() + ":"), ignore);
                            }
                        }
                        else
                        {
                            Log.Message((billGiver.Label + ", " + "BestMix.NullChosen".Translate() + ":"), ignore);
                        }
                    }
                }
            }
        }

        internal static void DebugFoundAll(Thing billGiver, bool foundAll)
        {
            if ((Prefs.DevMode) && (Controller.Settings.DebugMaster) && (Controller.Settings.DebugFound))
            {
                bool ignore = (Controller.Settings.DebugIgnore);
                if (IsValidForComp(billGiver))
                {
                    CompBestMix compBMix = billGiver.TryGetComp<CompBestMix>();
                    if (compBMix != null)
                    {
                        if (compBMix.BMixDebug)
                        {
                            string debugMsg = billGiver.Label;
                            if (foundAll)
                            {
                                debugMsg += ", " + "BestMix.FoundAll".Translate();
                            }
                            else
                            {
                                debugMsg += ", " + "BestMix.NotFoundAll".Translate();
                            }
                            Log.Message(debugMsg, ignore);
                        }
                    }
                }
            }
        }

        public static string MakeDebugString(int indx, Thing thing, Thing billGiver, IntVec3 rootCell, Bill bill, string BMixMode)
        {
            float stat = 0f;

            switch (BMixMode)
            {
                case "DIS": stat = (thing.Position - rootCell).LengthHorizontalSquared; break;
                case "DTR":
                    float maxdtr = 72000000;
                    float thingdtr = maxdtr;
                    CompRottable thingcomp = thing.TryGetComp<CompRottable>();
                    if (thingcomp != null)
                    {
                        thingdtr = thingcomp.TicksUntilRotAtCurrentTemp;
                    }
                    stat = (maxdtr - thingdtr);
                    break;
                case "HPT":
                    stat = 0f;
                    if (thing.def.useHitPoints)
                    {
                        stat = (((float)(thing.MaxHitPoints - thing.HitPoints)) / ((float)(Math.Max(1, thing.MaxHitPoints))));
                    }
                    break;
                case "RHP":
                    stat = 0f;
                    if (thing.def.useHitPoints)
                    {
                        stat = (((float)(thing.HitPoints)) / ((float)(Math.Max(1, thing.MaxHitPoints))));
                    }
                    break;
                case "VLC": stat = (0f - thing.MarketValue); break;
                case "VLE": stat = thing.MarketValue; break;
                case "TMP": stat = thing.AmbientTemperature; break;
                case "FRZ": stat = (0f - thing.AmbientTemperature); break;
                case "BIT": stat = ((float)thing.def.stackLimit / (float)(Math.Max(1, thing.stackCount))); break;
                case "MST": stat = GetStockAmount(thing, billGiver, rootCell, bill); break;
                case "LST": stat = (0f - GetStockAmount(thing, billGiver, rootCell, bill)); break;
                case "RND": stat = RNDFloat(); break;
                case "BTY": stat = thing.GetStatValue(StatDefOf.Beauty);
                    if ((thing.def?.stuffProps != null) && (thing.def?.stuffProps?.statOffsets != null))
                    {
                        if (thing.def.stuffProps.statOffsets.StatListContains(StatDefOf.Beauty))
                        {
                            stat += thing.def.stuffProps.statOffsets.GetStatOffsetFromList(StatDefOf.Beauty);
                        }
                    }
                    break;
                case "UGY": stat = (0f - (thing.GetStatValue(StatDefOf.Beauty)));
                    if ((thing.def?.stuffProps != null) && (thing.def?.stuffProps?.statOffsets != null))
                    {
                        if (thing.def.stuffProps.statOffsets.StatListContains(StatDefOf.Beauty))
                        {
                            stat -= thing.def.stuffProps.statOffsets.GetStatOffsetFromList(StatDefOf.Beauty);
                        }
                    };
                    break;
                case "HVY": stat = thing.GetStatValue(StatDefOf.Mass); break;
                case "LGT": stat = (0f - thing.GetStatValue(StatDefOf.Mass)); break;
                case "FLM": stat = thing.GetStatValue(StatDefOf.Flammability); break;
                case "PTB": stat = thing.GetStatValue(StatDefOf.StuffPower_Armor_Blunt); break;
                case "PTS": stat = thing.GetStatValue(StatDefOf.StuffPower_Armor_Sharp); break;
                case "PTH": stat = thing.GetStatValue(StatDefOf.StuffPower_Armor_Heat); break;
                case "PTE":
                    StatDef protElectric = DefDatabase<StatDef>.GetNamed(ProtElectricStat, false);
                    if (protElectric != null) { stat = thing.GetStatValue(protElectric); }
                    break;
                case "INH": stat = thing.GetStatValue(StatDefOf.StuffPower_Insulation_Heat); break;
                case "INC": stat = thing.GetStatValue(StatDefOf.StuffPower_Insulation_Cold); break;
                case "WSP": stat = thing.GetStatValue(StatDefOf.SharpDamageMultiplier); break;
                case "WBT": stat = thing.GetStatValue(StatDefOf.BluntDamageMultiplier); break;
                default: stat = 0f; break;
            }

            string debugPos = "(" + thing.Position.x.ToString() + ", " + thing.Position.z.ToString() + ")";
            string debugMsg = "Debug " + BMixMode + " " + indx.ToString() + " " + billGiver.ThingID + " " + thing.LabelShort + " " + debugPos + " " + stat.ToString("F2");
            return debugMsg;
        }

        public static bool BMIsForbidden(Thing thing)
        {
            if (thing != null)
            {
                if (thing.TryGetComp<CompForbiddable>() != null)
                {
                    if (thing.TryGetComp<CompForbiddable>().Forbidden)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool BMStockInRadius(Thing t, Thing billGiver, Bill bill)
        {
            if (Controller.Settings.mapStock)
            {
                return true;
            }
            return ((float)(t.Position - billGiver.Position).LengthHorizontalSquared < bill.ingredientSearchRadius * bill.ingredientSearchRadius);
        }

        public static Predicate<Thing> BestMixValidator(Pawn pawn, Thing billGiver, Bill bill)
        {
            Predicate<Thing> Validator = (Thing t) => t.Spawned && !t.IsForbidden(pawn)
                                            && (float)(t.Position - billGiver.Position).LengthHorizontalSquared < bill.ingredientSearchRadius * bill.ingredientSearchRadius
                                            && bill.IsFixedOrAllowedIngredient(t) && bill.recipe.ingredients.Any((IngredientCount ingNeed) => ingNeed.filter.Allows(t))
                                            && pawn.CanReserve(t);
            return Validator;
        }

        private class BMixDefCountList
        {
            private List<ThingDef> defs = new List<ThingDef>();

            private List<float> counts = new List<float>();

            public int Count => defs.Count;

            public float this[ThingDef def]
            {
                get
                {
                    int num = defs.IndexOf(def);
                    if (num < 0)
                    {
                        return 0f;
                    }
                    return counts[num];
                }
                set
                {
                    int num = defs.IndexOf(def);
                    if (num < 0)
                    {
                        defs.Add(def);
                        counts.Add(value);
                        num = defs.Count - 1;
                    }
                    else
                    {
                        counts[num] = value;
                    }
                    CheckRemove(num);
                }
            }

            public float GetCount(int index)
            {
                return counts[index];
            }

            public void SetCount(int index, float val)
            {
                counts[index] = val;
                CheckRemove(index);
            }

            public ThingDef GetDef(int index)
            {
                return defs[index];
            }

            private void CheckRemove(int index)
            {
                if (counts[index] == 0f)
                {
                    counts.RemoveAt(index);
                    defs.RemoveAt(index);
                }
            }

            public void Clear()
            {
                defs.Clear();
                counts.Clear();
            }

            public void GenerateFrom(List<Thing> things)
            {
                Clear();
                for (int i = 0; i < things.Count; i++)
                {
                    this[things[i].def] += things[i].stackCount;
                }
            }
        }

        internal static bool TryFindBestMixInSet(List<Thing> availableThings, Bill bill, List<ThingCount> chosen, List<IngredientCount> ingredientsOrdered)
        {
            if (bill.recipe.allowMixingIngredients)
            {
                return TryFindBestMixInSet_AllowMix(availableThings, bill, chosen);
            }
            return TryFindBestMixInSet_NoMix(availableThings, bill, chosen, ingredientsOrdered);
        }

        internal static bool TryFindBestMixInSet_NoMix(List<Thing> availableThings, Bill bill, List<ThingCount> chosen, List<IngredientCount> ingredientsOrdered)
        {
            BMixDefCountList availableCounts = new BMixDefCountList();
            RecipeDef recipe = bill.recipe;
            chosen.Clear();
            availableCounts.Clear();
            availableCounts.GenerateFrom(availableThings);
            for (int i = 0; i < ingredientsOrdered.Count; i++)
            {
                IngredientCount ingredientCount = recipe.ingredients[i];
                bool flag = false;
                for (int j = 0; j < availableCounts.Count; j++)
                {
                    float num = ingredientCount.CountRequiredOfFor(availableCounts.GetDef(j), bill.recipe);
                    if (num > availableCounts.GetCount(j) || !ingredientCount.filter.Allows(availableCounts.GetDef(j)) || (!ingredientCount.IsFixedIngredient && !bill.ingredientFilter.Allows(availableCounts.GetDef(j))))
                    {
                        continue;
                    }
                    for (int k = 0; k < availableThings.Count; k++)
                    {
                        if (availableThings[k].def != availableCounts.GetDef(j))
                        {
                            continue;
                        }
                        int num2 = availableThings[k].stackCount - ThingCountUtility.CountOf(chosen, availableThings[k]);
                        if (num2 > 0)
                        {
                            int num3 = Mathf.Min(Mathf.FloorToInt(num), num2);
                            ThingCountUtility.AddToList(chosen, availableThings[k], num3);
                            num -= (float)num3;
                            if (num < 0.001f)
                            {
                                flag = true;
                                float count = availableCounts.GetCount(j);
                                count -= (float)ingredientCount.CountRequiredOfFor(availableCounts.GetDef(j), bill.recipe);
                                availableCounts.SetCount(j, count);
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                if (!flag)
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool TryFindBestMixInSet_AllowMix(List<Thing> availableThings, Bill bill, List<ThingCount> chosen)
        {
            chosen.Clear();
            for (int i = 0; i < bill.recipe.ingredients.Count; i++)
            {
                IngredientCount ingredientCount = bill.recipe.ingredients[i];
                float num = ingredientCount.GetBaseCount();
                for (int j = 0; j < availableThings.Count; j++)
                {
                    Thing thing = availableThings[j];
                    if (ingredientCount.filter.Allows(thing) && (ingredientCount.IsFixedIngredient || bill.ingredientFilter.Allows(thing)))
                    {
                        float num2 = bill.recipe.IngredientValueGetter.ValuePerUnitOf(thing.def);
                        int num3 = Mathf.Min(Mathf.CeilToInt(num / num2), thing.stackCount);
                        ThingCountUtility.AddToList(chosen, thing, num3);
                        num -= (float)num3 * num2;
                        if (num <= 0.0001f)
                        {
                            break;
                        }
                    }
                }
                if (num > 0.0001f)
                {
                    return false;
                }
            }
            return true;
        }

    }
}


