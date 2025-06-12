using KSP.Localization;
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

            settingsRect = ClickThruBlocker.GUILayoutWindow(Localizer.Format("#LOC_DangIt_265").GetHashCode(),
                                            settingsRect,
                                            SettingsWindowFcn,
                                            Localizer.Format("#LOC_DangIt_265"),
                                            GUILayout.ExpandWidth(true),
                                            GUILayout.ExpandHeight(true));

        }



        void SettingsWindowFcn(int windowID)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_DangIt_266") + nsDangIt.FailureModule.streamMultiplier.ToString());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_DangIt_267"));

            double m;
            var   newMultiplier = GUILayout.TextField(multiplier, GUILayout.Width(90));
            try
            {
                m = float.Parse(newMultiplier);
                multiplier = m.ToString();

            } catch
            { }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_DangIt_268"));

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


            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
 
            if (GUILayout.Button(Localizer.Format("#LOC_DangIt_269"), GUILayout.Width(60)))
            {
                if (multiplier != "")
                    nsDangIt.FailureModule.streamMultiplier += float.Parse(multiplier);
                if (decay != "")
                    nsDangIt.FailureModule.decayPerMinute += float.Parse(decay);
                nsDangIt.FailureModule.lastDecayTime = Planetarium.GetUniversalTime();
                visible = false;
            }
            if (GUILayout.Button(Localizer.Format("#LOC_DangIt_270"), GUILayout.Width(60)))
            {
                visible = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            UnityEngine.GUI.DragWindow();
        }
    }
}
