using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nsDangIt
{
	using static nsDangIt.DangIt;

	public class ModuleWheelTireReliability : FailureModule
	{
		//ModuleWheelBase wheelBase;
        ModuleWheels.ModuleWheelDamage wheelDamage;

        #region NO_LOCALIZATION
        public override string DebugName { get { return "DangItWheel_Tire"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_166"); } }
		public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_167"); } }
		public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_168"); } }
		public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_169"); } }
		public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_170"); } }
		public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_171"); } }
		public override string ExtraEditorInfo{ get { return Localizer.Format("#LOC_DangIt_172"); } }


		public override bool PartIsActive()
		{
			return this.part.GroundContact;
		}


		protected override void DI_Start(StartState state)
		{
			if (HighLogic.LoadedSceneIsFlight)
			{
				//this.wheelBase = this.part.Modules.OfType<ModuleWheelBase>().First();
                wheelDamage = part.Modules.OfType<ModuleWheels.ModuleWheelDamage>().First();

            }
            
			//if (!this.wheel.damageable) {
			//	this.enabled = false;
			//}
		}

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams3>().AllowWheelTireFailures;
        }

        protected override bool DI_FailBegin()
		{
			return DI_AllowedToFail();
		}

		protected override void DI_Disable()
		{
            wheelDamage.SetDamaged(true);
			//this.wheel.isDamaged = true; //Do we need this?
			//this.wheel.wheels [0].damageWheel ();

			Events ["Maintenance"].active = false; //We should repair with the ModuleWheel UI option
			Events ["EvaRepair"].active = false;
			Events ["Fail"].active = false;
		}

		protected override void DI_Update(){
		}

		protected override void DI_EvaRepair()
		{
            if (wheelDamage.isRepairable)
                wheelDamage.SetDamaged(false);
        }

	}
}
