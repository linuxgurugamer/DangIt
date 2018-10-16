using System;
using System.Collections;
using UnityEngine;
using KSP.UI.Screens;

using ToolbarControl_NS;

// Disabled since the settings are now using the stock settings pages
#if true
namespace nsDangIt
{
    public partial class DangIt
    {

        //ApplicationLauncherButton appBtn;
        ToolbarControl toolbarControl;

        //SettingsWindow settingsWindow = new SettingsWindow();

        void OnGUI()
        {
            if (toolbarControl != null)
                toolbarControl.UseBlizzy(HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams1>().useBlizzy);

            GUI.skin = HighLogic.Skin;

            //if (settingsWindow.Enabled) settingsWindow.Draw();
        }


        /// <summary>
        /// Coroutine that creates the button in the toolbar. Will wait for the runtime AND the launcher to be ready
        /// before creating the button.
        /// </summary>
        void AddAppButton()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(onAppBtnToggle, onAppBtnToggle,
                ApplicationLauncher.AppScenes.ALWAYS,
                "DangIt_NS",
                "dangItButton",
                "DangIt/Textures/appBtn_38",
                "DangIt/Textures/appBtn_24",
                "Dang It!"
            );
            toolbarControl.UseBlizzy(HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams1>().useBlizzy);

        }

        private void OnGUIAppLauncherDestroyed()
        {
            if (toolbarControl != null)
            {
                toolbarControl.OnDestroy();
                Destroy(toolbarControl);
            }
        }

        // The AppLauncher requires a callback for some events that are not used by this plugin
        void dummyVoid() { return; }

        internal static ippo.Runtime.GUI.StreamMultiplier gui = null;
        void onAppBtnToggle()
        {
            Debug.Log("onAppBtnToggleOn");
            if (gui != null)
            {
                Debug.Log("visible: " + gui.visible.ToString());
                if (!gui.visible)
                {
                    gui.visible = true;
                    gui.multiplier = "";
                    gui.decay = FailureModule.decayPerMinute.ToString();
                }
                else
                {
                    UnityEngine.Object.Destroy(gui);
                    gui = null;
                }
            }
            else
            {
                Debug.Log("Adding new object");
                // Use MapView.MapCamera to get a gameObject
                gui = MapView.MapCamera.gameObject.GetComponent<ippo.Runtime.GUI.StreamMultiplier>();
                if (gui == null)
                {
                    gui = MapView.MapCamera.gameObject.AddComponent<ippo.Runtime.GUI.StreamMultiplier>();
                }
                gui.visible = true;
                gui.multiplier = "";
                gui.decay = FailureModule.decayPerMinute.ToString();
            }
        }
    }
}
#endif