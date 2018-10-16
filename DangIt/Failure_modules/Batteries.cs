using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace nsDangIt
{
    public class ModuleBatteryReliability : FailureModule
    {
        // Unlike other failure modules, batteries are not PartModules
        // We just need a reference to the ElectricCharge resource to simulate a battery short
        protected PartResource battery;
        int batteryResIdx;

        public override string DebugName { get { return "DangItBattery"; } }
        public override string ScreenName { get { return "Battery"; } }
        public override string FailureMessage { get { return "A battery has short-circuited!"; } }
        public override string RepairMessage { get { return "Battery repaired."; } }
        public override string FailGuiName { get { return "Fail battery"; } }
        public override string EvaRepairGuiName { get { return "Repair battery"; } }
        public override string MaintenanceString { get { return "Replace battery"; } }
		public override string ExtraEditorInfo{ get { return "This part can lose all electric charge if it fails"; } }

        
        protected override void DI_Start(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                for (int idx = 0; idx < part.Resources.Count; idx++)
                {
                    PartResource pr = part.Resources[idx];
                    if (pr.resourceName == "ElectricCharge")
                    {
                        //battery = part.Resources[idx];
                        batteryResIdx = idx;
                        return;
                    }
                }
                throw new Exception("No ElectricCharge was found in the part!");
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
