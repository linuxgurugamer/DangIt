using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nsDangIt
{
    using static nsDangIt.DangIt;

    public class ModuleLightsReliability : FailureModule
    {
        ModuleLight lightModule;

        #region NO_LOCALIZATION
        public override string DebugName { get { return "DangItLights"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_116"); } }
        public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_117"); } }
        public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_118"); } }
        public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_119"); } }
        public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_120"); } }
        public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_120"); } }
		public override string ExtraEditorInfo{ get { return Localizer.Format("#LOC_DangIt_121"); } }


        public override bool PartIsActive()
        {
            return this.lightModule.isOn;
        }


        protected override void DI_Start(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                this.lightModule = this.part.Modules.OfType<ModuleLight>().First();
            }
        }

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams2>().AllowLightFailures;
        }

        protected override bool DI_FailBegin()
        {
            return DI_AllowedToFail();
            // Can always fail
            // return true;

        }

        protected override void DI_Disable()
        {
            this.lightModule.LightsOff();

            // The module needs to be entirely removed from the part,
            // since setting enabled = false still makes it respond to the
            // master light switch at the top of the screen.
            this.part.Modules.Remove(this.lightModule);
        }


        protected override void DI_EvaRepair()
        {
            this.part.Modules.Add(this.lightModule); 
        }

    }
}
