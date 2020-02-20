﻿using System;
using UnityEngine;
using Verse;

namespace BestMix
{
    public class Settings : ModSettings
    {
        public void DoWindowContents(Rect canvas)
        {
            float gap = 10f;
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = canvas.width;
            listing_Standard.Begin(canvas);
            listing_Standard.Gap(gap);
            checked
            {
                listing_Standard.CheckboxLabeled("BestMix.AllowBestMix".Translate(), ref AllowBestMix, null);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled("BestMix.AllowMealMakersOnly".Translate(), ref AllowMealMakersOnly, null);
                listing_Standard.Gap(gap * 2);

                if (RadiusRestrict)
                {
                    listing_Standard.CheckboxLabeled("BestMix.UseRadiusLimit".Translate(), ref UseRadiusLimit, null);
                    listing_Standard.Gap(gap);
                    listing_Standard.Label("BestMix.RadiusLimit".Translate() + "  " + (int)RadiusLimit, -1f, null);
                    RadiusLimit = (int)listing_Standard.Slider((int)RadiusLimit, 10f, 100f);
                    listing_Standard.Gap(gap);
                }

                if ((Prefs.DevMode) && (DebugMaster))
                {
                    listing_Standard.Gap(gap * 2);
                    listing_Standard.CheckboxLabeled("BestMix.IncludeRegionLimiter".Translate(), ref IncludeRegionLimiter, null);
                    listing_Standard.Gap(gap * 2);
                    listing_Standard.CheckboxLabeled("BestMix.DebugSort".Translate(), ref DebugSort, null);
                    listing_Standard.Gap(gap);
                    listing_Standard.CheckboxLabeled("BestMix.DebugChosen".Translate(), ref DebugChosen, null);
                    listing_Standard.Gap(gap);
                    listing_Standard.CheckboxLabeled("BestMix.DebugFound".Translate(), ref DebugFound, null);
                    listing_Standard.Gap(gap * 2);
                    listing_Standard.CheckboxLabeled("BestMix.DebugIgnore".Translate(), ref DebugIgnore, null);
                    listing_Standard.Gap(gap);
                }

                listing_Standard.End();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref AllowBestMix, "AllowBestMix", true, false);
            Scribe_Values.Look<bool>(ref AllowMealMakersOnly, "AllowMealMakersOnly", false, false);
            Scribe_Values.Look<bool>(ref UseRadiusLimit, "UseRadiusLimit", false, false);
            Scribe_Values.Look<int>(ref RadiusLimit, "RadiusLimit", 100, false);
            Scribe_Values.Look<bool>(ref IncludeRegionLimiter, "IncludeRegionLimiter", true, false);
            Scribe_Values.Look<bool>(ref DebugSort, "DebugSort", false, false);
            Scribe_Values.Look<bool>(ref DebugChosen, "DebugChosen", false, false);
            Scribe_Values.Look<bool>(ref DebugFound, "DebugFound", false, false);
            Scribe_Values.Look<bool>(ref DebugIgnore, "DebugIgnore", false, false);
        }

        public bool AllowBestMix = true;
        public bool AllowMealMakersOnly = false;
        public bool UseRadiusLimit = false;
        public int RadiusLimit = 100;

        public bool IncludeRegionLimiter = true;
        public bool DebugSort = false;
        public bool DebugChosen = false;
        public bool DebugFound = false;
        public bool DebugIgnore = false;

        public bool DebugMaster = false;
        public bool RadiusRestrict = false;
    }
}


