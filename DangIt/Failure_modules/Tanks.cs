using KSP.Localization;
using System;
using System.Linq;
using System.Collections.Generic;
using KSP;
using System.Collections;
using UnityEngine;

namespace nsDangIt
{
    using static nsDangIt.DangIt;

    public class ModuleTankReliability : FailureModule
    {
        #region NO_LOCALIZATION
        public override string DebugName { get { return "DangItTank"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_136"); } }
        public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_137") + leakName + Localizer.Format("#LOC_DangIt_138"); } }
        public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_139"); } }
        public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_140"); } }
        public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_141"); } }
        public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_142"); } }
        public override string ExtraEditorInfo
        {
            get
            {
                var temp = Localizer.Format("#LOC_DangIt_143") + part.partInfo.title + Localizer.Format("#LOC_DangIt_144");

                foreach (PartResource pr in part.Resources)
                {
                    if (!DangIt.LeakBlackList.Contains(pr.resourceName))
                        temp += pr.resourceName + ", ";
                };
                Log.Info(temp);
                return temp.TrimEnd(' ').TrimEnd(',');
            }
        }


        // The leak is modeled as an exponential function
        // by approximating the differential equation
        // dQ(t) = - pole * Q(t)
        // where Q is the amount of fuel left in the tank
        [KSPField(isPersistant = true, guiActive = false)]
        protected float pole = 0.01f;


        // Maximum and minimum values of the time constant
        // The time constant is generated randomly between these two limits
        // and pole = 1 / TC
        [KSPField(isPersistant = true, guiActive = false)]
        public float MaxTC = 60f;

        [KSPField(isPersistant = true, guiActive = false)]
        public float MinTC = 10f;

        // Name of the leaking resource
        [KSPField(isPersistant = true, guiActive = false)]
        public string leakName = "#LOC_DangIt_145";


        // List of resources that the module will choose from when starting a new leak.
        // This list is created when the module is started by taking all the resources
        // in the part and excluding the ones that have been blacklisted in the configuration file
        protected List<PartResource> leakables;


        // This method is executed once at startup during a coroutine
        // that waits for the runtime component to be available and then triggers
        // this method.
        protected override void DI_RuntimeFetch()
        {
            leakables = new List<PartResource>();

            // At this point DangIt.Instance is not null: fetch the blacklist
            foreach (PartResource pr in part.Resources)
            {
                if (!DangIt.LeakBlackList.Contains(pr.resourceName))
                    this.leakables.Add(pr);
            }


            // If no leakables are found, just disable the module
            if (leakables.Count == 0)
            {
                Log.Info("The part " + this.part.name + " does not contain any leakable resource.");
                this.Events["Fail"].active = false;
                this.leakName = Localizer.Format("#LOC_DangIt_145"); // null;
                this.enabled = false; // disable the monobehaviour: this won't be updated
            }
        }

        protected override void DI_Start(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                // The part was already failed when loaded:
                // check if the resource is still in the tank
                if (this.HasFailed)
                {
                    if (string.IsNullOrEmpty(leakName) || leakName == Localizer.Format("#LOC_DangIt_145") || !part.Resources.Contains(leakName))
                    {
                        this.FailureLog(Localizer.Format("#LOC_DangIt_146")); ;
                        this.SetFailureState(false);
                    }
                }

            }
        }



        protected override void DI_OnLoad(ConfigNode node)
        {
            if (part != null && part.partInfo != null)
                Log.Info("ModuleTankReliability.DI_OnLoad, part: " + part.partInfo.title);
            else
                Log.Info("ModuleTankReliability.DI_OnLoad, no part");
            this.pole = DangIt.Parse<float>(Localizer.Format("#LOC_DangIt_147"), 0.01f);

            this.leakName = node.GetValue("leakName");

            if (string.IsNullOrEmpty(leakName))
                leakName = Localizer.Format("#LOC_DangIt_145"); // null;;

            this.FailureLog(Localizer.Format("#LOC_DangIt_148") + ((leakName == null) ? Localizer.Format("#LOC_DangIt_145") : leakName));
        }



        protected override void DI_OnSave(ConfigNode node)
        {
            if (leakName == null)
                leakName = "none"; //  Log.Info("Adding leak: (empty)");
            else
                Log.Info("Adding leak: [" + leakName + "]");

            node.SetValue("leakName", (leakName == null) ? "none" : leakName);
            node.SetValue("pole", this.pole.ToString());
        }



        protected override void DI_Update()
        {
            try
            {
                if (this.HasFailed &&
                   (!(string.IsNullOrEmpty(leakName)  || leakName == Localizer.Format("#LOC_DangIt_145")) &&
                   (part.Resources[leakName].amount > 0)))  // ignore empty tanks
                {
                    double amount = pole * part.Resources[leakName].amount * TimeWarp.fixedDeltaTime;

                    // The user can disable the flow from tanks: if he does, RequestResource
                    // won't drain anything.
                    // In that case, we need to subtract directly the amount we want

                    if (part.Resources[leakName].flowState)
                        part.RequestResource(leakName, amount);
                    else
                    {
                        part.Resources[leakName].amount -= amount;
                        part.Resources[leakName].amount = Math.Max(part.Resources[leakName].amount, 0);
                    }
                }
            }
            catch (Exception e)
            {
                OnError(e);
                this.isEnabled = false;
                this.SetFailureState(false);
            }
        }


        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams2>().AllowTankFailures;
        }

