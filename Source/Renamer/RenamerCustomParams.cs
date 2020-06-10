using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Renamer
{
    public class RenamerCustomParams : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Kerbal Renamer Options"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string DisplaySection { get { return "Kerbal Renamer"; } }
        public override string Section { get { return "Kerbal Renamer"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }

        [GameParameters.CustomParameterUI("Preserve Original 4", toolTip = "Keep Jeb, Bill, Bob and Val?", autoPersistance = true)]
        public bool preserveOriginals = false;
        [GameParameters.CustomParameterUI("Preserve Original Traits", toolTip = "Set to start with 2 pilots, 1 engineer, and 1 scientist.  Disable to take your chances.", autoPersistance = true)]
        public bool preserveOriginalTraits = true;
        [GameParameters.CustomParameterUI("Reroll stats", toolTip = "Set to get random stats (courage, stupidity, roles, and gender).", autoPersistance = true)]
        public bool generateNewStats = false;
        [GameParameters.CustomFloatParameterUI("Female Percentage", toolTip = "What percentage of recruits should be female?", autoPersistance = true, asPercentage = true, displayFormat = "N1", minValue = 0.0f, maxValue = 1.0f)]
        public float femalePercent = 0.5f;
        [GameParameters.CustomFloatParameterUI("Badass Percentage", toolTip = "What percentage of recruits should be Badass?", autoPersistance = true, asPercentage = true, displayFormat = "N1", minValue = 0.0f, maxValue = 1.0f)]
        public float badassPercent = 0.5f;
        [GameParameters.CustomParameterUI("Use Bell Curve Method", toolTip = "Use a Bell Curve distribution when rolling stats?", autoPersistance = true)]
        public bool useBellCurveMethod = true;
        [GameParameters.CustomParameterUI("Don't Insult Me", toolTip = "Limits stupidity", autoPersistance = true)]
        public bool dontInsultMe = true;

        public static RenamerCustomParams OptionsInstance
        {
            get
            {
                RenamerCustomParams options = HighLogic.CurrentGame?.Parameters?.CustomParams<RenamerCustomParams>();
                if (options == null)
                {
                    options = new RenamerCustomParams();
                }
                return options;
            }
        }

        public static bool PreserveOriginal4Enabled
        {
            get
            {
                return OptionsInstance.preserveOriginals;
            }
        }

        public static bool PreserveOriginalTraitsEnabled
        {
            get
            {
                return OptionsInstance.preserveOriginalTraits;
            }
        }

        public static bool RerollStatsEnabled
        {
            get
            {
                return OptionsInstance.generateNewStats;
            }
        }

        public static float GetFemalePercentage
        {
            get
            {
                return OptionsInstance.femalePercent;
            }
        }

        public static float GetBadassPercentage
        {
            get
            {
                return OptionsInstance.badassPercent;
            }
        }

        public static bool BellCurveEnabled
        {
            get
            {
                return OptionsInstance.useBellCurveMethod;
            }
        }

        public static bool DontInsultMeEnabled
        {
            get
            {
                return OptionsInstance.dontInsultMe;
            }
        }

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            base.SetDifficultyPreset(preset);
        }
    }
}
