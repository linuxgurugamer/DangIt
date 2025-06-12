using KSP.Localization;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using nsDangIt;
using UnityEngine;

namespace nsDangIt
{
	using static nsDangIt.DangIt;

	class ModuleSmallTankExploder : FailureModule
	{
        #region NO_LOCALIZATION
        public override string DebugName { get { return "EntropyExploder"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_216"); } }
		public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_217"); } }
		public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_218"); } }
		public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_219"); } }
		public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_220"); } }
		public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_221"); } }

		public override bool PartIsActive()
		{
			// A tank is active if it's not empty
			return part.GetResourceMass()>0.1;
		}

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams3>().AllowSmallTankFailures;
        }

        protected override bool DI_FailBegin()
		{
            return DI_AllowedToFail();
            //return true;
		}

		protected override void DI_Disable()
		{
			part.explode ();
		}

		IEnumerator WaitAndPrint() {
			yield return new WaitForSeconds(20);
			if (HasFailed == true) {
				part.explode ();
			}
		}

		protected override void DI_EvaRepair()
		{}
	}
}
