using KSP.IO;
using System;
using System.Collections.Generic;
using System.Collections;
//using System.Xml.Serialization;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.IO;
using KSP.UI.Screens;

namespace nsDangIt
{
    /// <summary>
    /// Mod's runtime controller and manager.
    /// Provides general utilities, definitions, and handles the user's settings.
    /// </summary>
    public partial class DangIt : ScenarioModule
    {
        /// <summary>
        /// List of resources that must be ignored by tank leaks.
        /// </summary>
        public static List<string> LeakBlackList
        {
            get
            {
                if (_leakBlackList == null) // Load the file on the first call
                {
                    _leakBlackList = new List<string>();

                    Assembly execAssembly = Assembly.GetExecutingAssembly();
                    string _pluginDirectory = Path.GetDirectoryName(execAssembly.Location);
                    //dll's path + filename for the config file
                    string blacklistFilePath = Path.Combine(_pluginDirectory, "../PluginData/BlackList.cfg");

                    ConfigNode blacklistFile = ConfigNode.Load(blacklistFilePath);
                    try
                    {
                        ConfigNode blackListNode = blacklistFile.GetNode("BLACKLIST");
                        foreach (string item in blackListNode.GetValues("ignore"))
                            _leakBlackList.Add(item);
                    }
                    catch (Exception e)
                    {
                        _leakBlackList.Add("ElectricCharge");
                        // _leakBlackList.Add("SolidFuel");
                        _leakBlackList.Add("SpareParts");

                        Logger.Info("[DangIt]: An exception occurred while loading the resource blacklist and a default one has been created. " + e.Message);
                    }
                }

                return _leakBlackList;
            }
            set
            {
                _leakBlackList = value;
            }
        }
        internal static List<string> _leakBlackList = null;


        // private DangIt.Settings currentSettings;
        public AlarmManager alarmManager;

        // public DangItCustomParams CurrentSettings
        //{
        //   get { return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams>(); }
        //}
        /// <summary>
        /// General settings about notifications and gameplay elements.
        /// </summary>


#if false
        {
           get { return value; }

            set
            {
                //this.Log("Applying new settings:\n" + value.ToNode().ToString());
                currentSettings = value;
				DangIt.Instance.StartPartInfoCacheReload ();
				if (FindObjectOfType<AlarmManager> () != null) {
					FindObjectOfType<AlarmManager> ().UpdateSettings ();
				}
            }

        }
#endif

        /// <summary>
        /// Return the current running instance.
        /// </summary>
        public static DangIt Instance { get; private set; }
        public Settings CurrentSettings = new Settings();

        /// <summary>
        /// Returns true if the instance is initialized and ready to work.
        /// </summary>
        public bool IsReady { get; private set; }

        public DangIt()
        {
            Logger.Info("[DangIt]: Instantiating runtime.");

            // Now the instance is built and can be exposed, but it is not yet ready until after OnLoad
            Instance = this;
            //this.IsReady = false;

            // Add the button to the stock toolbar
            this.StartCoroutine("AddAppButton");

        }

        void Start()
        {
            GameEvents.OnGameSettingsApplied.Add(ReloadSettings);
            GameEvents.onGameStatePostLoad.Add(ReloadSettings);
            Logger.Info("[DangIt]: Start.");

            ReloadSettings();
        }

        /// <summary>
        /// Reload settings if necessary.
        /// </summary>
        void ReloadSettings(ConfigNode node)
        {
            ReloadSettings();
        }
        void ReloadSettings()
        {
            //  this.IsReady = false;
            //DangIt.Instance.StartPartInfoCacheReload();
            if (FindObjectOfType<AlarmManager>() != null)
            {
                FindObjectOfType<AlarmManager>().UpdateSettings();
            }
            DangIt.Instance.StartPartInfoCacheReload();

            //  this.IsReady = true;
        }

#if false
        /// <summary>
        /// Load the saved settings
        /// </summary>
        public override void OnLoad(ConfigNode node)
        {
            print("DangIt.OnLoad");

            if (node.HasNode("SETTINGS"))
                this.currentSettings = new Settings(node.GetNode("SETTINGS"));
            else
            {
                this.CurrentSettings = new Settings();
                this.Log("WARNING: No settings node to load, creating default one");
            }

            this.IsReady = true;
        }


