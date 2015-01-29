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

    public partial class MudObject
    {
        /// <summary>
        /// This should factor in the time of day, and the phase of the moon if at night, to determine if there is adequate lighting for exterior rooms to be visible.
        /// </summary>
        public static LightingLevel AmbientExteriorLightingLevel { get { return LightingLevel.Bright; } }

        public static bool IsDay { get { return true; } }
        public static bool IsNight { get { return false; } }

        public static DateTime TimeOfDay = DateTime.Parse("03/15/2015 11:15:00 -5:00");
    }
}
