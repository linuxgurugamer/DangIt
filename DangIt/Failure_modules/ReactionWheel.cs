using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nsDangIt
{
    using static nsDangIt.DangIt;

    public class ModuleReactionWheelReliability : FailureModule
    {
        ModuleReactionWheel torqueModule;

        #region NO_LOCALIZATION
        public override string DebugName { get { return "DangItReactionWheel"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_129"); } }
        public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_130"); } }
        public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_131"); } }
        public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_132"); } }
        public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_133"); } }
        public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_134"); } }
		public override string ExtraEditorInfo{ get { return Localizer.Format("#LOC_DangIt_135"); } }


        public override bool PartIsActive()
        {
            // A reaction wheel is always spinning
            // Unless the user has turned it off, consider it active even if there's no torque
            return (torqueModule.isEnabled &&
                    torqueModule.wheelState == ModuleReactionWheel.WheelState.Active);
        }


        protected override void DI_Start(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                this.torqueModule = this.part.Modules.OfType<ModuleReactionWheel>().First();            
            }
        }

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams2>().AllowReactionWheelFailures;
        }

        protected override bool DI_FailBegin()
        {
            return DI_AllowedToFail();
            // Can always fail
            // return true;

        }


        protected override void DI_Disable()
        {
            this.torqueModule.OnToggle();
            this.torqueModule.isEnabled = false;
            this.torqueModule.Events["OnToggle"].active = false;    // hides the ability to turn it back on from the user
            this.torqueModule.wheelState = ModuleReactionWheel.WheelState.Broken;
        }


        protected override void DI_EvaRepair()
        {
            this.torqueModule.isEnabled = true;
            this.torqueModule.Events["OnToggle"].active = true;
            this.torqueModule.wheelState = ModuleReactionWheel.WheelState.Active;
        }

    }
}
