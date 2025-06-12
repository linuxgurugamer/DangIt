using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ippo;
using UnityEngine;
using KSP;
using RealChute;

namespace coffeeman
{
	public class ModuleRealChutesReliability : ippo.FailureModule
	{
		RealChuteModule chute;
		bool canFail;

        #region NO_LOCALIZATION
        public override string DebugName { get { return "Canopy_realchutes_chutes"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_209"); } }
		public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_210"); } }
		public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_211"); } }
		public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_212"); } }
		public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_213"); } }
		public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_214"); } }
		public override string ExtraEditorInfo{ get { return Localizer.Format("#LOC_DangIt_215"); } }

		public override bool PartIsActive()
		{
			// A chute is active if its not stowed
			return chute.anyDeployed;
		}

		protected override void DI_Start(StartState state)
		{
			chute = this.part.Modules.OfType<RealChuteModule>().Single();

			foreach ( Part part_each in this.part.vessel.Parts){ //Make sure that there is at least one other chute on the craft!
				if (part_each != this.part) {
					foreach (PartModule module_each in part_each.Modules) {
						if (module_each is ModuleParachute || module_each is RealChuteModule) {
							this.canFail = true;
						}
					}
				}
			}
		}

		protected override bool DI_FailBegin()
		{
			return canFail;
		}

		protected override void DI_Disable()
		{
			chute.parachutes.ForEach (p => p.deployedDiameter /= 2);
			chute.parachutes.ForEach (p => p.preDeployedDiameter /= 2);

		}


		protected override void DI_EvaRepair()
		{ 
			chute.parachutes.ForEach (p => p.deployedDiameter *= 2);
			chute.parachutes.ForEach (p => p.preDeployedDiameter *= 2);
		}
	}
}

