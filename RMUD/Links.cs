using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public enum Direction
	{
		North,
		NorthEast,
		East,
		SouthEast,
		South,
		SouthWest,
		West,
		NorthWest,
		Up,
		Down,
		In,
		Out
	}

	public class Link
	{
		public Direction Direction;
		public String Destination;
	}

}