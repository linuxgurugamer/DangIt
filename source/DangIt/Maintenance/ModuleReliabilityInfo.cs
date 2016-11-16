using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ippo
{
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
            
            Logger.Info("ModuleReliabilityInfo.GetInfo");
			if (fails.Count == 0)   // no failure module, return a placeholder message
                return "This part has been built to last";
            else
            {
                StringBuilder sb = new StringBuilder();

                Logger.Info("ModuleReliabilityInfo, part: " + part.partInfo.title);
                
                foreach (FailureModule fm in fails)
                {
                    Logger.Info("failureModule: " + fm.name);
                    Logger.Info("Lifetime: " + fm.LifeTime.ToString());
                    Logger.Info("MTBF: " + fm.MTBF.ToString());

                    float mtbfMultipler = 1.0f;
                    float lifetimeMultiplier = 1f;
                    if (HighLogic.CurrentGame != null && HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams1>() != null)
                    {
                        mtbfMultipler = HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams1>().MTBF_Multiplier;
                        lifetimeMultiplier = HighLogic.CurrentGame.Parameters.CustomParams<DangItCustomParams1>().Lifetime_Multiplier;
                    }

                    double EOL = Math.Round (Math.Max (-fm.LifeTime * lifetimeMultiplier * Math.Log (1 / fm.MTBF * mtbfMultipler ), 0));

                    Logger.Info("EOL: " + EOL.ToString());
					sb.AppendLine (fm.ScreenName);
					sb.AppendLine (" - MTBF: " + fm.MTBF * mtbfMultipler  + " hours");
					sb.AppendLine (" - Lifetime: " + fm.LifeTime * lifetimeMultiplier + " hours");
					sb.AppendLine (" - EOL : " + EOL + " hours");
					sb.AppendLine (" - Repair cost: " + fm.RepairCost);
					sb.AppendLine (" - Priority: " + fm.Priority);

					if (fm.ExtraEditorInfo != "") {
						sb.AppendLine (" - " + fm.ExtraEditorInfo); //Append any extra info the module wants to add
					}

					if (!string.IsNullOrEmpty (fm.PerksRequirementName)) {
						sb.AppendLine (string.Format (
							" - Servicing requires a level {0} {1}",
							fm.PerksRequirementValue,
							fm.PerksRequirementName));
					} else {
						sb.AppendLine (" - No special requirements for servicing");
					}

					sb.AppendLine ();
                }

                return sb.ToString();
            }

        }
    }
}
