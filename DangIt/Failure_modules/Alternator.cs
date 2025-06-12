using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nsDangIt
{
    using static nsDangIt.DangIt;

    public class ModuleAlternatorReliability : FailureModule
    {
        EngineManager engineManager;
        ModuleAlternator alternatorModule;

        #region NO_LOCALIZATION
        public override string DebugName { get { return "DangItAlternator"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_15"); } }
        public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_16"); } }
        public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_17"); } }
        public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_18"); } }
        public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_19"); } }
        public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_19"); } }
		public override string ExtraEditorInfo{ get { return Localizer.Format("#LOC_DangIt_20"); } }


        public override bool PartIsActive()
        {
            // Alternators are active when the engine is
            return engineManager.IsActive;
        }


        protected override void DI_Start(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                this.alternatorModule = this.part.Modules.OfType<ModuleAlternator>().FirstOrDefault();
                this.engineManager = new EngineManager(this.part);
            }
        }

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams2>().AllowAlternatorFailures;
        }

        protected override bool DI_FailBegin()
        {
            return DI_AllowedToFail();
            // Can always fail
//            return true;
        }


        protected override void DI_Disable()
        {
            this.alternatorModule.enabled = false;
        }


        protected override void DI_EvaRepair()
        {
            this.alternatorModule.enabled = true; 
        }

    }
}
