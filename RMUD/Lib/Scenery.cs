using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Scenery : IDescribed, IMatchable
	{
		public List<String> Nouns { get; set; }
		public DescriptiveText Long { get; set; }

		public Scenery()
		{
			Nouns = new List<string>();
		}
	}
}
