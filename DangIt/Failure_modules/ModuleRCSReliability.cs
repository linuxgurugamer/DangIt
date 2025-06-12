using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nsDangIt
{
    using static nsDangIt.DangIt;

    class ModuleRCSReliability : FailureModule
    {
        ModuleRCS rcsModule;

        #region NO_LOCALIZATION
        public override string DebugName { get { return "DangItRCS"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_122"); } }
        public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_123"); } }
        public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_124"); } }
        public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_125"); } }
        public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_126"); } }
        public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_127"); } }
		public override string ExtraEditorInfo{ get { return Localizer.Format("#LOC_DangIt_128"); } }


        public override bool PartIsActive()
        {
            // A thruster block is active if any of the thrusters is firing
            return rcsModule.thrustForces.Max() > (0.1 * rcsModule.thrusterPower);
        }


        protected override void DI_Start(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                rcsModule = this.part.Modules.OfType<ModuleRCS>().First();         
            }
        }

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams2>().AllowRCSFailures;
        }

        protected override bool DI_FailBegin()
        {
            return DI_AllowedToFail();
            // Can always fail
            // return true;

        }

        protected override void DI_Disable()
        {
            rcsModule.enabled = false;
        }


        protected override void DI_EvaRepair()
        {
            rcsModule.enabled = true;           
        }

    }
}
