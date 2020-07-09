using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace nsDangIt
{
    using static nsDangIt.DangIt;

    public class FailedPart
    {
        public Part part;
        public string PartId { get { return part.vessel.id.ToString() + ":" + part.persistentId.ToString(); } }
        public override int GetHashCode() { return PartId.GetHashCode(); }
        public override bool Equals(object obj) { return Equals(obj as FailedPart); }
        public bool Equals(FailedPart obj) { return obj != null && obj.PartId == this.PartId; }

        public FailedPart(Part p) { this.part = p; }
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AlarmManager : MonoBehaviour
    {

        public static Dictionary<FailureModule, int> loops;
        public static Dictionary<FailedPart, int> failedParts;


        static FXGroup audioSource = null;


        public void Start()
        {
            Log.Info("[DangIt] [AlarmManager] Starting...");

            audioSource = new FXGroup("DangItAlarm");

            Log.Info("[DangIt] [AlarmManager] Creating Dictionary");
            if (loops == null)
            {
                loops = new Dictionary<FailureModule, int>(); //Reset counter, so on logic pass we play it
                failedParts = new Dictionary<FailedPart, int>();
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
            if (failedParts.TryGetValue(new FailedPart(p), out i))
            {
                Log.Info("Part already failed");
                var i1 = failedParts[new FailedPart(p)];
                failedParts[new FailedPart(p)] = i1 + 1;
            }
            else
            {
                Log.Info("Adding new failed part");
                failedParts[new FailedPart(p)] = 1;
            }
        }

        static internal void RemovePartFailure(Part p)
        {
            FailedPart fp = new FailedPart(p);
            if (failedParts.ContainsKey(fp))
            {
                var i1 = failedParts[fp] - 1;
                if (i1 > 0)
                    failedParts[fp] = i1;
                else
                    failedParts.Remove(fp);
            }
            else
                Log.Error("RemovePartFailure, failedParts does NOT contain key: " + p.vessel.id.ToString() + ":" + p.persistentId.ToString());
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
                        //RemovePartFailure(element.Key.part);

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
                                element.Key.AlarmsDoneCallback();
                            }
                        }
                        else
                        {
                            Destroy(audioSource.audio);
                            element.Key.AlarmsDoneCallback();
                        }
                    }
                }
            }
        }

        public void RemoveAllAlarmsForModule(FailureModule fm)
        {
             Log.Info("[DangIt] [AlarmManager] Removing alarms...");
           if (audioSource.audio != null)
                audioSource.audio.volume = 0;
            if (loops.Keys.Contains(fm))
            {
                fm.AlarmsDoneCallback();
                loops.Remove(fm);
                //RemovePartFailure(fm.part);
            }
        }

        public void RemoveAllAlarms()
        {
            while (loops.Count > 0)
            {
                var element = loops.ElementAt(0);
                loops.Remove(element.Key);
               // RemovePartFailure(element.Key.part);
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

