using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace nsDangIt
{

	using static nsDangIt.DangIt;

	public class ModuleWheelMotorReliability : FailureModule
	{
		ModuleWheels.ModuleWheelMotor wheelMotor;

        #region NO_LOCALIZATION
        public override string DebugName { get { return "DangItWheel_Motor"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_159"); } }
		public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_160"); } }
		public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_161"); } }
		public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_162"); } }
		public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_163"); } }
		public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_164"); } }
		public override string ExtraEditorInfo{ get { return Localizer.Format("#LOC_DangIt_165"); } }


		public override bool PartIsActive()
		{
			return this.part.GroundContact;
		}


		protected override void DI_Start(StartState state)
		{
			if (HighLogic.LoadedSceneIsFlight && this.vessel != null && this.part != null && this.part.Modules != null)
			{
				var moduleWheels = this.part.Modules.OfType<ModuleWheels.ModuleWheelMotor>();
				if (moduleWheels != null && moduleWheels.Count() > 0)
					this.wheelMotor = moduleWheels.First();
			}
		}

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams3>().AllowWheelMotorFailures;
        }

        protected override bool DI_FailBegin()
		{
			return DI_AllowedToFail();
		}
                #region NO_LOCALIZATION

		protected override void DI_Disable(){
			if (this.wheelMotor == null)
				return;
			this.wheelMotor.motorEnabled = false;
            this.wheelMotor.state = ModuleWheels.ModuleWheelMotor.MotorState.Inoperable;

			foreach (BaseEvent e in this.wheelMotor.Events) {
				this.FailureLog ("[WheelMotor] e.guiName = "+e.guiName);
				if (e.guiName.EndsWith ("Motor")) {
					e.active = false;
				}
			}
		}

		protected override void DI_EvaRepair(){
			if (this.wheelMotor == null)
				return;
			this.wheelMotor.motorEnabled = true;
            this.wheelMotor.state = ModuleWheels.ModuleWheelMotor.MotorState.Idle;
            foreach (BaseEvent e in this.wheelMotor.Events) {
                if (e.guiName == "Disable Motor") {
					e.active = true;
				}
            }
        }
                #endregion

	}
}
