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
    internal class FailureStatusWindow: MonoBehaviour
    {

        internal static FailureStatusWindow instance;

        internal bool isVisible = false;
        private Rect settingsRect = new Rect(200, 200, 350, 300);
        internal static int failedHighlightID = -1;
        List<Part> Parts = new List<Part>();
        void Start()
        {
            Log.Debug("FailureStatusWindow.Start");

            instance = this;
            if (failedHighlightID == -1)
                failedHighlightID = FailureModule.phl.CreateHighlightList(0.25f, Color.red);

            FailureModule.phl.UpdateHighlightColors(failedHighlightID, Color.red);


        }

        void OnDestroy()
        {
            Log.Debug("FailureStatusWindow.OnDestroy");
            FailureModule.phl.PauseHighlighting(FailureModule.vesselHighlightDict[FlightGlobals.ActiveVessel.id], false);
            foreach (var p in Parts)
                FailureModule.phl.DisablePartHighlighting(failedHighlightID, p);
            Parts.Clear();

        }
        void OnGUI()
        {
            if (!isVisible)
                return;
            UnityEngine.GUI.skin = HighLogic.Skin;

            settingsRect = ClickThruBlocker.GUILayoutWindow("Failure Status Window".GetHashCode(),
                                            settingsRect,
                                            FailureWindow,
                                            "Failure Status Window",
                                            GUILayout.ExpandWidth(true),
                                            GUILayout.ExpandHeight(true));

        }

        internal void RemoveBCRepaired(Part part)
        {
            if (FailureModule.phl.HighlightListContains(failedHighlightID, part))
                FailureModule.phl.RemovePartFromList(failedHighlightID, part);
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
                            if (FailureModule.phl.HighlightListContains(failedHighlightID, part))
                                FailureModule.phl.RemovePartFromList(failedHighlightID, part);
                            else
                                FailureModule.phl.AddPartToHighlight(failedHighlightID, part);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close"))
            {
                isVisible = false;
                FailureModule.phl.PauseHighlighting(FailureModule.vesselHighlightDict[FlightGlobals.ActiveVessel.id], false);
                foreach (var p in Parts)
                        FailureModule.phl.DisablePartHighlighting(failedHighlightID, p);

            }
            if (GUILayout.Button("Mute All"))
            {
                AlarmManager alarmManager = FindObjectOfType<AlarmManager>();
                if (alarmManager != null)
                {
                    alarmManager.RemoveAllAlarms();
                }
            }
            if (GUILayout.Button("Disable All Highlighting"))
            {
                foreach (var p in FlightGlobals.ActiveVessel.Parts)
                {
                    if (FailureModule.phl.HighlightListContains(FailureModule.vesselHighlightDict[FlightGlobals.ActiveVessel.id], p))
                    {
                        FailureModule.phl.PauseHighlighting(FailureModule.vesselHighlightDict[FlightGlobals.ActiveVessel.id], true);
                        Parts.Add(p);
                    }
                }
                     FailureModule.phl.EmptyList(failedHighlightID);
           }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            UnityEngine.GUI.DragWindow();
        }
    }
}
