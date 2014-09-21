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
	}

	public class Link
	{
		public Direction Direction;
		public String Destination;
		public Door Door;

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
			"OUT", "O"
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
				default: return Direction.NORTH;
			}
		}

	}

}