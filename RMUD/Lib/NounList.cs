using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class NounList : List<String>
    {
        new public void Add(String Noun)
        {
            base.Add(Noun.ToUpper());
        }

        public void Add(params String[] Nouns)
        {
            for (int i = 0; i < Nouns.Length; ++i)
                base.Add(Nouns[i].ToUpper());
        }

        new public void AddRange(IEnumerable<String> Range)
        {
            foreach (var str in Range)
                Add(str);
        }
    }
}
