﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP_PartHighlighter;

using static nsDangIt.DangIt;


namespace nsDangIt
{
    /// <summary>
    /// Base failure module that abstracts all the common behaviour for discrete failures:
    /// * keeps track of the part's age and failure chance
    /// * causes random failures
    /// * handles the EVA repair and preemptive maintenance
    /// * Returns the message for inspections
    /// </summary>
    public abstract class FailureModule : PartModule, IPartCostModifier
    {

        #region Custom strings

        // These strings customize the failure module, both in the log
        // and in the messages that are shown to the user.

        public abstract string ScreenName { get; }                   // name shown to the user during inspections or in the editors (e.g, "Alternator")
        public abstract string DebugName { get; }                    // name used to identify the module in the debug logs
        public abstract string RepairMessage { get; }                // message posted to the screen upon successful repair
        public abstract string FailureMessage { get; }               // message posted to the screen upon failure
        public abstract string FailGuiName { get; }                  // gui name for the failure event (when visible)
        public abstract string EvaRepairGuiName { get; }             // gui name for the EVA repair event
        public abstract string MaintenanceString { get; }            // gui name for maintinence event
        public virtual string ExtraEditorInfo { get { return ""; } }  // extra descriptive info for the

      
        /// <summary>
        /// Returns the string that is displayed during an inspection.
        /// </summary>
        public virtual string InspectionMessage()
        {
            Log.Info("InspectionMessage");

            if (this.HasFailed)
                return "the part has failed!";

            // The same experience that is needed for repair is also needed to inspect the element
            Part evaPart = DangIt.FindEVAPart();
            if (evaPart != null)
            {
                if (!CheckOutExperience(evaPart.protoModuleCrew[0]))
                    return evaPart.protoModuleCrew[0].name + " isn't quite sure about this...";
            }

            // Perks check out, return a message based on the age
            float ratio = this.Age / this.LifeTimeSecs;

            if (ratio < 0.10)
                return "This part seems to be as good as new";
            else if (ratio < 0.50)
                return "This part is still in good condition";
            else if (ratio < 0.75)
                return "This part is starting to show its age";
            else if (ratio < 1.25)
                return "It looks like it's time to get a new one";
            else if (ratio < 2.00)
                return "It really isn't a good idea to keep using this part";
            else if (ratio < 3)
                return "This part needs replacing soon";
            else
                return "This part is in terrible condition";
        }

        #endregion


        #region Methods to add the specific logic of the module

        protected virtual void DI_Reset() { }
        protected virtual void DI_OnLoad(ConfigNode node) { }
        protected virtual void DI_Start(StartState state) { }
        protected virtual void DI_RuntimeFetch() { }
        protected virtual void DI_Update() { }
        protected abstract bool DI_AllowedToFail();
        protected abstract bool DI_FailBegin();
        protected abstract void DI_Disable();
        protected abstract void DI_EvaRepair();
        protected virtual void DI_OnSave(ConfigNode node) { }
        public virtual bool PartIsActive() { return true; }
        protected virtual float LambdaMultiplier() { return 1f; }
        public virtual bool DI_ShowInfoInEditor() { return true; }

        #endregion
        #region IPartCostModifier
        public float GetModuleCost(float defaultCost, ModifierStagingSituation sit)
        {
            return 0;
            //return defaultCost;
        }
        public ModifierChangeWhen GetModuleCostChangeWhen()
        {
            return ModifierChangeWhen.FIXED;
        }
        #endregion

        #region Fields from the cfg file

        [KSPField(isPersistant = true, guiActive = false)]
        public float MTBF = 1000f;                                  // Original Mean Time Between Failures.

        [KSPField(isPersistant = true, guiActive = false)]
        public float LifeTime = 100f;                               // Time constant of the exponential decay

        [KSPField(isPersistant = true, guiActive = false)]
        public float RepairCost = 5f;                               // Amount of spares needed to repair the part

        [KSPField(isPersistant = true, guiActive = false)]
        public float RepairBonus = 0f;                              // Age discount during a repair (percentage, between 0 and 1)

        [KSPField(isPersistant = true, guiActive = false)]
        public float MaintenanceCost = 1f;                          // Amount of spares needed to perform maintenance

        [KSPField(isPersistant = true, guiActive = false)]
        public float MaintenanceBonus = 0.2f;                       // Age discount for preemptive maintenance

