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

	public class ModuleParachuteReliability : FailureModule
	{
		ModuleParachute chute;
		bool canFail;

        #region NO_LOCALIZATION
        public override string DebugName { get { return "Canopy_stock_chutes"; } }
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
			if (chute == null)
				return false;
			// A chute is active if its not stowed
			return chute.deploymentState != ModuleParachute.deploymentStates.STOWED;
		}

		protected override void DI_Start(StartState state)
		{
			if (this.vessel == null)
				return;
			chute = this.part.Modules.OfType<ModuleParachute>().FirstOrDefault();

			if (HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams3>().Allow1ParachuteFailures == false)
            {
				foreach (Part part_each in this.vessel.Parts)
                {
					//Make sure that there is at least one other chute on the craft!
					if (part_each != this.part)
                    {
						foreach (PartModule module_each in part_each.Modules)
                        {
                            if (module_each is ModuleParachute)
                            {
                                this.canFail = true;
                            }
                        }
                    }
                }
            }
            else
                this.canFail = true;
        }

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams3>().AllowParachuteFailures;
        }

        protected override bool DI_FailBegin()
		{
            return DI_AllowedToFail() & canFail;
            //return canFail;
		}

		protected override void DI_Disable()
		{
			;
		}


		protected override void DI_EvaRepair()
		{ 
			;
		}

		protected override void DI_Update(){
			if (this.HasFailed) {
				chute.deploymentState = ModuleParachute.deploymentStates.SEMIDEPLOYED;
				chute.Deploy ();
			}
		}
	}
}

