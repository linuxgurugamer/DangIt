using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nsDangIt;
using UnityEngine;
using KSP;

namespace nsDangIt
{
	using static nsDangIt.DangIt;

	public class ModuleAnimationReliability : FailureModule
	{
		ModuleAnimateGeneric bay;

        #region NO_LOCALIZATION
        public override string DebugName { get { return "ModuleAnimateGeneric"; } }
		#endregion
		public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_159"); } }
		public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_196"); } }
		public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_197"); } }
		public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_198"); } }
		public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_199"); } }
		public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_200"); } }
		public override string ExtraEditorInfo{ get { return Localizer.Format("#LOC_DangIt_201"); } }

		public override bool PartIsActive()
		{
			// It's only active if its not on the ground
			return !part.vessel.LandedOrSplashed;
		}

		protected override void DI_Start(StartState state)
		{
			bay = this.part.Modules.OfType<ModuleAnimateGeneric>().First();
		}

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams3>().AllowAnimateFailures;
        }

        protected override bool DI_FailBegin()
		{
            return DI_AllowedToFail();

            //return true;
		}

		protected override void DI_Disable()
		{
			bay.allowManualControl = false;
			bay.enabled = false;

		}


		protected override void DI_EvaRepair()
		{
			bay.allowManualControl = true;
			bay.enabled = true; 
		}
	}
}