        [KSPField(isPersistant = true, guiActive = false)]
        public float InspectionBonus = 60f;                         // Duration of the inspection bonus

        [KSPField(isPersistant = true, guiActive = false)]
        public bool Silent = false;                                 // If this flag is true, no message is displayed when failing

        [KSPField(isPersistant = true, guiActive = false)]
        public string Priority = "MEDIUM";                          // Priority of the failure as string. Used for beeping

        [KSPField(isPersistant = true, guiActive = false)]
        public string PerksRequirementName = "";                    // Trait name required to fix this part. "" = Any

        [KSPField(isPersistant = true, guiActive = false)]
        public int PerksRequirementValue = 0;                       // Skill level required to fix this part.

        // This is NOT persistant
        internal static float streamMultiplier = 0;
        internal static float decayPerMinute = 0;
        internal static double lastDecayTime = 0;
        #endregion


        #region Internal state

        [KSPField(isPersistant = true, guiActive = false)]
        public bool HasInitted = false;

        [KSPField(isPersistant = true, guiActive = false)]
        public float TimeOfLastReset = float.PositiveInfinity;

        [KSPField(isPersistant = true, guiActive = false)]
        public float TimeOfLastInspection = float.NegativeInfinity;

        [KSPField(isPersistant = true, guiActive = false)]
        public float Age = 0;

        [KSPField(isPersistant = true, guiActive = false)]
        public float LastFixedUpdate = 0;

        [KSPField(isPersistant = true, guiActive = false)]
        public float CurrentMTBF = float.PositiveInfinity;

        [KSPField(isPersistant = true, guiActive = false)]
        public float LifeTimeSecs = float.PositiveInfinity;

        [KSPField(isPersistant = true, guiActive = false)]
        public bool HasFailed = false;

        #endregion


#if false
        bool printChances = false;
        [KSPEvent(active = true, guiActive = true)]
        public void PrintChances()
        {
            printChances = !printChances;
        }
#endif

        #region Lambda


        /// <summary>
        /// Chance that the part will fail during the next fixed update.
        /// </summary>
        public float Lambda()
        {
            float f = LambdaFromMTBF(this.CurrentMTBF)
                    * (1 + TemperatureMultiplier())     // the temperature increases the chance of failure
                    * LambdaMultiplier()                // optional multiplier from the child class
                    * InspectionLambdaMultiplier()      // temporary inspection bonus
                    * StreamMultiplier();               // for streamers, temporarily increase the chance for failure
#if false
            if (printChances)
                DangIt.myLog.Info("Lambda: " + f.ToString());
#endif      
            return f;
        }


        /// <summary>
        /// Convert a MTBF in hours to the chance of failure during the next fixed update.
        /// </summary>
        private float LambdaFromMTBF(float MTBF)
        {
            float f = 0f;
            try
            {
                f = (1f / MTBF) / 3600f * TimeWarp.fixedDeltaTime;
            }
            catch (Exception e)
            {
                OnError(e);
                //return 0f;
            }
#if false
            if (printChances)
                Log.Info("LambdaFromMTBF: " + f.ToString());
#endif    
            return f;
        }


        /// <summary>
        /// Multiplier that reduces the chance of failure right after an inspection.
        /// </summary>
        private float InspectionLambdaMultiplier()
        {
            float elapsed = (DangIt.Now() - this.TimeOfLastInspection);
            // Constrain it between 0 and 1
            float f = Math.Max(0f, Math.Min(elapsed / this.InspectionBonus, 1f));
#if false
            if (printChances)
                Log.Info("InspectionLambdaMultiplier: " + f.ToString());
#endif    
            return f;

        }


        #endregion


        /// <summary>
        /// Coroutine that waits for the runtime to be ready before executing.
        /// Sets the range and gui name of the Fail, EvaRepair and Maintenance events,
        /// and then calls DI_RuntimeFetch() so that child classes can interact with the runtime.
        /// </summary>
        IEnumerator RuntimeFetch()
        {
            Log.Info("FailureModule.RuntimeFetch, part: " + this.part.partInfo.title);
            // Wait for the server to be available
            while (DangIt.Instance == null || !DangIt.Instance.IsReady)
                yield return null;
            Log.Info("FailureModule.RuntimeFetch, part: " + this.part.partInfo.title + " ready");

            if (DI_AllowedToFail())
                this.Events["Fail"].guiActive = DangIt.Instance.CurrentSettings.ManualFailures;
            else
                this.Events["Fail"].guiActive = false;
            this.Events["EvaRepair"].unfocusedRange = DangIt.Instance.CurrentSettings.MaxDistance;
            this.Events["Maintenance"].unfocusedRange = DangIt.Instance.CurrentSettings.MaxDistance;

            this.Fields["Age"].guiName = DebugName + " Age";
            this.Fields["Age"].guiActive = DangIt.Instance.CurrentSettings.DebugStats;

            DI_RuntimeFetch();
        }


