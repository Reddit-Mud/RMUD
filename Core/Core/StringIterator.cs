using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class StringIterator
	{
		internal String data;
		internal int place = 0;

		public char Next
		{
            get
            {
                return data[place];
            }
		}

        public bool PeekCheck(String s)
        {
            if (place + s.Length > data.Length) return false;
            return data.Substring(place, s.Length) == s;
        }

		public void Advance()
		{
			++place;
		}

        public bool AtEnd
        {
            get
            {
                return place >= data.Length;
            }
        }

		public StringIterator(String data)
		{
			this.data = data;
		}

		public StringIterator(String data, int place)
		{
			this.data = data;
			this.place = place;
		}
	}

}
