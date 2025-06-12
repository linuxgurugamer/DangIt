using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ferram4;
using System.Reflection;
using ippo;

namespace DangIt_FAR
{
    /// <summary>
    /// Module that causes failures in aerodynamic control surfaces
    /// </summary>
    public class ModuleFARControlSurfaceReliability : FailureModule
    {
        FARControllableSurface controlSurfaceModule;

        [KSPField(isPersistant = true, guiActive = false)]
        public bool wasFlap = true;

        FieldInfo AoAOffset;
        FieldInfo AoAFromFlap;
        

        public override string DebugName { get { return Localizer.Format("#LOC_DangIt_2"); } }
        public override string InspectionName { get { return Localizer.Format("#LOC_DangIt_3"); } }
        public override string FailureMessage { get { return Localizer.Format("#LOC_DangIt_4"); } }
        public override string RepairMessage { get { return Localizer.Format("#LOC_DangIt_5"); } }
        public override string FailGuiName { get { return Localizer.Format("#LOC_DangIt_6"); } }
        public override string EvaRepairGuiName { get { return Localizer.Format("#LOC_DangIt_7"); } }
        public override string MaintenanceString { get { return Localizer.Format("#LOC_DangIt_8"); } }


        public override bool PartIsActive()
        {
            return (this.part.vessel.atmDensity > 0);
        }

        protected override float LambdaMultiplier()
        {
            return (float)this.part.vessel.atmDensity;
        }



        protected override void DI_OnLoad(ConfigNode node)
        {
            this.wasFlap = DangIt.Parse<bool>(node.GetValue("wasFlap"), defaultTo: true);
        }


        protected override void DI_OnSave(ConfigNode node)
        {
            node.SetValue("wasFlap", this.wasFlap.ToString());
        }


        protected override void DI_Start(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                this.controlSurfaceModule = this.part.Modules.OfType<FARControllableSurface>().Single();
                this.wasFlap = controlSurfaceModule.isFlap;

                this.AoAFromFlap = typeof(FARControllableSurface).GetField(Localizer.Format("#LOC_DangIt_9"), BindingFlags.NonPublic);
                this.AoAOffset = typeof(FARControllableSurface).GetField(Localizer.Format("#LOC_DangIt_10"), BindingFlags.NonPublic);

                if (AoAOffset == null || AoAFromFlap == null)
                {
                    throw new Exception(Localizer.Format("#LOC_DangIt_11"));
                }

            }
        }


        protected override bool DI_FailBegin()
        {
            return true;
        }


        protected override void DI_Disable()
        {
            // Save the settings before overwriting them,
            // just in the case that the user has already set the control surface to ignore some direction
            this.wasFlap = this.controlSurfaceModule.isFlap;

            // Make the control surface unresponsive
            //AoAFromFlap.SetValue(this.controlSurfaceModule, AoAOffset.GetValue(this.controlSurfaceModule));
            this.controlSurfaceModule.isFlap = false;

            // Disable the module for good measure
            this.controlSurfaceModule.enabled = false; 
        }



        protected override void DI_EvaRepair()
        {
            // Enable the module
            this.controlSurfaceModule.enabled = true;

            // Re-allow control as a flap
            this.controlSurfaceModule.isFlap = true;
        }

    }


}