        /// <summary>
        /// Resets the failure state and age tracker.
        /// This must be called only at the beginning of the flight to initialize the age tracking.
        /// Put your reset logic in DI_Reset()
        /// </summary>
        protected void Reset()
        {
            Log.Info("Reset(), HasInitted: " + HasInitted.ToString());
            try
            {
                this.FailureLog("Resetting");

                float now = DangIt.Now();

                #region Internal state

                this.Age = 0;
                this.TimeOfLastReset = now;
                this.LastFixedUpdate = now;

                this.TimeOfLastInspection = float.NegativeInfinity;

                this.CurrentMTBF = this.MTBF * HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams1>().MTBF_Multiplier;
                this.LifeTimeSecs = this.LifeTime * HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams1>().Lifetime_Multiplier * 3600f;
                this.HasFailed = false;
                #endregion
              
                // Run the custom reset of the subclasses
                this.DI_Reset();

                this.HasInitted = true;


            }
            catch (Exception e)
            {
                OnError(e);
            }

        }



        /// <summary>
        /// Load the values from the config node of the persistence file.
        /// Put your loading logic in DI_OnLoad()
        /// </summary>
        public override void OnLoad(ConfigNode node)
        {
            try
            {
                // Load all the internal state variables
                this.HasInitted = DangIt.Parse<bool>(node.GetValue("HasInitted"), false);
                this.Age = DangIt.Parse<float>(node.GetValue("Age"), defaultTo: 0f);
                this.TimeOfLastReset = DangIt.Parse<float>(node.GetValue("TimeOfLastReset"), defaultTo: float.PositiveInfinity);
                this.TimeOfLastInspection = DangIt.Parse<float>(node.GetValue("TimeOfLastInspection"), defaultTo: float.NegativeInfinity);
                this.LastFixedUpdate = DangIt.Parse<float>(node.GetValue("LastFixedUpdate"), defaultTo: 0f);
                this.CurrentMTBF = DangIt.Parse<float>(node.GetValue("CurrentMTBF"), defaultTo: float.PositiveInfinity);
                this.LifeTimeSecs = DangIt.Parse<float>(node.GetValue("LifeTimeSecs"), defaultTo: float.PositiveInfinity);
                this.HasFailed = DangIt.Parse<bool>(node.GetValue("HasFailed"), defaultTo: false);

                Log.Info("FailureModule.OnLoad");
                // Run the subclass' custom onload
                this.DI_OnLoad(node);

                // If OnLoad is called during flight, call the start again
                // so that modules can be rescanned
                if (HighLogic.LoadedSceneIsFlight)
                    this.DI_Start(StartState.Flying);

                base.OnLoad(node);                

            }
            catch (Exception e)
            {
                this.OnError(e);
            }
        }



        /// <summary>
        /// Saves the internal state of the failure module to the persistence file.
        /// Put your custom save logic in DI_OnSave()
        /// </summary>
        public override void OnSave(ConfigNode node)
        {
            try
            {
                // Save the internal state
                node.SetValue("HasInitted", this.HasInitted.ToString());
                node.SetValue("Age", Age.ToString());
                node.SetValue("TimeOfLastReset", TimeOfLastReset.ToString());
                node.SetValue("TimeOfLastInspection", TimeOfLastInspection.ToString());
                node.SetValue("LastFixedUpdate", LastFixedUpdate.ToString());
                node.SetValue("CurrentMTBF", CurrentMTBF.ToString());
                node.SetValue("LifeTimeSecs", LifeTimeSecs.ToString());
                node.SetValue("HasFailed", HasFailed.ToString());

                // Run the subclass' custom onsave
                this.DI_OnSave(node);

                base.OnSave(node);
            }
            catch (Exception e)
            {
                this.OnError(e);
            }
        }

