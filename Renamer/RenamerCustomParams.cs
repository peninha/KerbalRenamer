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
        public override string Section { get { return "Kerbal Renamer"; } }
        public override string DisplaySection { get { return Section; } }
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

        public static bool PreserveOriginal4Enabled
        {
            get
            {
                RenamerCustomParams options = HighLogic.CurrentGame.Parameters.CustomParams<RenamerCustomParams>();
                return options.preserveOriginals;
            }
        }

        public static bool PreserveOriginalTraitsEnabled
        {
            get
            {
                RenamerCustomParams options = HighLogic.CurrentGame.Parameters.CustomParams<RenamerCustomParams>();
                return options.preserveOriginalTraits;
            }
        }

        public static bool RerollStatsEnabled
        {
            get
            {
                RenamerCustomParams options = HighLogic.CurrentGame.Parameters.CustomParams<RenamerCustomParams>();
                return options.generateNewStats;
            }
        }

        public static float GetFemalePercentage
        {
            get
            {
                RenamerCustomParams options = HighLogic.CurrentGame.Parameters.CustomParams<RenamerCustomParams>();
                return options.femalePercent;
            }
        }

        public static float GetBadassPercentage
        {
            get
            {
                RenamerCustomParams options = HighLogic.CurrentGame.Parameters.CustomParams<RenamerCustomParams>();
                return options.badassPercent;
            }
        }

        public static bool BellCurveEnabled
        {
            get
            {
                RenamerCustomParams options = HighLogic.CurrentGame.Parameters.CustomParams<RenamerCustomParams>();
                return options.useBellCurveMethod;
            }
        }

        public static bool DontInsultMeEnabled
        {
            get
            {
                RenamerCustomParams options = HighLogic.CurrentGame.Parameters.CustomParams<RenamerCustomParams>();
                return options.dontInsultMe;
            }
        }

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            base.SetDifficultyPreset(preset);
        }

    }
}
