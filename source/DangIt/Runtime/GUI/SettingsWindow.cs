using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ippo
{
#if false
    class SettingsWindow
    {
        private Rect settingsRect = new Rect(20, 20, 300, 150);
        string evaDistanceString = string.Empty;    // temp variable to edit the Max Distance in the GUI
        string SoundLoopsString_Low = string.Empty;    // temp variable to edit the Sound Loops in the GUI
        string SoundLoopsString_Medium = string.Empty;    // temp variable to edit the Sound Loops in the GUI
        string SoundLoopsString_High = string.Empty;    // temp variable to edit the Sound Loops in the GUI
        string SoundVolumeString = string.Empty;    // temp variable to edit the Sound Volume
        private bool enabled;

        bool lastEnabledValue;
        bool waitingForConfirm = false;

        DangIt.Settings newSettings;



        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                if (value) // Copy the current settings when the window is enabled
                {
                    ReInitilize();
                }
            }
        }

#if false
        private void ReInitilize()
        { //Set our string data mirrors at start, and when we change settings
            this.newSettings = DangIt.Instance.CurrentSettings.ShallowClone();
            this.evaDistanceString = newSettings.MaxDistance.ToString();
            this.SoundLoopsString_Low = newSettings.Pri_Low_SoundLoops.ToString();
            this.SoundLoopsString_Medium = newSettings.Pri_Medium_SoundLoops.ToString();
            this.SoundLoopsString_High = newSettings.Pri_High_SoundLoops.ToString();
            this.SoundVolumeString = newSettings.AlarmVolume.ToString();
            this.lastEnabledValue = newSettings.EnabledForSave;
        }
#endif
        public void Draw()
        {
            // The settings are only available in the space center
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                settingsRect = GUILayout.Window("DangItSettings".GetHashCode(),
                                                settingsRect,
                                                SettingsWindowFcn,
                                                "Dang It! Settings",
                                                GUILayout.ExpandWidth(true),
                                                GUILayout.ExpandHeight(true));
            }
        }



        void SettingsWindowFcn(int windowID)
        {
            if (waitingForConfirm)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("WARNING! Changing the state of DangIt! while ships are in flight is not supported.");
                GUILayout.Label("There is no gaurentee that ships will remain in a stable state after toggle, ESPECIALLY if they currently have failed parts.");
                GUILayout.Label("It is reccomended that this option is only changed immediatley after the start of a game AND while no ships are in flight");
                GUILayout.Label("You currently have " + (FlightGlobals.Vessels.Count - 1).ToString() + " vessels in flight. Are you sure you want to proceed?");
                GUILayout.Space(50);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Yes"))
                {
                    lastEnabledValue = newSettings.EnabledForSave;
                    waitingForConfirm = false;
                    DangIt.Instance.CurrentSettings = this.newSettings;
                }
                if (GUILayout.Button("No"))
                {
                    newSettings.EnabledForSave = lastEnabledValue;
                    waitingForConfirm = false;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            else
            {

                // Display the toggles and controls to read the new settings
                newSettings.EnabledForSave = GUILayout.Toggle(newSettings.EnabledForSave, "Enable");
                if (newSettings.EnabledForSave != this.lastEnabledValue)
                {
                    waitingForConfirm = true;
                }

                if (newSettings.EnabledForSave)
                {
                    newSettings.ManualFailures = GUILayout.Toggle(newSettings.ManualFailures, "Manual failures");
                    newSettings.DebugStats = GUILayout.Toggle(newSettings.DebugStats, "Show Debug Stats");
                    newSettings.Glow = GUILayout.Toggle(newSettings.Glow, "Glow");
                    newSettings.RequireExperience = GUILayout.Toggle(newSettings.RequireExperience, "Check Experience");
                    newSettings.Messages = GUILayout.Toggle(newSettings.Messages, "Messages");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Max EVA distance: ");
                    evaDistanceString = GUILayout.TextField(evaDistanceString);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Alarm Volume (0-100): ");
                    SoundVolumeString = GUILayout.TextField(SoundVolumeString);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("# Times to beep for Priorities (-1=>Inf) of Failures");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("LOW: ");
                    SoundLoopsString_Low = GUILayout.TextField(SoundLoopsString_Low);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("MEDIUM: ");
                    SoundLoopsString_Medium = GUILayout.TextField(SoundLoopsString_Medium);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("HIGH: ");
                    SoundLoopsString_High = GUILayout.TextField(SoundLoopsString_High);
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.Label("DangIt! is disabled");
                }

                // Creates the button and returns true when it is pressed
                if (GUILayout.Button("Apply"))
                {
                    // Parse the strings
                    this.newSettings.MaxDistance = DangIt.Parse<float>(evaDistanceString, defaultTo: 2f);
                    this.newSettings.Pri_Low_SoundLoops = DangIt.Parse<int>(SoundLoopsString_Low, defaultTo: 0);
                    this.newSettings.Pri_Medium_SoundLoops = DangIt.Parse<int>(SoundLoopsString_Medium, defaultTo: 2);
                    this.newSettings.Pri_High_SoundLoops = DangIt.Parse<int>(SoundLoopsString_High, defaultTo: -1);
                    int av = DangIt.Parse<int>(SoundVolumeString, defaultTo: 100);
                    //av = (av < 0) ? 0 : (av > 100) ? 100 : av;  //This clamps it between 0 and 100 (or not)
                    if (av < 1)
                    {
                        av = 1;
                    }
                    else if (av > 100)
                    {
                        av = 100;
                    }
                    this.newSettings.AlarmVolume = av;
                    DangIt.Instance.CurrentSettings = this.newSettings;

                    ReInitilize(); //Reinit string data in case you entered a invalid value (or went over cap in volume)
                }
            }

            // This call allows the user to drag the window around the screen
            GUI.DragWindow();
        }
    }
