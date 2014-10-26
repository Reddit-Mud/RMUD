using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public static partial class Mud
    {
        public static Random Random = new Random();

        public static T ChooseAtRandom<T>(List<T> From)
        {
            return From[Random.Next(From.Count)];
        }

        public static T ChooseAtRandom<T>(params T[] From)
        {
            return From[Random.Next(From.Length)];
        }
    }
}
