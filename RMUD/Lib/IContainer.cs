using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public interface IContainer
	{
		void Remove(MudObject Object);
		void Add(MudObject Object);
        EnumerateObjectsControl EnumerateObjects(Func<MudObject, EnumerateObjectsControl> Callback);
        bool Contains(MudObject Object);
	}
}
