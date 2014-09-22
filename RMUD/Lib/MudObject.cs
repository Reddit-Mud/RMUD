using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class MudObject
	{
		public String Id { get; internal set; }
        public bool Is(String other) { return Id == other; }

		public virtual void Initialize() { }

		public override string ToString()
		{
            return Id;
		}

        public virtual void Save() { }

	}
}
