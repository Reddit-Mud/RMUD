using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public interface ICommandProcessor
    {
		void Perform(PossibleMatch Match, Actor Actor);
    }
}
