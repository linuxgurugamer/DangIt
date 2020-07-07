﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nsDangIt
{
    using static nsDangIt.DangIt;

    class ModuleRCSReliability : FailureModule
    {
        ModuleRCS rcsModule;

        public override string DebugName { get { return "DangItRCS"; } }
        public override string ScreenName { get { return "RCS Thruster"; } }
        public override string FailureMessage { get { return "A thruster has stopped thrusting!"; } }
        public override string RepairMessage { get { return "Thruster back online."; } }
        public override string FailGuiName { get { return "Fail thruster"; } }
        public override string EvaRepairGuiName { get { return "Repair thruster"; } }
        public override string MaintenanceString { get { return "Clean thruster"; } }
		public override string ExtraEditorInfo{ get { return "This part's thrusters can stop firing if it fails"; } }


        public override bool PartIsActive()
        {
            // A thruster block is active if any of the thrusters is firing
            return rcsModule.thrustForces.Max() > (0.1 * rcsModule.thrusterPower);
        }


        protected override void DI_Start(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                rcsModule = this.part.Modules.OfType<ModuleRCS>().First();         
            }
        }

        protected override bool DI_AllowedToFail()
        {
            return HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams2>().AllowRCSFailures;
        }

        protected override bool DI_FailBegin()
        {
            return DI_AllowedToFail();
            // Can always fail
            // return true;

        }

        protected override void DI_Disable()
        {
            rcsModule.enabled = false;
        }


        protected override void DI_EvaRepair()
        {
            rcsModule.enabled = true;           
        }

    }
}
