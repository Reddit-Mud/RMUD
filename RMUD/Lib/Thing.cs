using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Thing : MudObject
	{
		public String Short;
		public String Long;
		public String IndefiniteArticle = "a";
		public List<String> Nouns = new List<string>();
	}
}
