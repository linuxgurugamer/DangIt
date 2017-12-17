using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace nsDangIt
{
    //	[RequireComponent(typeof(AudioSource))]
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AlarmManager : MonoBehaviour
    {
        public Dictionary<FailureModule, int> loops;
        public static  Dictionary<Part, int> partFailures;

        //AudioSource audio;
        static FXGroup audioSource = null;


        public void Start()
        {
            print("[DangIt] [AlarmManager] Starting...");
            print("[DangIt] [AlarmManager] Setting Volume...");
            audioSource = new FXGroup("DangItAlarm");
            audioSource.audio = Camera.main.gameObject.AddComponent<AudioSource>();
            // audio = new AudioSource();
            audioSource.audio.spatialBlend = 0f; //This disable the game scaling volume with distance from source
            audioSource.audio.volume = 0f;

            Logger.Info("[DangIt] [AlarmManager] Creating Clip");
            audioSource.audio.clip = GameDatabase.Instance.GetAudioClip("DangIt/Sounds/alarm"); //Load alarm sound

            Logger.Info("[DangIt] [AlarmManager] Creating Dictionary");
            loops = new Dictionary<FailureModule, int>(); //Reset counter, so on logic pass we play it
            partFailures = new Dictionary<Part, int>();

            GameEvents.onShowUI.Add(showUI);
            GameEvents.onHideUI.Add(hideUI);
        }

        public static bool visibleUI = true;
        internal void showUI() // triggered on F2
        {
            visibleUI = true;
        }

        internal void hideUI() // triggered on F2
        {
            visibleUI = false;
        }


        internal void OnDestroy()
        {
            GameEvents.onShowUI.Remove(showUI);
            GameEvents.onHideUI.Remove(hideUI);

        }

        public void UpdateSettings()
        {
            print("[DangIt] [AlarmManager] UpdateSettings");
            // float scaledVolume = DangIt.Instance.CurrentSettings.AlarmVolume / 100f;
            //Logger.Info("[DangIt] [AlarmManager] Rescaling Volume (at UpdateSettings queue)..., now at " + scaledVolume);
            // if (audioSource != null)
            //     audioSource.audio.volume = 0; // scaledVolume;
        }

        void AddPartFailure(Part p)
        {
            int i;
            if (partFailures.TryGetValue(p, out i))
            {
                var i1 = partFailures[p];
                partFailures[p] = i1 + 1;
            }
            else
            {
                partFailures.Add(p, 1);
            }
        }
        void RemovePartFailure(Part p)
        {
            int i;
            if (partFailures.TryGetValue(p, out i))
            {
                var i1 = partFailures[p] - 1;
                if (i1 > 0)
                    partFailures[p] = i1;
                else
                    partFailures.Remove(p);                
            }
        }

        public void AddAlarm(FailureModule fm, int number)
        {
            AddPartFailure(fm.part);
            audioSource.audio.volume = DangIt.Instance.CurrentSettings.GetMappedVolume(); //This seems like an OK place for this, because if I put it in the constructor...
                                                                                          // ...you would have to reboot to change it, but I don't want to add lag by adding it to each frame in Update()
            if (number != 0)
            {
                Logger.Info("[DangIt] [AlarmManager] Adding '" + number.ToString() + "' alarms from '" + fm.ToString() + "'");
                loops.Add(fm, number);
            }
            else
            {
                Logger.Info("[DangIt] [AlarmManager] No alarms added: Would have added 0 alarms");
            }
        }

        public void LateUpdate()
        {
            if (audioSource.audio != null)
            {
                if (!audioSource.audio.isPlaying)
                {
                    if (loops.Count > 0)
                    {
                        var element = loops.ElementAt(0);
                        loops.Remove(element.Key);
                        RemovePartFailure(element.Key.part);
                        Logger.Info("[DangIt] [AlarmManager] Playing Clip");
                        float scaledVolume = DangIt.Instance.CurrentSettings.GetMappedVolume();
                        audioSource.audio.volume = scaledVolume;
                        audioSource.audio.Play();
                        if (element.Value != 0 && element.Value != 1)
                        {
                            if (element.Key.vessel == FlightGlobals.ActiveVessel)
                            {
                                loops.Add(element.Key, element.Value - 1); //Only re-add if still has alarms
                                AddPartFailure(element.Key.part);
                            }
                            else
                            {
                                audioSource.audio.volume = 0;
                                element.Key.AlarmsDoneCallback();
                            }
                        }
                        else
                        {
                            audioSource.audio.volume = 0;
                            element.Key.AlarmsDoneCallback();
                        }
                    }
                }
            }
        }

        public void RemoveAllAlarmsForModule(FailureModule fm)
        {
            audioSource.audio.volume = 0;
            Logger.Info("[DangIt] [AlarmManager] Removing alarms...");
            if (this.loops.Keys.Contains(fm))
            {
                fm.AlarmsDoneCallback();
                loops.Remove(fm);
                RemovePartFailure(fm.part);
            }
        }

        public bool HasAlarmsForModule(FailureModule fm)
        {
            if (this.loops.Keys.Contains(fm))
            {
                int i;
                loops.TryGetValue(fm, out i);
                if (i != 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

