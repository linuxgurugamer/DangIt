using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Added by Linuxgurugamer

namespace nsDangIt
{
    using static nsDangIt.DangIt;

    public class ModuleDeployableAntennaReliability : FailureModule
    {
        ModuleDeployableAntenna antennaModule;

        public override string DebugName { get { return "DangItDeployableAntenna"; } }
        public override string ScreenName { get { return "Deployable Antenna"; } }
        public override string FailureMessage { get { return "Antenna failure!"; } }
        public override string RepairMessage { get { return "Antenna repaired."; } }
        public override string FailGuiName { get { return "Fail antenna"; } }
        public override string EvaRepairGuiName { get { return "Replace antenna"; } }
        public override string MaintenanceString { get { return "Replace antenna"; } }
        public override string ExtraEditorInfo { get { return "This part's antenna can stop providing communications if it fails"; } }


        protected override void DI_Start(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                this.antennaModule = this.part.Modules.OfType<ModuleDeployableAntenna>().First();
            }
        }

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams2>().AllowDeployableAntennaFailures;
        }

        protected override bool DI_FailBegin()
        {
            return DI_AllowedToFail();
            // Can always fail
            // return true;

        }

        ModuleDeployablePart.DeployState lastDeployState; 
        protected override void DI_Disable()
        {
            this.antennaModule.enabled = false;
            lastDeployState = this.antennaModule.deployState;
            this.antennaModule.deployState = ModuleDeployablePart.DeployState.BROKEN;
        }


        protected override void DI_EvaRepair()
        {
            this.antennaModule.enabled = true;
            this.antennaModule.deployState = lastDeployState;
        }

    }
}
