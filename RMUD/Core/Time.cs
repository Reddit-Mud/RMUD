using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public enum LightingLevel
    {
        Bright = 2,
        Dim = 1,
        Dark = 0
    }

    public partial class Mud
    {
        /// <summary>
        /// This should factor in the time of day, and the phase of the moon if at night, to determine if there is adequate lighting for exterior rooms to be visible.
        /// </summary>
        public static LightingLevel AmbientExteriorLightingLevel { get { return LightingLevel.Bright; } }

        public static bool IsDay { get { return true; } }
        public static bool IsNight { get { return false; } }
    }
}
