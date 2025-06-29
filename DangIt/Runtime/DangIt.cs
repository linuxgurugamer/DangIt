using KSP.Localization;
using KSP.IO;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.IO;
using KSP_Log;
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
        #region NO_LOCALIZATION
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
                        _leakBlackList.Add(DangIt.Instance.CurrentSettings.GetSparesResource());

                        Log.Info("[DangIt]: An exception occurred while loading the resource blacklist and a default one has been created. " + e.Message);
                    }
                }

                return _leakBlackList;
            }
            set
            {
                _leakBlackList = value;
            }
        }
        #endregion

        internal static List<string> _leakBlackList = null;


        public AlarmManager alarmManager;


        /// <summary>
        /// Return the current running instance.
        /// </summary>
        public static DangIt Instance { get; private set; }
        public Settings CurrentSettings = new Settings();

        /// <summary>
        /// Returns true if the instance is initialized and ready to work.
        /// </summary>
        public bool IsReady { get; private set; }
        public static KSP_Log.Log Log; // Initialized in Registertoolbar



        void Start()
        {
            Instance = this;

            GameEvents.OnGameSettingsApplied.Add(ReloadSettings);
            GameEvents.onGameStatePostLoad.Add(ReloadSettings);
            Log.Info("[DangIt]: Start.");
            AddAppButton();

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
            AlarmManager alarmManager = FindObjectOfType<AlarmManager>();
            DangIt.Instance.StartPartInfoCacheReload();
        }

        public void OnDestroy()
        {
            Log.Info("Destroying instance.");

            if (toolbarControl != null)
            {
                toolbarControl.OnDestroy();
                Destroy(toolbarControl);
            }

            GameEvents.OnGameSettingsApplied.Remove(ReloadSettings);
            GameEvents.onGameStatePostLoad.Remove(ReloadSettings);
        }


        public void StartPartInfoCacheReload()
        {
            Log.Info("Starting refresh of Part Info cache");

            StartCoroutine(RefreshPartInfo());
        }

        private IEnumerator RefreshPartInfo()
        {
            if (CurrentSettings == null || PartLoader.LoadedPartsList == null)
                yield break;
            ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_DangIt_189"), 1, ScreenMessageStyle.UPPER_CENTER);
            this.IsReady = false;
            yield return null;
            float lastTime = Time.realtimeSinceStartup;

            var apList = PartLoader.LoadedPartsList.Where(ap => ap.partPrefab.Modules != null);
            int totcnt = 1;
            if (apList == null)
            {
                Log.Info("apList is null");
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
                        ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_DangIt_190") + intPercent + "%", 1, ScreenMessageStyle.UPPER_CENTER);
                    }

                    AvailablePart.ModuleInfo target = null;
                    foreach (var mi in ap.moduleInfos)
                    {
                        if (mi.moduleName == Localizer.Format("#LOC_DangIt_191"))
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
                            newModuleInfo.moduleName = Localizer.Format("#LOC_DangIt_191");
                            newModuleInfo.info = reliabilityModules.First().GetInfo();
                            ap.moduleInfos.Add(newModuleInfo);
                        }
                    }
                }
                Log.Info("Refresh Finished");
                this.IsReady = true;
            }
            catch (Exception e)
            {
                Log.Info("ERROR [" + e.GetType().ToString() + "]: " + e.Message + "\n" + e.StackTrace);
            }
            ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_DangIt_192"), 2, ScreenMessageStyle.UPPER_CENTER);
        }
    }
}
