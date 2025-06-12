using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ippo;
using UnityEngine;
using KSP;

namespace ippo
{
	public class ModuleIntakeReliabilityCore : ippo.FailureModule //Renamed so that it dosen't conflict if user has an old version of Entropy
	{
		ModuleResourceIntake intake;

		public override string DebugName { get { return Localizer.Format("#LOC_DangIt_109"); } }
		public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_109"); } }
		public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_110"); } }
		public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_111"); } }
		public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_112"); } }
		public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_113"); } }
		public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_114"); } }
		public override string ExtraEditorInfo{ get { return Localizer.Format("#LOC_DangIt_115"); } }

		public override bool PartIsActive()
		{
			// A intake is active if its not landed and in atmosphere
			return !part.vessel.LandedOrSplashed & part.vessel.atmDensity>0 & intake.intakeEnabled;
		}

		protected override void DI_Start(StartState state)
		{
			intake = this.part.Modules.OfType<ModuleResourceIntake>().Single();
		}

		protected override bool DI_FailBegin()
		{
			return true;
		}

		protected override void DI_Disable()
		{
			intake.enabled = false;
		}


		protected override void DI_EvaRepair()
		{
			intake.enabled = true;           
		}
	}
}

