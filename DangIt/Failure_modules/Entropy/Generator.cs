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

	public class ModuleGeneratorReliability : FailureModule
	{
		ModuleGenerator generator;
        float initialEfficiency = 0f;

        #region NO_LOCALIZATION
        public override string DebugName { get { return "ModuleGenerator"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_202"); } }
		public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_203"); } }
		public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_204"); } }
		public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_205"); } }
		public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_206"); } }
		public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_207"); } }
		public override string ExtraEditorInfo{ get { return Localizer.Format("#LOC_DangIt_208"); } }

		public override bool PartIsActive()
		{
			// It's active when its active (duh!)
			return generator.generatorIsActive;
		}

		protected override void DI_Start(StartState state)
		{
			generator = this.part.Modules.OfType<ModuleGenerator>().First();
            initialEfficiency = generator.efficiency;
		}

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams3>().AllowGeneratorFailures;
        }

        protected override bool DI_FailBegin()
		{
            return DI_AllowedToFail();
            //return true;
		}

		protected override void DI_Disable()
		{
            generator.efficiency /= 2;
            //generator.outputList.ForEach (r => r.rate /= 2);
        }


		protected override void DI_EvaRepair()
		{
            generator.efficiency *= 2;
            if (generator.efficiency > initialEfficiency)
                generator.efficiency = initialEfficiency;
            //generator.outputList.ForEach (r => r.rate *= 2);
        }

		protected override void DI_Update(){
			if (this.HasFailed) {
				generator.efficiency /= 2;
			}
		}
	}
}

