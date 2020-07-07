using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nsDangIt
{
    using static nsDangIt.DangIt;

    /// <summary>
    /// Constants related to the spare parts resource
    /// </summary>
    public static class Spares
    {
        /// <summary>
        /// Maximum amount that a kerbal can carry
        /// </summary>
        public static readonly double MaxEvaAmount = 15f;

        /// <summary>
        /// Resource name as a string
        /// </summary>
        public static  string Name
        {
            get { if (DangIt.Instance != null) return DangIt.Instance.CurrentSettings.GetSparesResource();  return "SpareParts"; }
        }

        public static readonly float MinIncrement = 1f;
    }

}
