using KSP.Localization;
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

        #region NO_LOCALIZATION
        public override string DebugName { get { return "DangItDeployableAntenna"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_46"); } }
        public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_47"); } }
        public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_48"); } }
        public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_49"); } }
        public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_50"); } }
        public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_50"); } }
        public override string ExtraEditorInfo { get { return Localizer.Format("#LOC_DangIt_51"); } }


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
