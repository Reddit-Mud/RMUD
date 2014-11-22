using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public struct Noun
    {
        public String Value;
        public Func<Actor, bool> Available;

        public bool Match(String Word, Actor Actor)
        {
            if (Word != Value) return false;
            if (Available != null) return Available(Actor);
            return true;
        }

        public Noun (String Value, Func<Actor, bool> Available)
        {
            this.Value = Value;
            this.Available = Available;
        }

        public Noun (String Value)
        {
            this.Value = Value;
            this.Available = null;
        }
    }

    public class NounList : List<Noun>
    {
        public NounList() { }

        public NounList(IEnumerable<String> From)
        {
            AddRange(From);
        }

        public void Add(String Noun)
        {
            base.Add(new Noun(Noun.ToUpper()));
        }

        public void Add(String Word, Func<Actor,bool> Available)
        {
            Add(new Noun(Word.ToUpper(), Available));
        }

        public void Add(params String[] Nouns)
        {
            for (int i = 0; i < Nouns.Length; ++i)
                Add(Nouns[i]);
        }

        public void AddRange(IEnumerable<String> Range)
        {
            foreach (var str in Range)
                Add(str);
        }

        public bool Match(String Word, Actor Actor)
        {
            foreach (var noun in this)
                if (noun.Match(Word, Actor)) return true;
            return false;
        }
    }
}
