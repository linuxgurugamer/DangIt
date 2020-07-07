using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace nsDangIt
{
    using static nsDangIt.DangIt;

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AlarmManager : MonoBehaviour
    {
        public static Dictionary<FailureModule, int> loops;
        public static Dictionary<Part, int> partFailures;


        static FXGroup audioSource = null;


        public void Start()
        {
            Log.Info("[DangIt] [AlarmManager] Starting...");

            audioSource = new FXGroup("DangItAlarm");
            //AddAudio();

            Log.Info("[DangIt] [AlarmManager] Creating Dictionary");
            if (loops == null)
            {
                loops = new Dictionary<FailureModule, int>(); //Reset counter, so on logic pass we play it
                partFailures = new Dictionary<Part, int>();
            }
            GameEvents.onShowUI.Add(showUI);
            GameEvents.onHideUI.Add(hideUI);
            //DontDestroyOnLoad(this);
        }

        void AddAudio()
        {
            audioSource.audio = Camera.main.gameObject.AddComponent<AudioSource>();

            audioSource.audio.spatialBlend = 0f; //This disable the game scaling volume with distance from source
            audioSource.audio.volume = 0f;

            Log.Info("[DangIt] [AlarmManager] Creating Clip");
            audioSource.audio.clip = GameDatabase.Instance.GetAudioClip("DangIt/Sounds/alarm"); //Load alarm sound

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

        void AddPartFailure(Part p)
        {
            int i;
            Log.Info("AddPartFailure, p: " + p.partInfo.title);
            if (partFailures.TryGetValue(p, out i))
            {
                Log.Info("Part already failed");
                var i1 = partFailures[p];
                partFailures[p] = i1 + 1;
            }
            else
            {
                Log.Info("Adding new failed part");
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
            if (number != 0)
            {
                Log.Info("[DangIt] [AlarmManager] Adding '" + number + "' alarms from '" + fm.ToString());
                loops.Add(fm, number);
            }
            else
            {
                Log.Info("[DangIt] [AlarmManager] No alarms added: Would have added 0 alarms");
            }
        }


        public void LateUpdate()
        {
            if (loops.Count > 0)
            {
                if (audioSource.audio == null)
                {
                    AddAudio();
                    audioSource.audio.volume = DangIt.Instance.CurrentSettings.GetMappedVolume();
                    Log.Info("Audio added, volume: " + audioSource.audio.volume);
                }
                if (audioSource.audio != null)
                {
                    if (!audioSource.audio.isPlaying)
                    {
                        Log.Info("[DangIt] [AlarmManager] Playing Clip, loops.Count: " + loops.Count);

                        // Dictionary key,pair can't be modified, so first remove the first element from the dictionary
                        // Then play the audio clip, finally, if this was NOT the last time for the audio to be played, add it back to the dictionary
                        // otherwise silence the alarm and clear it out

                        var element = loops.ElementAt(0);
                        loops.Remove(element.Key);
                        RemovePartFailure(element.Key.part);

                        float scaledVolume = DangIt.Instance.CurrentSettings.GetMappedVolume();
                        audioSource.audio.volume = scaledVolume;
                        audioSource.audio.Play();

                        if (element.Value != 0)
                        {
                            if (element.Key.vessel == FlightGlobals.ActiveVessel)
                            {
                                loops.Add(element.Key, element.Value - 1); //Only re-add if still has alarms
                                AddPartFailure(element.Key.part);
                            }
                            else
                            {
                                Destroy(audioSource.audio);
                                //audioSource.audio.volume = 0;
                                element.Key.AlarmsDoneCallback();
                            }
                        }
                        else
                        {
                            Destroy(audioSource.audio);
                            //audioSource.audio.volume = 0;
                            element.Key.AlarmsDoneCallback();
                        }
                    }
                }
            }
        }

        public void RemoveAllAlarmsForModule(FailureModule fm)
        {
            audioSource.audio.volume = 0;
            Log.Info("[DangIt] [AlarmManager] Removing alarms...");
            if (loops.Keys.Contains(fm))
            {
                fm.AlarmsDoneCallback();
                loops.Remove(fm);
                RemovePartFailure(fm.part);
            }
        }

        public void RemoveAllAlarms()
        {
            while (loops.Count > 0)
            {
                var element = loops.ElementAt(0);
                loops.Remove(element.Key);
                RemovePartFailure(element.Key.part);
            }
            Destroy(audioSource.audio);
        }
        public bool HasAlarmsForModule(FailureModule fm)
        {
            if (loops.Keys.Contains(fm))
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

