using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class MudObject
	{
		public String Path { get; internal set; }
		
		public virtual void Initialize() { }

	}
}
