using System;
using System.Collections;
using UnityEngine;
using KSP.UI.Screens;

// Disabled since the settings are now using the stock settings pages
#if true
namespace nsDangIt
{
    public partial class DangIt
    {        
 
        ApplicationLauncherButton appBtn;
        //SettingsWindow settingsWindow = new SettingsWindow();

        void OnGUI()
        {
            GUI.skin = HighLogic.Skin;

            //if (settingsWindow.Enabled) settingsWindow.Draw();
        }


        /// <summary>
        /// Coroutine that creates the button in the toolbar. Will wait for the runtime AND the launcher to be ready
        /// before creating the button.
        /// </summary>
        IEnumerator AddAppButton()
        {
            while (!ApplicationLauncher.Ready || !this.IsReady)
                yield return null;

            try
            {
                if (HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.FLIGHT ||
                    HighLogic.LoadedScene == GameScenes.TRACKSTATION)
                {
                    // Load the icon for the button
                    Texture btnTex = GameDatabase.Instance.GetTexture("DangIt/Textures/appBtn", false);
                    if (btnTex == null)
                        throw new Exception("The button texture wasn't loaded!");
                    
                    appBtn = ApplicationLauncher.Instance.AddModApplication(
                                onAppBtnToggle,
                                onAppBtnToggle,
                                dummyVoid,  // ignore callbacks for more elaborate events
                                dummyVoid,
                                dummyVoid,
                                dummyVoid,
                                ApplicationLauncher.AppScenes.ALWAYS,
                                btnTex);
                }
                
            }
            catch (Exception e)
            {
                this.Log("Error! " + e.Message);
                throw e;
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