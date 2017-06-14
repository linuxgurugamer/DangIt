using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using KSP.UI.Screens;

namespace nsDangIt
{
    public partial class DangIt
    {
		/// <summary>
		/// Converts a string representing a priority to a int representing it
		/// </summary>
		/// <returns>The int representation</returns>
		/// <param name="modeString">Priority string</param>
		public static int PriorityIntFromString(string modeString)
		{
			Logger.Info("[DangIt] [Static] Translating '" + modeString + "' to int...");
			var keys = new List<string> ();
			keys.Add ("LOW");
			keys.Add ("MEDIUM");
			keys.Add ("HIGH");
			return keys.IndexOf (modeString.ToUpper ())+1; //+1 so that LOW = 1
		}

        /// <summary>
        /// Returns the in-game universal time
        /// </summary>
        public static float Now()
        {
            return (float)Planetarium.GetUniversalTime();
        }


        /// <summary>
        /// Returns the full path to a given file in the configuration folder.
        /// Likely, GameData/DangIt/PluginData/DangIt/ + filename
        /// </summary>
        internal static string aGetConfigFilePath(string fileName)
        {
            return "GameData/PluginData/" +  fileName;
        }
     

        /// <summary>
        /// Adds a new entry to the flight events log.
        /// Automatically adds the MET at the beginning of the log
        /// </summary>
        public static void FlightLog(string msg)
        {
            FlightLogger.eventLog.Add("[" + KSPUtil.PrintTimeStamp(FlightLogger.met) + "]: " + msg);
        }




        



        /// <summary>
        /// Broadcasts a message at the top-center of the screen
        /// The message is ignored if the settings have disabled messages, unless
        /// overrideMute is true
        /// </summary>
        public static void Broadcast(string message, bool overrideMute = false, float time = 5f)
        {
            if (overrideMute || DangIt.Instance.CurrentSettings.Messages)
                ScreenMessages.PostScreenMessage(message, time, ScreenMessageStyle.UPPER_CENTER);
        }


        /// <summary>
        /// Posts a new message to the messaging system unless notifications have been disabled in the general settings.
        /// </summary>
        public static void PostMessage(string title, string message, MessageSystemButton.MessageButtonColor messageButtonColor, MessageSystemButton.ButtonIcons buttonIcons,
            bool overrideMute = false)
        {
            if (DangIt.Instance.CurrentSettings.Messages || overrideMute)
            {
                MessageSystem.Message msg = new MessageSystem.Message(
                        title,
                        message,
                        messageButtonColor,
                        buttonIcons);
                MessageSystem.Instance.AddMessage(msg); 
            }

        }



        /// <summary>
        /// Tries to parse a string and convert it to the type T.
        /// If the string is empty or an exception is raised it returns the
        /// specified default value.
        /// </summary>
        public static T Parse<T>(string text, T defaultTo)
        {
            try
            {
                return (String.IsNullOrEmpty(text) ? defaultTo : (T)Convert.ChangeType(text, typeof(T)));
            }
            catch
            {
                return defaultTo;
            }
        }



        /// <summary>
        /// Finds the active EVA vessel and returns its root part, or null if no EVA is found.
        /// </summary>
        public static Part FindEVAPart()
        {
            int idx = FlightGlobals.Vessels.FindIndex(v => ((v.vesselType == VesselType.EVA) && v.isActiveVessel));
            return ((idx < 0) ? null : FlightGlobals.Vessels[idx].rootPart);
        }

    }
}
