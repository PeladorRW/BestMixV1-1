using UnityEngine;
using Verse;

namespace BestMix
{
    public class Controller : Mod
    {
        public override string SettingsCategory()
        {
            return "BestMix.Name".Translate();
        }

        public override void DoSettingsWindowContents(Rect canvas)
        {
            Controller.Settings.DoWindowContents(canvas);
        }

        public Controller(ModContentPack content) : base(content)
        {
            Controller.Settings = base.GetSettings<Settings>();
        }

        public static Settings Settings;
    }

}

