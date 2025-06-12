using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ippo
{
	public class ModuleDecouplerReliability : FailureModule
	{
		DecouplerManager manager;

		[KSPField(isPersistant = true, guiActive = false)]
		float origPercentage = -1f;

		public override string DebugName { get { return Localizer.Format("#LOC_DangIt_37"); } }
		public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_38"); } }
		public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_39"); } }
		public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_40"); } }
		public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_41"); } }
		public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_42"); } }
		public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_43"); } }
		public override string ExtraEditorInfo{ get { return Localizer.Format("#LOC_DangIt_44"); } }

		public override bool PartIsActive()
		{
			return !InputLockManager.lockStack.Keys.Contains(Localizer.Format("#LOC_DangIt_45"));
		}


		protected override void DI_Start(StartState state)
		{
			if (HighLogic.LoadedSceneIsFlight)
			{
				this.manager = new DecouplerManager (this.part);
			}
		}


		protected override bool DI_FailBegin()
		{
			// Can always fail
			return true;
		}

		protected override void DI_Disable()
		{
			this.origPercentage = this.manager.ejectionForcePercent;
			this.manager.ejectionForcePercent = 0;
		}

		protected override void DI_EvaRepair()
		{
			this.manager.ejectionForcePercent = this.origPercentage;
		}

	}
}