        public static Dictionary<Guid, int> vesselHighlightDict = null;
        public static PartHighlighter phl = null;

        public override void OnAwake()
        {
            Log.Info("FailureModule.OnAwake");

            if (phl == null)
                phl = PartHighlighter.CreatePartHighlighter();
            
            if (vesselHighlightDict== null)
                vesselHighlightDict = new Dictionary<Guid, int>();

        }


        /// <summary>
        /// Module re-start logic. OnStart will be called usually once for each scene, editor included.
        /// Put your custom start logic in DI_Start(): if you need to act on other part's
        /// variable, this is the place to do it, not DI_Reset()
        /// </summary>
        public override void OnStart(PartModule.StartState state)
        {
            Log.Info("FailureModule.OnStart, part: " + this.part.partInfo.title);
            try
            {
                if (HighLogic.LoadedSceneIsFlight) // nothing to do in editor
                {
                    this.FailureLog("Starting in flight: last reset " + TimeOfLastReset + ", now " + DangIt.Now());

                    if (!DangIt.Instance.CurrentSettings.EnabledForSave)
                    { //Disable if we've disabled DangIt
                        foreach (var e in this.Events)
                        {
                            e.guiActive = false;
                        }
                    }

                    #region Fail and repair events

                    this.Events["Fail"].guiName = this.FailGuiName;
                    this.Events["EvaRepair"].guiName = this.EvaRepairGuiName;
                    this.Events["Maintenance"].guiName = this.MaintenanceString;

                    #endregion

                    // Reset the internal state at the beginning of the flight
                    // this condition also catches a revert to launch (+1 second for safety)
                    if (DangIt.Now() < (this.TimeOfLastReset + 1))
                        this.Reset();

                    // If the part was saved when it was failed,
                    // re-run the failure logic to disable it
                    // ONLY THE DISABLING PART IS RUN!
                    if (this.HasFailed)
                        this.DI_Disable();

                    this.SetFailureState(this.HasFailed);

                    if (!vesselHighlightDict.ContainsKey(vessel.id))
                    {
                        var failedHighlightID = phl.CreateHighlightList(0);
                        if (failedHighlightID < 0)
                            return;
                        if (DangIt.Instance.CurrentSettings.FlashingGlow)
                        {
                            phl.SetFlashInterval(failedHighlightID, HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams1>().flashingInterval);
                        }
                        vesselHighlightDict[vessel.id] = failedHighlightID;
                    }
                    phl.UpdateHighlightColors(vesselHighlightDict[vessel.id], Color.red);


                    if (DangIt.Instance.CurrentSettings.EnabledForSave)
                    {
                        this.DI_Start(state);
                        this.StartCoroutine("RuntimeFetch");
                    }
                }
            }
            catch (Exception e)
            {

                OnError(e);
            }

        }


