using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public interface IContainer : IEnumerable<MudObject>
	{
		void Remove(MudObject Object);
		void Add(MudObject Object);
	}
}
