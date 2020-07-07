using System;
using System.Collections;
using UnityEngine;
using KSP.UI.Screens;

using ToolbarControl_NS;
using JetBrains.Annotations;

// Disabled since the settings are now using the stock settings pages
#if true
namespace nsDangIt
{
    using static nsDangIt.DangIt;

    public partial class DangIt
    {

        ToolbarControl toolbarControl;


        internal const string MODID = "DangIt_NS";
        internal const string MODNAME = "Dang It!";

        void AddAppButton()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(onAppBtnToggle, onAppBtnToggle,
                ApplicationLauncher.AppScenes.SPACECENTER |
                ApplicationLauncher.AppScenes.FLIGHT,
                MODID,
                "dangItButton",
                "DangIt/PluginData/Textures/appBtn_38",
                "DangIt/PluginData/Textures/appBtn_24",
                MODNAME
            );
        }

        protected void OnGUIAppLauncherDestroyed()
        {
            if (toolbarControl != null)
            {
                toolbarControl.OnDestroy();
                Destroy(toolbarControl);
            }
        }


        internal static ippo.Runtime.GUI.StreamMultiplier gui = null;
        void onAppBtnToggle()
        {
            Log.Info("onAppBtnToggleOn");
            if (HighLogic.LoadedSceneIsFlight)
            {
                ippo.Runtime.GUI.FailureStatusWindow.instance.isVisible = !ippo.Runtime.GUI.FailureStatusWindow.instance.isVisible;
            }
            else
            {
                if (gui != null)
                {
                    Log.Info("visible: " + gui.visible.ToString());
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
                    Log.Info("Adding new object");
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
}
#endif