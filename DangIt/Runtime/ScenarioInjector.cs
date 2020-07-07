﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace nsDangIt
{
    using static nsDangIt.DangIt;

    /*
     * Check that the current game contains the runtime ScenarioModule
     * and add it if it's missing.
     * 
     * Original code by TaranisElsu in TAC Life Support:
     * https://github.com/taraniselsu/TacLifeSupport
     */
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class DangItScenarioInjector : MonoBehaviour
    {
        void Start()
        {
            var game = HighLogic.CurrentGame;
            ProtoScenarioModule psm = game.scenarios.Find(s => s.moduleName == typeof(DangIt).Name);

            if (psm == null)
            {
                Log.Info("[DangIt]: Adding the controller to the game.");
                psm = game.AddProtoScenarioModule(typeof(DangIt), GameScenes.EDITOR,
                                                                  GameScenes.FLIGHT,
                                                                  GameScenes.SPACECENTER,
                                                                  GameScenes.TRACKSTATION);
            }
            else // make sure the scenario is targeting all the scenes
            {
                Log.Info("[DangIt]: The runtime is already installed (OK).");

                SetTargetScene(psm, GameScenes.EDITOR);
                SetTargetScene(psm, GameScenes.FLIGHT);
                SetTargetScene(psm, GameScenes.SPACECENTER);
                SetTargetScene(psm, GameScenes.TRACKSTATION);
            }
        }

       
        private static void SetTargetScene(ProtoScenarioModule psm, GameScenes scene)
        {
            if (!psm.targetScenes.Any(s => s == scene))
            {
                psm.targetScenes.Add(scene);
            }
        }

    }


}