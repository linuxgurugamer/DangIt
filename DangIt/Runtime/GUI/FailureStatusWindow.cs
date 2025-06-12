using KSP.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ClickThroughFix;
using nsDangIt;

using static nsDangIt.DangIt;

namespace ippo.Runtime.GUI
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    internal class FailureStatusWindow : MonoBehaviour
    {

        internal static FailureStatusWindow instance;

        internal bool isVisible = false;
        private Rect settingsRect = new Rect(200, 200, 350, 300);

        internal static int failureWinHighlightID = -1;

        static List<Part> Parts = new List<Part>();


        void Start()
        {
            instance = this;
            if (FailureModule.phl != null)
            {
                if (failureWinHighlightID == -1)
                    failureWinHighlightID = FailureModule.phl.CreateHighlightList(0.25f, Color.red);

                FailureModule.phl.UpdateHighlightColors(failureWinHighlightID, Color.red);
            }
        }

        void OnDestroy()
        {
            FailureModule.phl.PauseHighlighting(FailureModule.vesselHighlightDict[FlightGlobals.ActiveVessel.id], false);
            foreach (var p in Parts)
                FailureModule.phl.RemovePartFromList(failureWinHighlightID, p);
            Parts.Clear();
        }

        protected void OnGUI()
        {
            if (!isVisible || !AlarmManager.visibleUI)
                return;
            UnityEngine.GUI.skin = HighLogic.Skin;

            settingsRect = ClickThruBlocker.GUILayoutWindow(
                Localizer.Format("#LOC_DangIt_235").GetHashCode(),
                settingsRect,
                FailureWindow,
                Localizer.Format("#LOC_DangIt_235"),
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));

        }



        Vector2 pos;
        void FailureWindow(int id)
        {
            GUILayout.BeginVertical();
            pos = GUILayout.BeginScrollView(pos);
            foreach (var part in FlightGlobals.ActiveVessel.Parts)
            {
                // Count all the failures on the part
                foreach (nsDangIt.FailureModule fm in part.Modules.OfType<nsDangIt.FailureModule>())
                {
                    if (fm.HasFailed)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(part.partInfo.title))
                        {
                            if (FailureModule.phl.HighlightListContains(failureWinHighlightID, part))
                                FailureModule.phl.RemovePartFromList(failureWinHighlightID, part);
                            else
                                FailureModule.phl.AddPartToHighlight(failureWinHighlightID, part);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localizer.Format("#LOC_DangIt_236")))
            {
                isVisible = false;
                FailureModule.phl.PauseHighlighting(FailureModule.vesselHighlightDict[FlightGlobals.ActiveVessel.id], false);
                foreach (var p in Parts)
                    FailureModule.phl.DisablePartHighlighting(failureWinHighlightID, p);

            }
            if (GUILayout.Button(Localizer.Format("#LOC_DangIt_237")))
            {
                AlarmManager alarmManager = FindObjectOfType<AlarmManager>();
                if (alarmManager != null)
                {
                    alarmManager.RemoveAllAlarms();
                }
            }
            if (GUILayout.Button(Localizer.Format("#LOC_DangIt_238")))
            {
                foreach (var p in FlightGlobals.ActiveVessel.Parts)
                {
                    if (FailureModule.phl.HighlightListContains(FailureModule.vesselHighlightDict[FlightGlobals.ActiveVessel.id], p))
                    {
                        FailureModule.phl.PauseHighlighting(FailureModule.vesselHighlightDict[FlightGlobals.ActiveVessel.id], true);
                        Parts.Add(p);
                    }
                }
                FailureModule.phl.EmptyList(failureWinHighlightID);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localizer.Format("#LOC_DangIt_239")))
            {
                foreach (var p in FlightGlobals.ActiveVessel.Parts)
                {
                    if (FailureModule.phl.HighlightListContains(FailureModule.vesselHighlightDict[FlightGlobals.ActiveVessel.id], p))
                    {
                        Log.Info("Disabling highlighted part: " + p.partInfo.title);
                        if (!FailureModule.phl.RemovePartFromList(FailureModule.vesselHighlightDict[FlightGlobals.ActiveVessel.id], p))
                            Log.Error("Error removing part from list: " + FailureModule.vesselHighlightDict[FlightGlobals.ActiveVessel.id] +
                                Localizer.Format("#LOC_DangIt_240") + p.partInfo.title);
                        AlarmManager.RemovePartFailure(p);
                        foreach (FailureModule fm in p.Modules.OfType<FailureModule>())
                            if (fm.HasFailed) fm.DisableAlarm();                       
                    }
                }
                FailureModule.phl.EmptyList(failureWinHighlightID);
                Parts.Clear();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            UnityEngine.GUI.DragWindow();
        }
    }
}
