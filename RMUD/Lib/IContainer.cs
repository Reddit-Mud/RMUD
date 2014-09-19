﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public interface IContainer : IEnumerable<Thing>
	{
		void Remove(Thing Object);
		void Add(Thing Object);
	}
}
