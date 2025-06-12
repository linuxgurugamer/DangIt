using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace nsDangIt
{
    using static nsDangIt.DangIt;

    public class ModuleBatteryReliability : FailureModule
    {
        // Unlike other failure modules, batteries are not PartModules
        // We just need a reference to the ElectricCharge resource to simulate a battery short
        protected PartResource battery;
        int batteryResIdx;

        #region NO_LOCALIZATION
        public override string DebugName { get { return "DangItBattery"; } }
        #endregion
        public override string ScreenName { get { return Localizer.Format("#LOC_DangIt_21"); } }
        public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_22"); } }
        public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_23"); } }
        public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_24"); } }
        public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_25"); } }
        public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_26"); } }
		public override string ExtraEditorInfo{ get { return Localizer.Format("#LOC_DangIt_27"); } }

        
        protected override void DI_Start(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                for (int idx = 0; idx < part.Resources.Count; idx++)
                {
                    PartResource pr = part.Resources[idx];
                    if (pr.resourceName == Localizer.Format("#LOC_DangIt_28"))
                    {
                        //battery = part.Resources[idx];
                        batteryResIdx = idx;
                        return;
                    }
                }
                throw new Exception(Localizer.Format("#LOC_DangIt_29"));
            }
        }

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams2>().AllowBatteryFailures;
        }

        protected override bool DI_FailBegin()
        {
            return DI_AllowedToFail();
            // Can always fail
            //            return true;

        }

        protected override void DI_Disable()
        {
            // Drain all the charge and disable the flow
            // Not really realistic as short circuits go
            // TODO: improve failure model
            battery = part.Resources[batteryResIdx];
            battery.amount = 0;
            battery.flowMode = PartResource.FlowMode.None;
        }
        
        protected override void DI_EvaRepair()
        {
            battery = part.Resources[batteryResIdx];
            battery.flowMode = PartResource.FlowMode.Both;
        }

    }
}
