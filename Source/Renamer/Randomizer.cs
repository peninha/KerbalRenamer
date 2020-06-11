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

namespace Renamer
{
    public class Randomizer
    {
        public static void RerollKerbal(ProtoCrewMember kerbal, Culture[] cultures)
        {
            RerollKerbal(kerbal, RenamerCustomParams.BellCurveEnabled, RenamerCustomParams.DontInsultMeEnabled, RenamerCustomParams.GetBadassPercentage,
                         RenamerCustomParams.GetFemalePercentage, cultures, RenamerCustomParams.PreserveOriginalTraitsEnabled);
        }

        public static void RerollKerbal(ProtoCrewMember kerbal, bool useBellCurveMethod, bool dontInsultMe, float badassPercent, float femalePercent, Culture[] cultures, bool keepRoles)
        {
            LogUtils.Log("Rerolling kerbal ", kerbal.name);
            UnityEngine.Random.InitState(DateTime.Now.Millisecond * kerbal.name.GetHashCode());

            if (kerbal.type == ProtoCrewMember.KerbalType.Crew || kerbal.type == ProtoCrewMember.KerbalType.Applicant)
            {
                if (useBellCurveMethod || dontInsultMe)
                {
                    kerbal.stupidity = rollStupidity(useBellCurveMethod, dontInsultMe);
                    kerbal.courage = rollCourage(useBellCurveMethod);
                }

                kerbal.isBadass = UnityEngine.Random.Range(0.0f, 1.0f) < badassPercent;

                float traitRoll = UnityEngine.Random.Range(0.0f, 1.0f);
                if (keepRoles)
                {
                    KerbalRoster.SetExperienceTrait(kerbal, kerbal.trait);
                }
                else
                {
                    if (traitRoll < 0.33f)
                    {
                        KerbalRoster.SetExperienceTrait(kerbal, "Pilot");
                    }
                    else if (traitRoll < 0.66f)
                    {
                        KerbalRoster.SetExperienceTrait(kerbal, "Engineer");
                    }
                    else
                    {
                        KerbalRoster.SetExperienceTrait(kerbal, "Scientist");
                    }
                }

                if (UnityEngine.Random.Range(0.0f, 1.0f) <= femalePercent)
                {
                    kerbal.gender = ProtoCrewMember.Gender.Female;
                }
                else
                {
                    kerbal.gender = ProtoCrewMember.Gender.Male;
                }
            }

            string name = getName(kerbal, cultures);

            LogUtils.Log("Renaming to ", name);
            if (name.Length > 0)
            {
                kerbal.ChangeName(name);
            }
        }

        public static string getName(ProtoCrewMember c, Culture[] cultures)
        {
            string firstName = "";
            string lastName = "";

            Culture parent = cultures[UnityEngine.Random.Range(0, cultures.Length)];
            if (c.gender == ProtoCrewMember.Gender.Female)
            {
                if (parent.fnames1.Length > 0)
                {
                    firstName += parent.fnames1[UnityEngine.Random.Range(0, parent.fnames1.Length)];
                }
                if (parent.fnames2.Length > 0)
                {
                    firstName += parent.fnames2[UnityEngine.Random.Range(0, parent.fnames2.Length)];
                }
                if (parent.fnames3.Length > 0)
                {
                    firstName += parent.fnames3[UnityEngine.Random.Range(0, parent.fnames3.Length)];
                }
            }
            else
            {
                if (parent.mnames1.Length > 0)
                {
                    firstName += parent.mnames1[UnityEngine.Random.Range(0, parent.mnames1.Length)];
                }
                if (parent.mnames2.Length > 0)
                {
                    firstName += parent.mnames2[UnityEngine.Random.Range(0, parent.mnames2.Length)];
                }
                if (parent.mnames3.Length > 0)
                {
                    firstName += parent.mnames3[UnityEngine.Random.Range(0, parent.mnames3.Length)];
                }
            }
            if (parent.femaleSurnamesExist && c.gender == ProtoCrewMember.Gender.Female)
            {
                if (parent.flnames1.Length > 0)
                {
                    lastName += parent.flnames1[UnityEngine.Random.Range(0, parent.flnames1.Length)];
                }
                if (parent.flnames2.Length > 0)
                {
                    lastName += parent.flnames2[UnityEngine.Random.Range(0, parent.flnames2.Length)];
                }
                if (parent.flnames3.Length > 0)
                {
                    lastName += parent.flnames3[UnityEngine.Random.Range(0, parent.flnames3.Length)];
                }
            }
            else
            {
                if (parent.lnames1.Length > 0)
                {
                    lastName += parent.lnames1[UnityEngine.Random.Range(0, parent.lnames1.Length)];
                }
                if (parent.lnames2.Length > 0)
                {
                    lastName += parent.lnames2[UnityEngine.Random.Range(0, parent.lnames2.Length)];
                }
                if (parent.lnames3.Length > 0)
                {
                    lastName += parent.lnames3[UnityEngine.Random.Range(0, parent.lnames3.Length)];
                }
            }
            if (lastName.Length > 0)
            {
                if (firstName.Length > 0)
                {
                    if (parent.cultureName.Length > 0)
                    {
                        c.flightLog.AddEntryUnique(new FlightLog.Entry(0, KerbalRenamer.Instance.cultureDescriptor, parent.cultureName));
                    }
                    return firstName + " " + lastName;
                }
                else
                {
                    if (parent.cultureName.Length > 0)
                    {
                        c.flightLog.AddEntryUnique(new FlightLog.Entry(0, KerbalRenamer.Instance.cultureDescriptor, parent.cultureName));
                    }
                    return lastName;
                }
            }
            else
            {
                // 0 length names should be handled elsewhere.
                return firstName;
            }
        }

        public static Culture getCultureByName(string name, Culture[] cultures)
        {
            for (int i = 0; i < cultures.Length; i++)
            {
                if (cultures[i].cultureName == name)
                {
                    return cultures[i];
                }
            }
            return null;
        }

        public static float rollCourage(bool useBellCurveMethod)
        {
            if (useBellCurveMethod)
            {
                float retval = 0;
                for (int i = 0; i < 5; i++)
                {
                    retval += UnityEngine.Random.Range(0f, 0.2f);
                }
                return retval;
            }
            else
            {
                return UnityEngine.Random.Range(0f, 1f);
            }
        }

        public static float rollStupidity(bool useBellCurveMethod, bool dontInsultMe)
        {
            if (useBellCurveMethod)
            {
                float retval = 0;
                int end = dontInsultMe ? 3 : 5;
                for (int i = 0; i < end; i++)
                {
                    retval += UnityEngine.Random.Range(0f, 0.2f);
                }
                return retval;
            }
            else
            {
                return UnityEngine.Random.Range(0.0f, 1.0f);
            }
        }
    }
}