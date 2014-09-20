using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	/// <summary>
	/// Decorates an object that can be open or closed, and interactive with the open and close commands.
	/// </summary>
	public interface IOpenable
	{
		bool Open { get; }
		bool CanOpen(Actor Actor);
		bool CanClose(Actor Actor);
		void HandleOpen(Actor Actor);
		void HandleClose(Actor Actor);
	}
}
