using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nsDangIt
{
    using static nsDangIt.DangIt;

    public class ModuleGimbalReliability : FailureModule
    {
        ModuleGimbal gimbalModule;
        EngineManager engineManager;

        #region NO_LOCALIZATION
        public override string DebugName { get { return "DangItGimbal"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_101"); } }
        public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_102"); } }
        public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_103"); } }
        public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_104"); } }
        public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_105"); } }
        public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_106"); } }
		public override string ExtraEditorInfo{ get { return Localizer.Format("#LOC_DangIt_107"); } }


        public override bool PartIsActive()
        {
            // The gimbal is considered active only when the engine is
            // TODO: this should be tied to the actual deflection of the engine
            return this.engineManager.IsActive;
        }


        protected override void DI_Start(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                this.gimbalModule = this.part.Modules.OfType<ModuleGimbal>().First();
                this.engineManager = new EngineManager(this.part);
            }
        }

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams2>().AllowGimbalFailures;
        }

        protected override bool DI_FailBegin()
        {
            return DI_AllowedToFail();
            // Can always fail
            // return true;

        }

        protected override void DI_Disable()
        {
            // Disable the gimbal module
			this.gimbalModule.gimbalLock = true;
			this.gimbalModule.Fields [Localizer.Format("#LOC_DangIt_108")].guiActive = false;
        }


        protected override void DI_EvaRepair()
        {
            // Restore the gimbaling module
			this.gimbalModule.gimbalLock = false;
			this.gimbalModule.Fields [Localizer.Format("#LOC_DangIt_108")].guiActive = true;
        }

    }
}
