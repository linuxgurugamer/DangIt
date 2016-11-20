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

    public class DangItCustomParams1 : GameParameters.CustomParameterNode
    {
       // public static DangItCustomParams instance;
       // public DangItCustomParams()
        //{
       //     instance = this;
       // }

        public override string Title { get { return "General Settings"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Dang It!"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return true; } }

        [GameParameters.CustomParameterUI("Mod Enabled")]
        public bool EnabledForSave = true;      // is enabled for this save file

        [GameParameters.CustomParameterUI("Manual failures")]
        public bool ManualFailures = false;     // initiate failures manually

#if false
        [GameParameters.CustomParameterUI("Show Debug Stats")]
#endif
        public bool DebugStats = false;         // show debug stats of the part in the right-click menu

        [GameParameters.CustomParameterUI("Glow")]
        public bool Glow = true;                // enable the part's glow upon failure

        [GameParameters.CustomParameterUI("Check Experience")]
        public bool RequireExperience = true;   // enable requiring experience levels

        [GameParameters.CustomParameterUI("Messages")]
        public bool Messages = true;            // enable messages and screen posts

        [GameParameters.CustomFloatParameterUI("Max EVA distance", minValue = 1.0f, maxValue = 15.0f, asPercentage = false,
            toolTip = "Repairs can be when when this close to the part")]
        public float MaxDistance = 2f;          // maximum distance for EVA activities

        [GameParameters.CustomIntParameterUI("Alarm Volume", minValue = 0, maxValue = 100)]
        public int AlarmVolume = 100;          // volume of the alarm (1-100) 

        [GameParameters.CustomIntParameterUI("Beep # for Low Priorities (-1=>Inf)", minValue = -1, maxValue = 5)]
        public int Pri_Low_SoundLoops = 0;     // number of times to beep

        [GameParameters.CustomIntParameterUI("Beep # for Medium Priorities (-1=>Inf)", minValue = -1, maxValue = 5)]
        public int Pri_Medium_SoundLoops = 2;  // number of times to beep

        [GameParameters.CustomIntParameterUI("Beep # for High Priorities (-1=>Inf)", minValue = -1, maxValue = 5)]
        public int Pri_High_SoundLoops = -1;   // number of times to beep

        [GameParameters.CustomFloatParameterUI("MTBF Multiplier", minValue = 0.25f, maxValue = 4.0f, logBase = 2, stepCount = 100, displayFormat = "0.00", asPercentage = false,
            toolTip ="Adjusts the MTBF.  Decrease this to make it harder, increase to lessen the difficulty")]
        public float MTBF_Multiplier = 1f;          // maximum distance for EVA activities

        [GameParameters.CustomFloatParameterUI("Lifetime Multiplier", minValue = 0.25f, maxValue = 4.0f, logBase = 2, stepCount = 100, displayFormat = "0.00", asPercentage = false,
            toolTip ="Adjusts the Lifetime.  Decrease this to make it harder, increase to lessen the difficulty")]
        public float Lifetime_Multiplier = 1f;          // maximum distance for EVA activities
            

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

                    MTBF_Multiplier = 2f;
                    Lifetime_Multiplier = 2f;
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
                    DebugStats = false;         // show debug stats of the part in the right-click menu

                    MTBF_Multiplier = 1f;
                    Lifetime_Multiplier = 1f;

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
                    DebugStats = false;         // show debug stats of the part in the right-click menu

                    MTBF_Multiplier = 0.75f;
                    Lifetime_Multiplier = 1f;

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
                    DebugStats = false;         // show debug stats of the part in the right-click menu

                    MTBF_Multiplier = 0.5f;
                    Lifetime_Multiplier = 0.75f;

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
            //if (HighLogic.LoadedScene != GameScenes.SPACECENTER)
            //    return false;
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




    public class DangItCustomParams2 : GameParameters.CustomParameterNode
    {

        public override string Title { get { return "Allow Failures on:"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Dang It!"; } }
        public override int SectionOrder { get { return 2; } }
        public override bool HasPresets { get { return true; } }


        [GameParameters.CustomParameterUI("Alternator")]
        public bool AllowAlternatorFailures = true;

        [GameParameters.CustomParameterUI("Batteries")]
        public bool AllowBatteryFailures = true;

        [GameParameters.CustomParameterUI("Control Surfaces")]
        public bool AllowControlSurfaceFailures = true;

        [GameParameters.CustomParameterUI("Deployable Antennas")]
        public bool AllowDeployableAntennaFailures = true;

        [GameParameters.CustomParameterUI("Engines")]
        public bool AllowEngineFailures = true;

        [GameParameters.CustomParameterUI("Engine Gimbals")]
        public bool AllowGimbalFailures = true;

        [GameParameters.CustomParameterUI("Lights")]
        public bool AllowLightFailures = true;

        [GameParameters.CustomParameterUI("RCS")]
        public bool AllowRCSFailures = true;

        [GameParameters.CustomParameterUI("Reaction Wheels")]
        public bool AllowReactionWheelFailures = true;

        [GameParameters.CustomParameterUI("Tanks")]
        public bool AllowTankFailures = true;

        [GameParameters.CustomParameterUI("Coolant")]
        public bool AllowCoolantFailures = true;


        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            switch (preset)
            {
                case GameParameters.Preset.Easy:

                    AllowAlternatorFailures = true;
                    AllowBatteryFailures = true;
                    AllowControlSurfaceFailures = true;
                    AllowDeployableAntennaFailures = true;
                    AllowEngineFailures = true;
                    AllowGimbalFailures = true;
                    AllowLightFailures = true;
                    AllowRCSFailures = true;
                    AllowReactionWheelFailures = true;
                    AllowTankFailures = true;

                    break;

                case GameParameters.Preset.Normal:

                    AllowAlternatorFailures = true;
                    AllowBatteryFailures = true;
                    AllowControlSurfaceFailures = true;
                    AllowDeployableAntennaFailures = true;
                    AllowEngineFailures = true;
                    AllowGimbalFailures = true;
                    AllowLightFailures = true;
                    AllowRCSFailures = true;
                    AllowReactionWheelFailures = true;
                    AllowTankFailures = true;

                    break;

                case GameParameters.Preset.Moderate:

                    AllowAlternatorFailures = true;
                    AllowBatteryFailures = true;
                    AllowControlSurfaceFailures = true;
                    AllowDeployableAntennaFailures = true;
                    AllowEngineFailures = true;
                    AllowGimbalFailures = true;
                    AllowLightFailures = true;
                    AllowRCSFailures = true;
                    AllowReactionWheelFailures = true;
                    AllowTankFailures = true;

                    break;

                case GameParameters.Preset.Hard:

                    AllowAlternatorFailures = true;
                    AllowBatteryFailures = true;
                    AllowControlSurfaceFailures = true;
                    AllowDeployableAntennaFailures = true;
                    AllowEngineFailures = true;
                    AllowGimbalFailures = true;
                    AllowLightFailures = true;
                    AllowRCSFailures = true;
                    AllowReactionWheelFailures = true;
                    AllowTankFailures = true;

                    break;
            }
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {            
                return true;
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            //if (HighLogic.LoadedScene != GameScenes.SPACECENTER)
            //    return false;
            
            return true;
            //            return true; //otherwise return true
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }


    public class DangItCustomParams3 : GameParameters.CustomParameterNode
    {
        // public static DangItCustomParams instance;
        // public DangItCustomParams()
        //{
        //     instance = this;
        // }

        public override string Title { get { return "Additional Allow Failures on:"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Dang It!"; } }
        public override int SectionOrder { get { return 3; } }
        public override bool HasPresets { get { return true; } }

        [GameParameters.CustomParameterUI("Wheel Motor")]
        public bool AllowWheelMotorFailures = true;

        [GameParameters.CustomParameterUI("Wheel Tire")]
        public bool AllowWheelTireFailures = true;

        [GameParameters.CustomParameterUI("Motor")]
        public bool AllowAnimateFailures = false;

        [GameParameters.CustomParameterUI("Generator")]
        public bool AllowGeneratorFailures = false;

        [GameParameters.CustomParameterUI("Parachute")]
        public bool AllowParachuteFailures = false;

        [GameParameters.CustomParameterUI("Parachute (1 on vessel)")]
        public bool Allow1ParachuteFailures = false;

        [GameParameters.CustomParameterUI("Small Tanks")]
        public bool AllowSmallTankFailures = false;

        [GameParameters.CustomParameterUI("Solar Panels")]
        public bool AllowSolarPanelFailures = false;

        [GameParameters.CustomParameterUI("SRBs")]
        public bool AllowSRBFailures = false;



        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            switch (preset)
            {
                case GameParameters.Preset.Easy:

                    AllowWheelMotorFailures = true;
                    AllowWheelTireFailures = true;

                    AllowAnimateFailures = false;
                    AllowGeneratorFailures = false;
                    AllowParachuteFailures = false;
                    Allow1ParachuteFailures = false;
                    AllowSmallTankFailures = false;
                    AllowSolarPanelFailures = false;
                    AllowSRBFailures = false;

                    break;

                case GameParameters.Preset.Normal:

                    AllowWheelMotorFailures = true;
                    AllowWheelTireFailures = true;

                    AllowAnimateFailures = false;
                    AllowGeneratorFailures = false;
                    AllowParachuteFailures = true;
                    Allow1ParachuteFailures = false;

                    AllowSmallTankFailures = true;
                    AllowSolarPanelFailures = true;
                    AllowSRBFailures = false;

                    break;

                case GameParameters.Preset.Moderate:

                    AllowWheelMotorFailures = true;
                    AllowWheelTireFailures = true;

                    AllowAnimateFailures = true;
                    AllowGeneratorFailures = true;
                    AllowParachuteFailures = true;
                    Allow1ParachuteFailures = false;

                    AllowSmallTankFailures = true;
                    AllowSolarPanelFailures = true;
                    AllowSRBFailures = false;

                    break;

                case GameParameters.Preset.Hard:

                    AllowWheelMotorFailures = true;
                    AllowWheelTireFailures = true;

                    AllowAnimateFailures = true;
                    AllowGeneratorFailures = true;
                    AllowParachuteFailures = true;
                    Allow1ParachuteFailures = true;

                    AllowSmallTankFailures = true;
                    AllowSolarPanelFailures = true;
                    AllowSRBFailures = true;

                    break;
            }
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {            
            return true;
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            //if (HighLogic.LoadedScene != GameScenes.SPACECENTER)
            //    return false;
            if (member.Name == "AllowParachuteFailures")
            {
                if (!AllowParachuteFailures)
                    Allow1ParachuteFailures = false;
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
