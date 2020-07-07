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

		public override string DebugName { get { return "DangItWheel_Motor"; } }
		public override string ScreenName { get { return "Motor"; } }
		public override string FailureMessage { get { return "A motor burnt out!"; } }
		public override string RepairMessage { get { return "Motor replaced."; } }
		public override string FailGuiName { get { return "Burn out motor"; } }
		public override string EvaRepairGuiName { get { return "Replace motor"; } }
		public override string MaintenanceString { get { return "Clean Motor"; } }
		public override string ExtraEditorInfo{ get { return "This part's motor can burn out if it fails"; } }


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

	}
}
