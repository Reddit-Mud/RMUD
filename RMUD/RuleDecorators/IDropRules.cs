﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public interface IDropRules
	{
		bool CanDrop(Actor Actor);
	}
}