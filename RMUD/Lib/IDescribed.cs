using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public interface IDescribed
	{
		DescriptiveText Long { get; set; }
	}
}
