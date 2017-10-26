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

    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class IconChanger_SpaceCentre : IconChanger
    {
        public void Awake()
        {
            GameEvents.onGUIAstronautComplexSpawn.Add(new EventVoid.OnEvent(OnGUIAstronautComplexSpawn));
            GameEvents.onGUILaunchScreenVesselSelected.Add(new EventData<ShipTemplate>.OnEvent(OnGUILaunchScreenVesselSelected));
        }
    }

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class IconChanger_EditorAny : IconChanger
    {
        public void Awake()
        {
            GameEvents.onEditorScreenChange.Add(new EventData<EditorScreen>.OnEvent(OnEditorScreenChange));
        }
    }

    public class IconChanger : MonoBehaviour
    {
        public void OnGUIAstronautComplexSpawn()
        {
            StartCoroutine(CallbackUtil.DelayedCallback(1, BuildAstronautComplex));
        }

        public void OnGUILaunchScreenVesselSelected(ShipTemplate t)
        {
            StartCoroutine(CallbackUtil.DelayedCallback(1, BuildCrewAssignmentDialogue));
        }

        public void OnEditorScreenChange(EditorScreen e)
        {
            if (e == EditorScreen.Crew)
            {
                StartCoroutine(CallbackUtil.DelayedCallback(1, BuildCrewAssignmentDialogue));
            }
        }

        public void BuildAstronautComplex()
        {
            ConfigNode data = null;
            foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("KERBALRENAMER"))
            {
                data = node;
            }


            KSP.UI.CrewListItem cic;
            KSP.UI.UIList scroll;

            UnityEngine.Object[] objs = Resources.FindObjectsOfTypeAll(typeof(KSP.UI.Screens.AstronautComplex));
            if (objs.Length < 1) { return; }
            KSP.UI.Screens.AstronautComplex complex = (KSP.UI.Screens.AstronautComplex)objs[0];
            FieldInfo[] scrolls = typeof(KSP.UI.Screens.AstronautComplex).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(c => c.FieldType == typeof(KSP.UI.UIList)).ToArray();

            for (int i = 0; i < scrolls.Length; i++)
            {
                scroll = (KSP.UI.UIList)scrolls[i].GetValue(complex);
                for (int j = 0; j < scroll.Count; j++)
                {
                    KSP.UI.UIListItem listItem = scroll.GetUilistItemAt(j);
                    cic = listItem.GetComponent<KSP.UI.CrewListItem>();
                    cic.AddButtonInputDelegate(new UnityAction<KSP.UI.CrewListItem.ButtonTypes, KSP.UI.CrewListItem>(RebuildAstronautComplex));
                    changeKerbalIcon(cic);
                }
            }
        }

        public void BuildCrewAssignmentDialogue()
        {
            if ((object)KSP.UI.CrewAssignmentDialog.Instance == null)
            {
                return;
            }

            KSP.UI.CrewAssignmentDialog dialogue = KSP.UI.CrewAssignmentDialog.Instance;
            KSP.UI.CrewListItem cic;

            for (int j = 0; j < dialogue.scrollListAvail.Count; j++)
            {
                KSP.UI.UIListItem listItem = dialogue.scrollListAvail.GetUilistItemAt(j);
                cic = listItem.GetComponent<KSP.UI.CrewListItem>();
                cic.AddButtonInputDelegate(new UnityAction<KSP.UI.CrewListItem.ButtonTypes, KSP.UI.CrewListItem>(RebuildCrewAssignmentDialogue));
                changeKerbalIcon(cic);
            }
            for (int j = 0; j < dialogue.scrollListCrew.Count; j++)
            {
                KSP.UI.UIListItem listItem = dialogue.scrollListCrew.GetUilistItemAt(j);
                cic = listItem.GetComponent<KSP.UI.CrewListItem>();
                if ((object)cic != null)
                {
                    cic.AddButtonInputDelegate(new UnityAction<KSP.UI.CrewListItem.ButtonTypes, KSP.UI.CrewListItem>(RebuildCrewAssignmentDialogue));
                    changeKerbalIcon(cic);
                }
            }
        }

        public void RebuildAstronautComplex(KSP.UI.CrewListItem.ButtonTypes type, KSP.UI.CrewListItem cic)
        {
            StartCoroutine(CallbackUtil.DelayedCallback(1, BuildAstronautComplex));
        }

        public void RebuildCrewAssignmentDialogue(KSP.UI.CrewListItem.ButtonTypes type, KSP.UI.CrewListItem cic)
        {
            StartCoroutine(CallbackUtil.DelayedCallback(1, BuildCrewAssignmentDialogue));
        }

        private void changeKerbalIcon(KSP.UI.CrewListItem cic)
        {
            if ((object)cic.GetCrewRef() == null)
            {
                return;
            }

            ConfigNode data = null;
            foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("KERBALRENAMER"))
            {
                data = node;
            }
            List<Culture> ctemp = new List<Culture>();
            ConfigNode[] cultureclub = data.GetNodes("Culture");
            for (int i = 0; i < cultureclub.Length; i++)
            {
                Culture c = new Culture(cultureclub[i]);
                ctemp.Add(c);
            }
            Culture[] cultures = ctemp.ToArray();

            FlightLog.Entry flight = cic.GetCrewRef().flightLog.Entries.FirstOrDefault(e => e.type == KerbalRenamer.Instance.cultureDescriptor);
            if ((object)flight != null)
            {
                FieldInfo fi = typeof(KSP.UI.CrewListItem).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(c => c.FieldType == typeof(RawImage));
                RawImage foo = (RawImage)fi.GetValue(cic);
                Culture culture = Randomizer.getCultureByName(flight.target, cultures);
                if ((object)culture != null)
                {
                    foo.texture = (Texture)GameDatabase.Instance.GetTexture("KerbalRenamer/Icons/" + culture.cultureName, false);
                }
            }
        }
    }
}
