using KSP.Localization;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using nsDangIt;
using UnityEngine;
using KSP;

namespace nsDangIt
{
	using static nsDangIt.DangIt;

	public class ModuleSolarReliability : FailureModule
	{
		ModuleDeployableSolarPanel panel;

        #region NO_LOCALIZATION
        public override string DebugName { get { return "Tracking Servo"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_222"); } }
		public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_223"); } }
		public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_224"); } }
		public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_225"); } }
		public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_226"); } }
		public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_227"); } }
		public override string ExtraEditorInfo { get { return Localizer.Format("#LOC_DangIt_228"); } }

		public override bool PartIsActive()
		{
			// Panels are active if deployed AND TRACKING
			return panel.isTracking & panel.flowRate>0;
		}

		protected override void DI_Start(StartState state)
		{
			panel = this.part.Modules.OfType<ModuleDeployableSolarPanel>().First();
			if (!panel.isTracking) {
				this.enabled = false; //Disable this if it's not tracking
			}
		}

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams3>().AllowSolarPanelFailures;
        }

        protected override bool DI_FailBegin()
		{
            return DI_AllowedToFail() & PartIsActive();
            //return true;
		}       

        protected override void DI_Disable()
		{
			panel.isTracking = false;
        }

		protected override void DI_EvaRepair()
        {
            panel.isTracking = true;
		}

		public override bool DI_ShowInfoInEditor(){          
            return this.part.Modules.OfType<ModuleDeployableSolarPanel>().First().isTracking; //Don't show for non-tracking panels
		}
	}
}

