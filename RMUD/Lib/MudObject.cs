using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class MudObject
	{
		public String Path { get; internal set; }
		public String Instance { get; internal set; }

		public bool Is(String other) 
		{
			return Path == other;
		}

		public Object GetDTO()
		{
			//Use the name 'Path@Instance' to fetch data from the dynamic database.
			// If Instance is null or empty, the resulting name is 'Path@'. This is the
			// name non-instanced objects can use to store data in the dynamic
			// database.
			return null;
		}

		public virtual void Initialize() { }

		public override string ToString()
		{
			return Path;
		}

	}
}
