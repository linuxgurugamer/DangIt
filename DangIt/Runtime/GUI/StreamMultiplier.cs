using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

using ClickThroughFix;

namespace ippo.Runtime.GUI
{
    using static nsDangIt.DangIt;

    class StreamMultiplier : MonoBehaviour
    {
        private Rect settingsRect = new Rect(200, 200, 350, 150);
        
        internal string multiplier = "";
        internal string decay = "";
        internal bool visible = false;

        void Start()
        {
        }


        void OnGUI()
        {
            if (!visible)
                return;
            UnityEngine.GUI.skin = HighLogic.Skin;
            settingsRect = ClickThruBlocker.GUILayoutWindow("Multiplier Entry".GetHashCode(),                                            settingsRect,                                            SettingsWindowFcn,                                            "Multiplier Entry",                                            GUILayout.ExpandWidth(true),
                                            GUILayout.ExpandHeight(true));
        }



        void SettingsWindowFcn(int windowID)
        {            GUILayout.BeginHorizontal();            GUILayout.Label("Current modifier: " + nsDangIt.FailureModule.streamMultiplier.ToString());            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();            GUILayout.Label("Enter additional modifier: ");

            double m;
            var   newMultiplier = GUILayout.TextField(multiplier, GUILayout.Width(90));
            try
            {
                m = float.Parse(newMultiplier);
                multiplier = m.ToString();

            } catch
            { }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();            GUILayout.Label("Enter decrease per minute: ");

            double d;
            var newDecay = GUILayout.TextField(decay, GUILayout.Width(90));
            try
            {
                d = float.Parse(newDecay);
                decay = d.ToString();

            }
            catch
            { }

            GUILayout.EndHorizontal();


            GUILayout.FlexibleSpace();            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); 
            if (GUILayout.Button("OK", GUILayout.Width(60)))            {
                if (multiplier != "")
                    nsDangIt.FailureModule.streamMultiplier += float.Parse(multiplier);
                if (decay != "")
                    nsDangIt.FailureModule.decayPerMinute += float.Parse(decay);
                nsDangIt.FailureModule.lastDecayTime = Planetarium.GetUniversalTime();
                visible = false;
            }
            if (GUILayout.Button("Cancel", GUILayout.Width(60)))            {
                visible = false;
            }
            GUILayout.FlexibleSpace();            GUILayout.EndHorizontal();            UnityEngine.GUI.DragWindow();
        }
    }
}
