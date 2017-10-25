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

namespace regexKSP {
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class KerbalRenamerOfOriginals : MonoBehaviour {
		public static KerbalRenamer rInstance = null;
		private float badassPercent = 0.05f;
		private float femalePercent = 0.5f;
		private bool useBellCurveMethod = true;
		private bool dontInsultMe = false;
		private bool preserveOriginals = false;
		private bool generateNewStats = true;
		public string cultureDescriptor = "Culture";
		private Culture[] cultures = {};

        private List<string> originalNames = new List<string> {
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

        public void Start()
        {
            if (Instance)
            {
                DestroyImmediate(gameObject);
            }
            else
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

            GameEvents.onGameStateCreated.Add(new EventData<Game>.OnEvent(OnGameCreated));
        }
        }

        void OnGameCreated(Game game)
        {
            if (!preserveOriginals)
            {
                RerollOriginals();
            }
        }
        
        private void RerollOriginals()
        {
            foreach (var originalKerbalName in originalNames)
            {
                if (HighLogic.CurrentGame.CrewRoster[originalKerbalName] != null)
                {
                    var origKerbal = HighLogic.CurrentGame.CrewRoster[originalKerbalName];
                    var origTrait = origKerbal.trait;
                    RerollKerbal(origKerbal);
                    KerbalRoster.SetExperienceTrait(origKerbal, origTrait);
                }
            }
        }

		private void RerollKerbal(ProtoCrewMember kerbal)
		{
			UnityEngine.Random.InitState(System.DateTime.Now.Millisecond * kerbal.name.GetHashCode());

			if (generateNewStats) {
				if (kerbal.type == ProtoCrewMember.KerbalType.Crew || kerbal.type == ProtoCrewMember.KerbalType.Applicant) {
					// generate some new stats
					kerbal.stupidity = rollStupidity();
					kerbal.courage = rollCourage();
					kerbal.isBadass = (UnityEngine.Random.Range(0.0f, 1.0f) < badassPercent);

					float rand = UnityEngine.Random.Range(0.0f, 1.0f);
					if (rand < 0.33f) {
						KerbalRoster.SetExperienceTrait(kerbal, "Pilot");
					}
					else if (rand < 0.66f) {
						KerbalRoster.SetExperienceTrait(kerbal, "Engineer");
					}
					else {
						KerbalRoster.SetExperienceTrait(kerbal, "Scientist");
					}

					if (UnityEngine.Random.Range(0.0f, 1.0f) < femalePercent) {
						kerbal.gender = ProtoCrewMember.Gender.Female;
					}
					else {
						kerbal.gender = ProtoCrewMember.Gender.Male;
					}
				}
			}

			string name = this.getName(kerbal);
			if (name.Length > 0) {
				kerbal.ChangeName(name);
			}
		}

		private string getName(ProtoCrewMember c) {
	        string firstName = "";
			string lastName = "";

			Culture parent = cultures[UnityEngine.Random.Range(0, cultures.Length)];
			if(c.gender == ProtoCrewMember.Gender.Female) {
				if(parent.fnames1.Length > 0) {
			        firstName += parent.fnames1[UnityEngine.Random.Range(0, parent.fnames1.Length)];
				}
				if(parent.fnames2.Length > 0) {
			        firstName += parent.fnames2[UnityEngine.Random.Range(0, parent.fnames2.Length)];
				}
				if(parent.fnames3.Length > 0) {
			        firstName += parent.fnames3[UnityEngine.Random.Range(0, parent.fnames3.Length)];
				}
			} else {
				if(parent.mnames1.Length > 0) {
			        firstName += parent.mnames1[UnityEngine.Random.Range(0, parent.mnames1.Length)];
				}
				if(parent.mnames2.Length > 0) {
			        firstName += parent.mnames2[UnityEngine.Random.Range(0, parent.mnames2.Length)];
				}
				if(parent.mnames3.Length > 0) {
			        firstName += parent.mnames3[UnityEngine.Random.Range(0, parent.mnames3.Length)];
				}
			}
			if(parent.femaleSurnamesExist && c.gender == ProtoCrewMember.Gender.Female) {
				if(parent.flnames1.Length > 0) {
			        lastName += parent.flnames1[UnityEngine.Random.Range(0, parent.flnames1.Length)];
				}
				if(parent.flnames2.Length > 0) {
			        lastName += parent.flnames2[UnityEngine.Random.Range(0, parent.flnames2.Length)];
				}
				if(parent.flnames3.Length > 0) {
			        lastName += parent.flnames3[UnityEngine.Random.Range(0, parent.flnames3.Length)];
				}
			} else {
				if(parent.lnames1.Length > 0) {
			        lastName += parent.lnames1[UnityEngine.Random.Range(0, parent.lnames1.Length)];
				}
				if(parent.lnames2.Length > 0) {
			        lastName += parent.lnames2[UnityEngine.Random.Range(0, parent.lnames2.Length)];
				}
				if(parent.lnames3.Length > 0) {
			        lastName += parent.lnames3[UnityEngine.Random.Range(0, parent.lnames3.Length)];
				}
			}
			if(lastName.Length > 0) {
				if(firstName.Length > 0) {
					if(parent.cultureName.Length > 0) {
						c.flightLog.AddEntryUnique(new FlightLog.Entry(0, cultureDescriptor, parent.cultureName));
					}
					return firstName + " " + lastName;
				} else {
					if(parent.cultureName.Length > 0) {
						c.flightLog.AddEntryUnique(new FlightLog.Entry(0, cultureDescriptor, parent.cultureName));
					}
					return lastName;
				}
			} else {
				// 0 length names should be handled elsewhere.
				return firstName;
			}
	    }

		public Culture getCultureByName(string name) {
			for(int i = 0; i < cultures.Length; i++) {
				if(cultures[i].cultureName == name) {
					return cultures[i];
				}
			}
			return null;
		}

		private float rollCourage() {
			if(useBellCurveMethod) {
				float retval = -0.05f;
				for(int i = 0; i < 6; i++) {
					retval += UnityEngine.Random.Range(0.01f, 0.21f);
				}
				return retval;
			} else {
				return UnityEngine.Random.Range(0.0f, 1.0f);
			}
		}

		private float rollStupidity() {
			if(useBellCurveMethod) {
				float retval = -0.05f;
				int end = 6;
				if(dontInsultMe) { end = 4; }
				for(int i = 0; i < end; i++) {
					retval += UnityEngine.Random.Range(0.01f, 0.21f);
				}
				if(retval < 0.001f) { retval = 0.001f; }
				return retval;
			} else {
				return UnityEngine.Random.Range(0.0f, 1.0f);
			}
		}
	}
    
}