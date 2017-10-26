/*
Copyright (c) 2014~2016, Justin Bengtson
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

  Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

  Redistributions in binary form must reproduce the above copyright notice, this
  list of conditions and the following disclaimer in the documentation and/or
  other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

namespace Renamer {
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class KerbalRenamer : MonoBehaviour {
		public static KerbalRenamer rInstance = null;
		private float badassPercent = 0.05f;
		private float femalePercent = 0.5f;
		private bool useBellCurveMethod = true;
		private bool dontInsultMe = false;
		private bool preserveOriginals = false;
        private bool preserveOriginalTraits = false;
		private bool generateNewStats = true;
		public string cultureDescriptor = "Culture";
		internal Culture[] cultures = {};

        public List<string> originalNames = new List<string> {
                "Jebediah Kerman",
                "Bill Kerman",
                "Bob Kerman",
                "Valentina Kerman"
            };

        public static KerbalRenamer Instance {
	        get {
				if((object)rInstance == null) {
					rInstance = (new GameObject("RenamerContainer")).AddComponent<KerbalRenamer>();
				}
	            return rInstance;
	        }
	    }

       
        public void Awake()
        {
            DontDestroyOnLoad(this);

            ConfigNode data = null;
            foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("KERBALRENAMER"))
            {
                data = node;
            }
            if ((object)data == null)
            {
                Debug.Log("KerbalRenamer: No config file found, thanks for playing.");
                return;
            }

            List<Culture> ctemp = new List<Culture>();
            if (data.HasValue("badassPercent"))
            {
                float ftemp = 0.0f;
                if (float.TryParse(data.GetValue("badassPercent"), out ftemp))
                {
                    badassPercent = ftemp;
                }
            }
            if (data.HasValue("femalePercent"))
            {
                float ftemp = 0.0f;
                if (float.TryParse(data.GetValue("femalePercent"), out ftemp))
                {
                    femalePercent = ftemp;
                }
            }
            if (data.HasValue("useBellCurveMethod"))
            {
                bool btemp = true;
                if (bool.TryParse(data.GetValue("useBellCurveMethod"), out btemp))
                {
                    useBellCurveMethod = btemp;
                }
            }
            if (data.HasValue("dontInsultMe"))
            {
                bool btemp = true;
                if (bool.TryParse(data.GetValue("dontInsultMe"), out btemp))
                {
                    dontInsultMe = btemp;
                }
            }
            if (data.HasValue("preserveOriginals"))
            {
                bool btemp = true;
                if (bool.TryParse(data.GetValue("preserveOriginals"), out btemp))
                {
                    preserveOriginals = btemp;
                }
            }
            if (data.HasValue("preserveOriginalTraits"))
            {
                bool btemp = true;
                if (bool.TryParse(data.GetValue("preserveOriginalTraits"), out btemp))
                {
                    preserveOriginalTraits = btemp;
                }
            }
            if (data.HasValue("generateNewStats"))
            {
                bool btemp = true;
                if (bool.TryParse(data.GetValue("generateNewStats"), out btemp))
                {
                    generateNewStats = btemp;
                }
            }
            if (data.HasValue("cultureDescriptor"))
            {
                cultureDescriptor = data.GetValue("cultureDescriptor");
            }
            ConfigNode[] cultureclub = data.GetNodes("Culture");
            for (int i = 0; i < cultureclub.Length; i++)
            {
                Culture c = new Culture(cultureclub[i]);
                ctemp.Add(c);
            }
            cultures = ctemp.ToArray();

            GameEvents.onKerbalAdded.Add(new EventData<ProtoCrewMember>.OnEvent(OnKerbalAdded));
            GameEvents.onGameStateCreated.Add(new EventData<Game>.OnEvent(OnGameCreated));
        }

        void OnGameCreated(Game game)
        {
            //if (!preserveOriginals)
            //{
            //    RerollOriginals();
            //}
        }

        public void OnKerbalAdded(ProtoCrewMember kerbal)
		{
			if (preserveOriginals) {
				if (originalNames.Contains(kerbal.name)) {
					return;
				}
			}
            else // see if any of the originals are still around
            {
                RerollOriginals();
            }

            Randomizer.RerollKerbal(ref kerbal,generateNewStats, useBellCurveMethod, dontInsultMe, badassPercent, femalePercent, cultures, preserveOriginalTraits);
		}
        

        private void RerollOriginals()
        {
            foreach (var originalKerbalName in originalNames)
            {
                if (HighLogic.CurrentGame.CrewRoster[originalKerbalName] != null)
                {
                    var origKerbal = HighLogic.CurrentGame.CrewRoster[originalKerbalName];
                    var origTrait = origKerbal.trait;
                    Randomizer.RerollKerbal(ref origKerbal, generateNewStats, useBellCurveMethod, dontInsultMe, badassPercent, femalePercent, cultures, preserveOriginalTraits);                    
                    KerbalRoster.SetExperienceTrait(origKerbal, origTrait);
                }
            }
        }        
	}
    
}