        public override void OnSave(ConfigNode node)
        {
            this.Log("Saving settings...");

            if (node.HasNode("SETTINGS"))
            {
                node.SetNode("SETTINGS", CurrentSettings.ToNode());
            }
            else
            {
                node.AddNode(CurrentSettings.ToNode());
            }
        }

#endif

        public void OnDestroy()
        {
            this.Log("Destroying instance.");
#if false
            // Remove the button from the toolbar
            if (appBtn != null)
                ApplicationLauncher.Instance.RemoveModApplication(this.appBtn);
#endif
            GameEvents.OnGameSettingsApplied.Remove(ReloadSettings);
            GameEvents.onGameStatePostLoad.Remove(ReloadSettings);
        }


        private void Log(string msg)
        {
            Logger.Info(msg);
        }

        public void StartPartInfoCacheReload()
        {
            Log("Starting refresh of Part Info cache");

            StartCoroutine(RefreshPartInfo());
        }

        private IEnumerator RefreshPartInfo()
        {
            if (CurrentSettings == null || PartLoader.LoadedPartsList == null)
                yield break;
            ScreenMessages.PostScreenMessage("Database Part Info reloading started", 1, ScreenMessageStyle.UPPER_CENTER);
            this.IsReady = false;
            yield return null;
            float lastTime = Time.realtimeSinceStartup;

            var apList = PartLoader.LoadedPartsList.Where(ap => ap.partPrefab.Modules != null);
            int totcnt = 1;
            if (apList == null)
            {
                Logger.Info("apList is null");
            }
            else
            {
                totcnt = apList.Count();
            }
            int cnt = 0;
            try
            {
                foreach (var ap in PartLoader.LoadedPartsList.Where(ap => ap.partPrefab.Modules != null))
                {
                    cnt++;
                    if (Time.realtimeSinceStartup - lastTime > 2)
                    {
                        lastTime = Time.realtimeSinceStartup;
                        int intPercent = Mathf.CeilToInt(((float)cnt / (float)totcnt * 100f));
                        ScreenMessages.PostScreenMessage("Database reloading " + intPercent + "%", 1, ScreenMessageStyle.UPPER_CENTER);
                    }

                    AvailablePart.ModuleInfo target = null;
                    foreach (var mi in ap.moduleInfos)
                    {
                        if (mi.moduleName == "Reliability Info")
                        {
                            target = mi;
                        }
                    }

                    if (target != null & !this.CurrentSettings.EnabledForSave)
                    {
                        ap.moduleInfos.Remove(target);
                    }

                    if (this.CurrentSettings.EnabledForSave)
                    {
                        if (target != null)
                            ap.moduleInfos.Remove(target);

                        IEnumerable<ModuleReliabilityInfo> reliabilityModules = ap.partPrefab.Modules.OfType<ModuleReliabilityInfo>();
                        if (reliabilityModules.Count() != 0)
                        {
                            AvailablePart.ModuleInfo newModuleInfo = new AvailablePart.ModuleInfo();
                            newModuleInfo.moduleName = "Reliability Info";
                            newModuleInfo.info = reliabilityModules.First().GetInfo();
                            ap.moduleInfos.Add(newModuleInfo);
                        }
                    }
                }
                Log("Refresh Finished");
                this.IsReady = true;
            }
            catch (Exception e)
            {
                this.Log("ERROR [" + e.GetType().ToString() + "]: " + e.Message + "\n" + e.StackTrace);
            }
            ScreenMessages.PostScreenMessage("Database Part Info reloading finished", 2, ScreenMessageStyle.UPPER_CENTER);
        }
    }
}