using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public interface Noun
    {
        bool Match(String Word, Actor Actor);
        bool CouldMatch(String Word);
    }

    public class BasicNoun : Noun
    {
        public String Value;
        public Func<Actor, bool> Available;

        public bool Match(String Word, Actor Actor)
        {
            if (Word != Value) return false;
            if (Available != null) return Available(Actor);
            return true;
        }

        public bool CouldMatch(String Word)
        {
            return Word == Value;
        }

        public BasicNoun (String Value, Func<Actor, bool> Available)
        {
            this.Value = Value;
            this.Available = Available;
        }

        public BasicNoun (String Value)
        {
            this.Value = Value;
            this.Available = null;
        }
    }

    public class NounSet : Noun
    {
        public List<String> Value;
        public Func<Actor, bool> Available;

        public bool Match(String Word, Actor Actor)
        {
            if (!Value.Contains(Word)) return false;
            if (Available != null) return Available(Actor);
            return true;
        }

        public bool CouldMatch(String Word)
        {
            return Value.Contains(Word);
        }

        public NounSet (List<String> Value, Func<Actor, bool> Available)
        {
            this.Value = Value;
            this.Available = Available;
        }

        public NounSet (List<String> Value)
        {
            this.Value = Value;
            this.Available = null;
        }
    }

    public class NounList
    {
        List<Noun> Nouns = new List<Noun>();

        public NounList() { }

        public NounList(IEnumerable<String> From)
        {
            Add(From);
        }

        public void Add(String Noun)
        {
            Nouns.Add(new BasicNoun(Noun.ToUpper()));
        }

        public void Add(String Word, Func<Actor,bool> Available)
        {
            Nouns.Add(new BasicNoun(Word.ToUpper(), Available));
        }

        public void Add(params String[] Range)
        {
            Nouns.Add(new NounSet(new List<String>(Range.Select(s => s.ToUpper()))));
        }

        public void Add(IEnumerable<String> Range)
        {
            Nouns.Add(new NounSet(new List<String>(Range.Select(s => s.ToUpper()))));
        }

        public void Add(List<String> Words, Func<Actor, bool> Available)
        {
            Nouns.Add(new NounSet(new List<String>(Words.Select(s => s.ToUpper())), Available));
        }

        public bool Match(String Word, Actor Actor)
        {
            foreach (var noun in Nouns)
                if (noun.Match(Word, Actor)) return true;
            return false;
        }

        public void Remove(String Word)
        {
            Nouns.RemoveAll(n => n.CouldMatch(Word));
        }
    }
}