        protected override bool DI_FailBegin()
        {
            if (!DI_AllowedToFail())
                return false;

            // Something has gone very wrong somewhere
            if (leakables == null)
                throw new Exception(Localizer.Format("#LOC_DangIt_149"));

            // Discard every resource that has already been emptied
            leakables.RemoveAll(r => r.amount == 0);

            if (leakables.Count > 0)
            {
                // Choose a random severity of the leak
                // The lower TC, the faster the leak
                float TC = UnityEngine.Random.Range(MinTC, MaxTC);
                this.pole = 1 / TC;

                this.FailureLog(string.Format( Localizer.Format("#LOC_DangIt_150") + " {0} " + Localizer.Format("#LOC_DangIt_151") + " {1}" + Localizer.Format("#LOC_DangIt_152") + " {2}" +")", TC, MinTC, MaxTC));

                // Pick a random index to leak.
                // Random.Range excludes the upper bound,
                // BUT because list.Count returns the length, not the max index, we DONT need a +1
                // e.g. [1].Count == 1 but MyListWithOneItem[1] == IndexError

                int idx = UnityEngine.Random.Range(0, leakables.Count);
                Log.Info("Selected IDX: " + idx.ToString());
                Log.Info("Length of leakables: " + this.leakables.Count.ToString());
                Log.Info("Leakables: " + this.leakables.ToString());

                this.leakName = leakables[idx].resourceName;

                // Picked a resource, allow failing
                return true;
            }
            else
            {
                leakName = Localizer.Format("#LOC_DangIt_145"); // null;
                this.FailureLog(Localizer.Format("#LOC_DangIt_153") + this.part.partName + Localizer.Format("#LOC_DangIt_154"));

                // Disallow failing
                return false;
            }
        }



        protected override void DI_Disable()
        {
            // nothing to do for tanks
            return;
        }



        protected override void DI_EvaRepair()
        {
            this.leakName = Localizer.Format("#LOC_DangIt_145");
        }


#if false
        [KSPEvent(active = true, guiActive = true)]
        public void PrintStatus()
        {
            this.FailureLog("#LOC_DangIt_155");
            foreach (PartResource res in this.part.Resources)
            {
                this.FailureLog(res.resourceName + ": " + res.flowMode + ", " + res.flowState);
            }

        }

        [KSPEvent(active = true, guiActive = true)]
        public void PrintBlackList()
        {
            this.FailureLog("#LOC_DangIt_156");
            foreach (string item in DangIt.LeakBlackList)
            {
                this.FailureLog(Localizer.Format("#LOC_DangIt_157") + item);
            }
            this.FailureLog(Localizer.Format("#LOC_DangIt_158"));
        }
#endif
        public override bool DI_ShowInfoInEditor()
        {
            foreach (PartResource pr in part.Resources)
            {
                if (!DangIt.LeakBlackList.Contains(pr.resourceName))
                    return true;
            }
            return false;
            //return part.Resources.GetAll().FindAll(r => !DangIt.LeakBlackList.Contains(r.resourceName)).Count>0; //Only show if has leakable rescoures
        }
    }
}
