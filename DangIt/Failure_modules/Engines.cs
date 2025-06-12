using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nsDangIt
{
    using static nsDangIt.DangIt;

    public class ModuleEngineReliability : FailureModule
    {
		EngineManager engines;
		ModuleSurfaceFX surfaceFX;

		[KSPField(isPersistant = true, guiActive = false)]
		float oldSurfaceFXMaxDistance = -1f;

        #region NO_LOCALIZATION
        public override string DebugName { get { return "DangItEngines"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_52"); } }
        public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_53"); } }
        public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_54"); } }
        public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_55"); } }
        public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_56"); } }
        public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_57"); } }
		public override string ExtraEditorInfo{ get { return Localizer.Format("#LOC_DangIt_58"); } }


        protected override float LambdaMultiplier()
        {
            // Engines are designed to operate at max throttle
            // this introduces a heavy penalty for low throttle values
            float x = this.engines.CurrentThrottle;
            return (5 - x);
        }


        public override bool PartIsActive()
        {
            return this.engines.IsActive;
        }


        protected override void DI_Start(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                // An engine might actually be two engine modules (e.g: SABREs)
                this.engines = new EngineManager(this.part);
				// Catch if the part has a ModuleSurfaceFX
				if (this.part.Modules.OfType<ModuleSurfaceFX> ().Any ()) {
					surfaceFX = this.part.Modules.OfType<ModuleSurfaceFX>().First();
				}
            }
        }

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams2>().AllowEngineFailures;
        }

        protected override bool DI_FailBegin()
        {
            return DI_AllowedToFail();
            // Can always fail
            // return true;

        }

        protected override void DI_Disable()
        {
            this.engines.Disable();
			if (this.surfaceFX){ // If we have a ModuleSurfaceFX, cache it's old max distance and set it to -1 to block its firing
				this.oldSurfaceFXMaxDistance = this.surfaceFX.maxDistance;
				this.surfaceFX.maxDistance = -1;
			}
        }

        protected override void DI_EvaRepair()
        {
            this.engines.Enable();
			if (this.surfaceFX) { // Reenable FX
				this.surfaceFX.maxDistance = this.oldSurfaceFXMaxDistance;
			}
        }

    }
}
