using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nsDangIt
{
    using static nsDangIt.DangIt;

    /// <summary>
    /// Module that produces the reliability info about a part to display in the VAB / SPH info tab.
    /// It aggregates the information from all the failure modules into one, instead of many separate tabs.
    /// </summary>
    public class ModuleReliabilityInfo : PartModule
    {

        public override string GetInfo()
        {
			List<FailureModule> raw_fails = this.part.Modules.OfType<FailureModule>().ToList();

			List<FailureModule> fails = new List<FailureModule>();

			foreach (FailureModule fm in raw_fails) {
				if (fm.DI_ShowInfoInEditor ()) { //Make sure the module wants to show info in the editor
					fails.Add (fm);
				}
			}
            
            Log.Info("ModuleReliabilityInfo.GetInfo");
			if (fails.Count == 0)   // no failure module, return a placeholder message
                return Localizer.Format("#LOC_DangIt_174");
            else
            {
                StringBuilder sb = new StringBuilder();

                Log.Info("ModuleReliabilityInfo, part: " + part.partInfo.title);
                
                foreach (FailureModule fm in fails)
                {
                    Log.Info("failureModule: " + fm.name);
                    Log.Info("Lifetime: " + fm.LifeTime.ToString());
                    Log.Info("MTBF: " + fm.MTBF.ToString());

                    float mtbfMultipler = 1.0f;
                    float lifetimeMultiplier = 1f;
                    if (HighLogic.CurrentGame != null && HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams1>() != null)
                    {
                        mtbfMultipler = HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams1>().MTBF_Multiplier;
                        lifetimeMultiplier = HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams1>().Lifetime_Multiplier;
                    }

                    double EOL = Math.Round (Math.Max (-fm.LifeTime * lifetimeMultiplier * Math.Log (1 / fm.MTBF * mtbfMultipler ), 0));

                    Log.Info("EOL: " + EOL.ToString());
					sb.AppendLine (fm.ScreenName);
					sb.AppendLine (Localizer.Format("#LOC_DangIt_175") + fm.MTBF * mtbfMultipler  + Localizer.Format("#LOC_DangIt_176"));
					sb.AppendLine (Localizer.Format("#LOC_DangIt_177") + fm.LifeTime * lifetimeMultiplier + Localizer.Format("#LOC_DangIt_176"));
					sb.AppendLine (Localizer.Format("#LOC_DangIt_178") + EOL + Localizer.Format("#LOC_DangIt_176"));
					sb.AppendLine (Localizer.Format("#LOC_DangIt_179") + fm.RepairCost);
					sb.AppendLine (Localizer.Format("#LOC_DangIt_180") + fm.Priority);

					if (fm.ExtraEditorInfo != "") {
						sb.AppendLine (" - " + fm.ExtraEditorInfo); //Append any extra info the module wants to add
					}

					if (!string.IsNullOrEmpty (fm.PerksRequirementName)) {
						sb.AppendLine (string.Format (
							 Localizer.Format("#LOC_DangIt_181") + " {0} " +"{1}",
							fm.PerksRequirementValue,
							fm.PerksRequirementName));
					} else {
						sb.AppendLine (Localizer.Format("#LOC_DangIt_182"));
					}

					sb.AppendLine ();
                }

                return sb.ToString();
            }

        }
    }
}