#endif

    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    class DangItCheckEnabledChange : MonoBehaviour
    {
        public static DangItCheckEnabledChange Instance;

        public enum Answer
        {
            inActive,
            notAnswered,
            Yes,
            No
        };
        public Answer answer = Answer.inActive;

        private Rect settingsRect = new Rect(20, 20, 300, 150);

        public DangItCheckEnabledChange()
        {
        }
        void Start()
        {
            Instance = this;
        }

        void OnGUI()
        {
            if (answer == Answer.inActive)
                return;

            Draw();
        }
        public void Draw()
        {
            // The settings are only available in the space center
            GUI.skin = HighLogic.Skin;
            settingsRect = GUILayout.Window("DangItSettings".GetHashCode(),
                                            settingsRect,
                                            SettingsWindowFcn,
                                            "Dang It! Settings",
                                            GUILayout.ExpandWidth(true),
                                            GUILayout.ExpandHeight(true));
        }

        void SettingsWindowFcn(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("WARNING! Changing the state of DangIt! while ships are in flight is not supported.");
            GUILayout.Label("There is no gaurentee that ships will remain in a stable state after toggle, ESPECIALLY if they currently have failed parts.");
            GUILayout.Label("It is reccomended that this option is only changed immediatley after the start of a game AND while no ships are in flight");
            GUILayout.Label("You currently have " + (FlightGlobals.Vessels.Count - 1).ToString() + " vessels in flight. Are you sure you want to proceed?");
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Yes"))
            {
                answer = Answer.Yes;
            }
            if (GUILayout.Button("No"))
            {
                answer = Answer.No;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            // This call allows the user to drag the window around the screen
            GUI.DragWindow();
        }

    }

    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    public class DangItCustomParams : GameParameters.CustomParameterNode
    {
       // public static DangItCustomParams instance;
       // public DangItCustomParams()
        //{
       //     instance = this;
       // }

        public override string Title { get { return ""; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Dang It!"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return true; } }

        [GameParameters.CustomParameterUI("Mod Enabled")]
        public bool EnabledForSave = true;      // is enabled for this save file

        [GameParameters.CustomParameterUI("Manual failures")]
        public bool ManualFailures = false;     // initiate failures manually

        [GameParameters.CustomParameterUI("Show Debug Stats")]
        public bool DebugStats = false;         // show debug stats of the part in the right-click menu

        [GameParameters.CustomParameterUI("Glow")]
        public bool Glow = true;                // enable the part's glow upon failure

        [GameParameters.CustomParameterUI("Check Experience")]
        public bool RequireExperience = true;   // enable requiring experience levels

        [GameParameters.CustomParameterUI("Messages")]
        public bool Messages = true;            // enable messages and screen posts

        [GameParameters.CustomFloatParameterUI("Max EVA distance", minValue = 1.0f, maxValue = 15.0f, asPercentage = false)]
        public float MaxDistance = 2f;          // maximum distance for EVA activities

        [GameParameters.CustomIntParameterUI("Alarm Volume", minValue = 1, maxValue = 100)]
        public int AlarmVolume = 100;          // volume of the alarm (1-100) 

        [GameParameters.CustomIntParameterUI("Beep # for Low Priorities (-1=>Inf)", minValue = -1, maxValue = 5)]
        public int Pri_Low_SoundLoops = 0;     // number of times to beep

        [GameParameters.CustomIntParameterUI("Beep # for Medium Priorities (-1=>Inf)", minValue = -1, maxValue = 5)]
        public int Pri_Medium_SoundLoops = 2;  // number of times to beep

        [GameParameters.CustomIntParameterUI("Beep # for High Priorities (-1=>Inf)", minValue = -1, maxValue = 5)]
        public int Pri_High_SoundLoops = -1;   // number of times to beep

        bool oldEnabled = true;

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            switch (preset)
            {
                case GameParameters.Preset.Easy:

                    EnabledForSave = true;      // is enabled for this save file
                    ManualFailures = false;     // initiate failures manually
                    MaxDistance = 5f;          // maximum distance for EVA activities
                    Messages = true;            // enable messages and screen posts
                    Glow = true;                // enable the part's glow upon failure
                    RequireExperience = false;   // enable requiring experience levels
                    Pri_Low_SoundLoops = 0;     // number of times to beep
                    Pri_Medium_SoundLoops = 2;  // number of times to beep
                    Pri_High_SoundLoops = -1;   // number of times to beep
                    AlarmVolume = 100;          // volume of the alarm (1-100)   
                    DebugStats = false;			// show debug stats of the part in the right-click menu

                    break;

                case GameParameters.Preset.Normal:

                    EnabledForSave = true;      // is enabled for this save file
                    ManualFailures = false;     // initiate failures manually
                    MaxDistance = 2f;          // maximum distance for EVA activities
                    Messages = true;            // enable messages and screen posts
                    Glow = true;                // enable the part's glow upon failure
                    RequireExperience = true;   // enable requiring experience levels
                    Pri_Low_SoundLoops = 0;     // number of times to beep
                    Pri_Medium_SoundLoops = 2;  // number of times to beep
                    Pri_High_SoundLoops = -1;   // number of times to beep
                    AlarmVolume = 100;          // volume of the alarm (1-100)   
                    DebugStats = false;			// show debug stats of the part in the right-click menu

                    break;

                case GameParameters.Preset.Moderate:

                    EnabledForSave = true;      // is enabled for this save file
                    ManualFailures = false;     // initiate failures manually
                    MaxDistance = 1f;          // maximum distance for EVA activities
                    Messages = true;            // enable messages and screen posts
                    Glow = true;                // enable the part's glow upon failure
                    RequireExperience = true;   // enable requiring experience levels
                    Pri_Low_SoundLoops = 0;     // number of times to beep
                    Pri_Medium_SoundLoops = 2;  // number of times to beep
                    Pri_High_SoundLoops = -1;   // number of times to beep
                    AlarmVolume = 100;          // volume of the alarm (1-100)   
                    DebugStats = false;			// show debug stats of the part in the right-click menu

                    break;

                case GameParameters.Preset.Hard:

                    EnabledForSave = true;      // is enabled for this save file
                    ManualFailures = false;     // initiate failures manually
                    MaxDistance = 0.5f;          // maximum distance for EVA activities
                    Messages = false;            // enable messages and screen posts
                    Glow = true;                // enable the part's glow upon failure
                    RequireExperience = true;   // enable requiring experience levels
                    Pri_Low_SoundLoops = 0;     // number of times to beep
                    Pri_Medium_SoundLoops = 2;  // number of times to beep
                    Pri_High_SoundLoops = -1;   // number of times to beep
                    AlarmVolume = 100;          // volume of the alarm (1-100)   
                    DebugStats = false;			// show debug stats of the part in the right-click menu

                    break;
            }
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            if (member.Name == "EnabledForSave") //This Field must always be enabled.
                return true;

            return oldEnabled; //otherwise return true
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            if (HighLogic.LoadedScene != GameScenes.SPACECENTER)
                return false;
            if (oldEnabled != EnabledForSave)
            {
               
                switch (DangItCheckEnabledChange.Instance.answer)
                {
                    case DangItCheckEnabledChange.Answer.inActive:
                        DangItCheckEnabledChange.Instance.answer = DangItCheckEnabledChange.Answer.notAnswered;
                        return false;

                    case DangItCheckEnabledChange.Answer.Yes:
                        oldEnabled = EnabledForSave;
                        DangItCheckEnabledChange.Instance.answer = DangItCheckEnabledChange.Answer.inActive;
                        break;
                    case DangItCheckEnabledChange.Answer.No:
                        EnabledForSave = oldEnabled;
                        DangItCheckEnabledChange.Instance.answer = DangItCheckEnabledChange.Answer.inActive;
                        break;
                    case DangItCheckEnabledChange.Answer.notAnswered:
                        return false;
                }
          
            }
            return true;
            //            return true; //otherwise return true
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }
}
