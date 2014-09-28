using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public enum Direction
	{
		NORTH,
		NORTHEAST,
		EAST,
		SOUTHEAST,
		SOUTH,
		SOUTHWEST,
		WEST,
		NORTHWEST,
		UP,
		DOWN,
		IN,
		OUT,
        PORT,
        STARBOARD,
        FORE,
        AFT,
	}

	public class Link
	{
		public Direction Direction;
		public String Destination;
		public MudObject Door;

        public override string ToString()
        {
            return Direction + " to " + Destination + (Door == null ? "" : (" through " + Door));
        }

		private static List<String> Names = new List<String>
        { 
            "NORTH", "N",
            "NORTHEAST", "NE",
            "EAST", "E",
            "SOUTHEAST", "SE", 
            "SOUTH", "S",
            "SOUTHWEST", "SW",
            "WEST", "W",
            "NORTHWEST", "NW", 
            "UP", "U",
            "DOWN", "D" ,
			"IN", "IN",		//'I' cannot be used as it is shorthand for the inventory command.
			"OUT", "O",
            "PORT", "P",
            "STARBOARD", "SB",
            "FORE", "F",
            "AFT", "A",
        };

		public static bool IsCardinal(String _str)
		{
			return Names.Contains(_str.ToUpper());
		}

		public static Direction ToCardinal(String _str)
		{
			return (Direction)(Names.IndexOf(_str.ToUpper()) / 2);
		}

		public static String ToString(Direction Cardinal)
		{
			return Cardinal.ToString().ToLower();
		}

		public static Direction Opposite(Direction Of)
		{
			switch (Of)
			{
				case Direction.NORTH: return Direction.SOUTH;
				case Direction.NORTHEAST: return Direction.SOUTHWEST;
				case Direction.EAST: return Direction.WEST;
				case Direction.SOUTHEAST: return Direction.NORTHWEST;
				case Direction.SOUTH: return Direction.NORTH;
				case Direction.SOUTHWEST: return Direction.NORTHEAST;
				case Direction.WEST: return Direction.EAST;
				case Direction.NORTHWEST: return Direction.SOUTHEAST;
				case Direction.UP: return Direction.DOWN;
				case Direction.DOWN: return Direction.UP;
				case Direction.IN: return Direction.OUT;
				case Direction.OUT: return Direction.IN;
                case Direction.PORT: return Direction.STARBOARD;
                case Direction.STARBOARD: return Direction.PORT;
                case Direction.FORE: return Direction.AFT;
                case Direction.AFT: return Direction.FORE;
				default: return Direction.NORTH;
			}
		}

        public static String FromMessage(Direction Of)
        {
            switch (Of)
            {
                case Direction.NORTH: return "from the north";
                case Direction.NORTHEAST: return "from the northeast";
                case Direction.EAST: return "from the east";
                case Direction.SOUTHEAST: return "from the southeast";
                case Direction.SOUTH: return "from the south";
                case Direction.SOUTHWEST: return "from the southwest";
                case Direction.WEST: return "from the west";
                case Direction.NORTHWEST: return "from the northwest";
                case Direction.UP: return "from above";
                case Direction.DOWN: return "from below";
                case Direction.IN: return "from inside";
                case Direction.OUT: return "from outside";
                case Direction.PORT: return "from port";
                case Direction.STARBOARD: return "from starboard";
                case Direction.FORE: return "from foreward";
                case Direction.AFT: return "from aftward";
                default: return "";
            }
        }

	}

}