        /// <summary>
        /// Update logic on every physics frame update.
        /// Place your custom update logic in DI_Update()
        /// </summary>
        public void FixedUpdate()
        {
            if (streamMultiplier > 0 && Planetarium.GetUniversalTime() - lastDecayTime > 1)
            {
                streamMultiplier = Math.Max(0, streamMultiplier -  decayPerMinute);
                lastDecayTime = Planetarium.GetUniversalTime();
            }
            try
            {
                // Only update the module during flight and after the re-initialization has run
                if (HighLogic.LoadedSceneIsFlight && this.HasInitted && this.vessel == FlightGlobals.ActiveVessel)
                {
                    // Highlighting the part, which contains this updating FailureModule if it is in a 'failed' state,
                    // it is not in 'silent' state and 'glow' is globally enabled
                    // Actually, there is not any place in a code of  the whole mod where that 'silent' state is turning on
                    // (maybe some FailureModules can be defined as 'silent' by editing files)

                    if (this.HasFailed && /* !this.Silent */  (DangIt.Instance.CurrentSettings.Glow && (AlarmManager.visibleUI || !DangIt.Instance.CurrentSettings.DisableGlowOnF2)))
                    {
                        phl.AddPartToHighlight(vesselHighlightDict[this.vessel.id], this.part);
                        //Debug.Log("Adding part to highlight list: " + this.part.partInfo.title);
                    } else

                    // Turning off the highlighting of the part, which contains this updating FailureModule
                    // if it is not in a 'failed' state, or it is in 'silent' state, or if 'glow' is globally disabled
                    //      if (!this.HasFailed || this.Silent || !DangIt.Instance.CurrentSettings.Glow || (!visibleUI && DangIt.Instance.CurrentSettings.DisableGlowOnF2))
                    {

                        if (!AlarmManager.partFailures.ContainsKey(this.part))
                        {
                            phl.DisablePartHighlighting(vesselHighlightDict[this.vessel.id], this.part);
                        }
                    }



                    float now = DangIt.Now();

                    float dt = now - LastFixedUpdate;
                    this.LastFixedUpdate = now;

                    // The temperature aging is independent from the use of the part
                    this.Age += (dt * this.TemperatureMultiplier());

                    if (!PartIsActive() || !DangIt.Instance.CurrentSettings.EnabledForSave)
                        return;
                    else
                    {
                        this.Age += dt;

                        this.CurrentMTBF = this.MTBF * HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams1>().MTBF_Multiplier * this.ExponentialDecay();

                        // If the part has not already failed, toss the dice
                        if (!this.HasFailed)
                        {
                            float f = this.Lambda();
#if DEBUG
                           // if (printChances)
                           //     DangIt.myLog.Debug("this.Lambda: " + f.ToString());
#endif


                            if (UnityEngine.Random.Range(0f, 1f) < f)
                            {
                                streamMultiplier = 0;
                                this.Fail();
                            }
                        }

                        // Run custom update logic
                        this.DI_Update();
                    }
                }
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }


        private float ExponentialDecay()
        {
            return (float)Math.Exp(-this.Age / this.LifeTimeSecs);
        }

        private float StreamMultiplier()
        {
            return 1 + streamMultiplier;
        }

        /// <summary>
        /// Increase the aging rate as the temperature increases.
        /// </summary>
        private float TemperatureMultiplier()
        {
            float f = 3 * (float)Math.Pow((Math.Max(part.temperature, 0) / part.maxTemp), 5);
#if false
            if (printChances)
                Log.Info("TemperatureMultiplier: " + f.ToString());
#endif
            return f;

        }


        /// <summary>
        /// Pre-emtpive maintenance procedure.
        /// This allows the kerbal to service a functioning part to permanently discount part of the age,
        /// thus making it permanently more reliable.
        /// </summary>
        [KSPEvent(active = true, guiActive = false, guiActiveUnfocused = true, unfocusedRange = 2f, externalToEVAOnly = true)]
        public void Maintenance()
        {
            this.FailureLog("Initiating EVA maitenance");

            Part evaPart = DangIt.FindEVAPart();
            if (evaPart == null)
            {
                throw new Exception("ERROR: couldn't find an active EVA!");
            }

            if (!CheckOutExperience(evaPart.protoModuleCrew[0]))
            {
                DangIt.Broadcast(evaPart.protoModuleCrew[0].name + " isn't really qualified for this...", true);
                return;
            }

            if (this.part.temperature > DangIt.Instance.CurrentSettings.GetMaxServicingTemp())
            {
                DangIt.Broadcast("This is too hot to service right now", true);
                return;
            }


            // Check if he is carrying enough spares
            if (evaPart.Resources.Contains(Spares.Name) && evaPart.Resources[Spares.Name].amount >= this.MaintenanceCost)
            {
                this.FailureLog("Spare parts check: OK! Maintenance allowed allowed");

                // Consume the spare parts
                // MC2 Breaks RequestResource, since amount is checked, simply decrement! Just like in SparesContainer! Whee! -TrypChangeling
                // evaPart.RequestResource(Spares.Name, this.MaintenanceCost);
                evaPart.Resources[Spares.Name].amount -= this.MaintenanceCost;

                // Distance between the kerbal's perks and the required perks, used to scale the maintenance bonus according to the kerbal's skills
                int expDistance = evaPart.protoModuleCrew[0].experienceLevel - this.PerksRequirementValue;

                //// The higher the skill gap, the higher the maintenance bonus
                //// The + 1 is there to makes it so that a maintenance bonus is always gained even when the perks match exactly
                this.DiscountAge(this.MaintenanceBonus * ((expDistance + 1) / 3));

                DangIt.Broadcast("This should last a little longer now");
            }
            else
            {
                this.FailureLog("Spare parts check: failed! Maintenance NOT allowed");
                DangIt.Broadcast("You need " + this.MaintenanceCost + " spares to maintain this.");
            }

        }


        /// <summary>
        /// Initiates the part's failure.
        /// Put your custom failure code in DI_Fail()
        /// </summary>
        [KSPEvent(active=true, guiActive = false)]
        public void Fail()
        {
            try
            {
                this.FailureLog("Initiating Fail()");

                // First, run the custom failure logic
                // The child class can refuse to fail in FailBegin()
                if (!this.DI_FailBegin())
                {
                    this.FailureLog(this.DebugName + " has not agreed to fail, failure aborted!");
                    return;
                }
                else
                {
                    this.FailureLog(this.DebugName + " has agreed to fail, failure allowed.");
                }

                // If control reaches this point, the child class has agreed to fail
                // Disable the part and handle the internal state and notifications

                this.DI_Disable();

                TimeWarp.SetRate(0, true);      // stop instantly
                this.SetFailureState(true);     // Sets the failure state, handles the events

                if (!this.Silent)
                {
                    DangIt.Broadcast(this.FailureMessage);
                    DangIt.PostMessage("Failure!",
                                       this.FailureMessage,
                                       MessageSystemButton.MessageButtonColor.RED,
                                       MessageSystemButton.ButtonIcons.ALERT);

                    AlarmManager alarmManager = FindObjectOfType<AlarmManager>();
                    if (alarmManager != null)
                    {
                        alarmManager.AddAlarm(this, DangIt.Instance.CurrentSettings.GetSoundLoopsForPriority(Priority));
                        if (alarmManager.HasAlarmsForModule(this))
                        {
                            Events["MuteAlarms"].active = true;
                            Events["MuteAlarms"].guiActive = true;
                        }
                    }
                    else
                        Log.Info("alarmManager is null");
                }

                DangIt.FlightLog(this.FailureMessage);
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        /// <summary>
        /// Sets / resets the failure of the part.
        /// Also sets the event's visibility
        /// </summary>
        protected void SetFailureState(bool state)
        {
            try
            {
                this.HasFailed = state;

                Events["Fail"].active = !state;
                Events["EvaRepair"].active = state;
                Events["Maintenance"].active = !state;
            }
            catch (Exception e)
            {
                this.OnError(e);
            }
        }



        /// <summary>
        /// Initiates the part's EVA repair.
        /// The repair won't be executed if the kerbonaut doesn't have enough spare parts.
        /// Put your custom repair code in DI_Repair()
        /// </summary>
        [KSPEvent(guiActiveUnfocused = true, unfocusedRange = 2f, externalToEVAOnly = true)]
        public void EvaRepair()
        {
            try
            {
                this.FailureLog("Initiating EVA repair");

                // Get the EVA part (parts can hold resources)
                Part evaPart = DangIt.FindEVAPart();

                if (evaPart == null)
                {
                    throw new Exception("ERROR: couldn't find an active EVA!");
                }

                // Check if the kerbal is able to perform the repair
                if (CheckRepairConditions(evaPart))
                {
                    this.DI_EvaRepair();
                    this.SetFailureState(false);

                    DangIt.FlightLog(this.RepairMessage);

                    //TODO: experience repair boni
                    float intelligence = 1 - evaPart.protoModuleCrew[0].stupidity;
                    float discountedCost = (float)Math.Round(RepairCost * (1 - UnityEngine.Random.Range(0f, intelligence)));
                    float discount = RepairCost - discountedCost;

                    this.FailureLog("Kerbal's intelligence: " + intelligence + ", discount: " + discount);

                    // One more MC2 hack - TrypChangeling
                    // evaPart.RequestResource(Spares.Name, discountedCost);
                    evaPart.Resources[Spares.Name].amount -= discountedCost;
                    ResourceDisplay.Instance.Refresh();

                    DangIt.Broadcast(this.RepairMessage, true);
                    this.DiscountAge(this.RepairBonus);

                    if (discount > 0)
                    {
                        DangIt.Broadcast(evaPart.protoModuleCrew[0].name + " was able to save " + discount + " spare parts");
                    }
                    AlarmManager alarmManager = FindObjectOfType<AlarmManager>();
                    alarmManager.RemoveAllAlarmsForModule(this); //Remove alarms from this module
                }

            }
            catch (Exception e)
            {
                OnError(e);
            }

        }


        /// <summary>
        /// Check if a kerbal is able to repair a part,
        /// factoring spares, perks, and additional conditions
        /// </summary>
        private bool CheckRepairConditions(Part evaPart)
        {
            bool allow = true;
            string reason = string.Empty;


#region Amount of spare parts
            if (!evaPart.Resources.Contains(Spares.Name) || evaPart.Resources[Spares.Name].amount < this.RepairCost)
            {
                allow = false;
                reason = "not carrying enough spares";
                DangIt.Broadcast("You need " + this.RepairCost + " spares to repair this.", true);
            }
#endregion

#region Part temperature
            if (this.part.temperature > DangIt.Instance.CurrentSettings.GetMaxServicingTemp())
            {
                allow = false;
                reason = "part is too hot (" + part.temperature.ToString() + " degrees)";
                DangIt.Broadcast("This is too hot to service right now", true);
            }
#endregion


            if (!CheckOutExperience(evaPart.protoModuleCrew[0]))
            {
                allow = false;
                reason = "perks don't match requirements";
                DangIt.Broadcast(evaPart.protoModuleCrew[0].name + " has no idea how to fix this...", true);
            }


            if (allow)
                this.FailureLog("Repair allowed!");
            else
                this.FailureLog("Repair NOT allowed. Reason: " + reason);

            return allow;
        }


        /// <summary>
        /// Checks if a kerbal has the required experience to interact with this module
        /// </summary>
        bool CheckOutExperience(ProtoCrewMember kerbal)
        {
            // Haskell made me fond of unreadable one-line functional expressions.
            this.FailureLog("Checking Experience");
            this.FailureLog("this.PerksRequirementName       = " + this.PerksRequirementName);
            this.FailureLog("this.PerksRequirementValue      = " + this.PerksRequirementValue);
            this.FailureLog("kerbal.experienceTrait.TypeName = " + kerbal.experienceTrait.TypeName);
            this.FailureLog("kerbal.experienceLevel          = " + kerbal.experienceLevel);
            return !DangIt.Instance.CurrentSettings.RequireExperience || string.IsNullOrEmpty(this.PerksRequirementName)                  // empty string means no restrictions
                || ((kerbal.experienceTrait.Config.Name == this.PerksRequirementName) && (kerbal.experienceLevel >= this.PerksRequirementValue));
        }


        /// <summary>
        /// Decreases the part's age by the given percentage.
        /// </summary>
        private void DiscountAge(float percentage)
        {
            this.Age *= (1 - percentage);
            this.Age = Math.Max(this.Age, 0);   // prevent negative ages if the percentage is greater than 100%
        }


#region Logging utilities

        public void FailureLog(string msg)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(this.DebugName);
                if (part != null)
                {
                    if (part.vessel != null) sb.Append("[Ship: " + part.vessel.vesselName + "]");
                    if (part.partInfo != null)
                        sb.Append("[" + part.partInfo.title + "]");
                }
                sb.Append(": " + msg);

                Log.Info(sb.ToString());
            }
            catch (Exception e)
            {
                this.OnError(e);
            }
        }

        /// <summary>
        /// Exception handling code: logs the exception message and then disables the module.
        /// Disabled modules are not updated.
        /// </summary>
        protected void OnError(Exception e)
        {
            LogException(e);
            this.enabled = false;   // prevent the module from updating
            return;
        }

        public void LogException(Exception e)
        {
            Log.Error("ERROR [" + e.GetType().ToString() + "]: " + e.Message + "\n" + e.StackTrace);
        }

#endregion


        /// <summary>
        /// Reduces the value of the part when it is recovered.
        /// </summary>
        public float GetModuleCost(float defaultCost)
        {
            return (this.ExponentialDecay() - 1) * defaultCost;
        }

        [KSPEvent(guiActive = false, active = false, guiName = "Mute Alarm")]
        public void MuteAlarms()
        {
            Log.Info("Muting alarms for... " + this.ToString());
            AlarmManager alarmManager = FindObjectOfType<AlarmManager>();
            if (alarmManager != null)
            {
                alarmManager.RemoveAllAlarmsForModule(this);
            }
        }

        public void AlarmsDoneCallback()
        { //Called from AlarmManager when no alarms remain
            Log.Info("AlarmsDoneCallback called");
            Events["MuteAlarms"].active = false;
            Events["MuteAlarms"].guiActive = false;
        }
    }

}
