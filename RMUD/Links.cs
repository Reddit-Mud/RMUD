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

	}

}