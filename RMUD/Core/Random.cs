using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public static partial class Core
    {
        private static Random _Random;
        public static Random Random
        {
            get
            {
                if (_Random == null) _Random = new Random();
                return _Random;
            }
        }
    }

    public partial class MudObject
    {
        public static Random Random { get { return Core.Random; } }

        public static T ChooseAtRandom<T>(List<T> From)
        {
            return From[Core.Random.Next(From.Count)];
        }

        public static T ChooseAtRandom<T>(params T[] From)
        {
            return From[Core.Random.Next(From.Length)];
        }
    }
}
