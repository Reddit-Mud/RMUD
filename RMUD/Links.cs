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
		OUT
	}

	public class Link
	{
		public Direction Direction;
		public String Destination;
	}

}