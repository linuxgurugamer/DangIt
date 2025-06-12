using KSP.Localization;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using nsDangIt;
using UnityEngine;
using KSP;

namespace nsDangIt
{
	using static nsDangIt.DangIt;

	public class ModuleCoolantReliabilityCore : FailureModule //Renamed so that it dosen't conflict if user has an old version of Entropy
	{
		EngineManager engines;

        #region NO_LOCALIZATION
        public override string DebugName { get { return "Coolant Line"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_31"); } }
		public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_32"); } }
		public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_33"); } }
		public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_34"); } }
		public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_35"); } }
		public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_35"); } }
		public override string ExtraEditorInfo{ get { return Localizer.Format("#LOC_DangIt_36"); } }

		public override bool PartIsActive()
		{
			return this.engines.IsActive;
		}

		protected override void DI_Start(StartState state)
		{
			if (HighLogic.LoadedSceneIsFlight)
			{
				// An engine might actually be two engine modules (e.g: SABREs)
				this.engines = new EngineManager(this.part);
			}
		}

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams2>().AllowCoolantFailures;
        }

        protected override bool DI_FailBegin()
		{
            return DI_AllowedToFail();

        }

		protected override void DI_Disable()
		{
			this.engines.engines.ForEach (e => e.heatProduction *= 3);
			this.engines.enginesFX.ForEach (e => e.heatProduction *= 3);
		}

		protected override void DI_EvaRepair(){
			this.engines.engines.ForEach (e => e.heatProduction /= 3);
			this.engines.enginesFX.ForEach (e => e.heatProduction /= 3);
		}

		protected override void DI_Update(){}
	}
}

