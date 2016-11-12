using System;
using System.Collections;
using System.Linq;
using System.Text;
using ippo;
using UnityEngine;

namespace ippo
{
	class ModuleSmallTankExploder : FailureModule
	{
		public override string DebugName { get { return "EntropyExploder"; } }
		public override string ScreenName { get { return "Pressure Vessel"; } }
		public override string FailureMessage { get { return "MAIN BUS UNDERVOLT: 20s to repair"; } }
		public override string RepairMessage { get { return "Tank reparied"; } }
		public override string FailGuiName { get { return "Fail Sheilding"; } }
		public override string EvaRepairGuiName { get { return "Repair tank plating"; } }
		public override string MaintenanceString { get { return "Reinforce tank plating"; } }

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
