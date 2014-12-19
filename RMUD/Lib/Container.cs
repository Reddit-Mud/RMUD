using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    [Flags]
    public enum RelativeLocations
    {
        None = 0,
        Default = 0,

        Contents = 1, //As of a room
        Links = 2,
        Scenery = 4,

        In = 8,
        On = 16,
        Under = 32,
        Behind = 64,

        Held = 128,
        Worn = 256,

        ConnectedPlayers = 512,

        Room = Contents | Links | Scenery,
        Player = Held | Worn,
        Supporter = In | On | Under | Behind,
        EveryMudObject = Contents | Links | Scenery | In | On | Under | Behind | Held | Worn,
    }
    
    public class ContainerSerializer : PersistentValueSerializer
    {
        private static String RelativeLocationToString(RelativeLocations Relloc)
        {
            var parts = Relloc.ToString().Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 2) throw new InvalidOperationException();
            return parts[1].Trim();
        }

        private static RelativeLocations StringToRelativeLocation(String Str)
        {
            RelativeLocations r = RelativeLocations.None;
            if (Enum.TryParse(Str, out r))
                return r;
            else
                throw new InvalidOperationException();
        }

        public override void WriteValue(object Value, Newtonsoft.Json.JsonWriter Writer, MudObject Owner)
        {
            var contents = Value as Dictionary<RelativeLocations, List<MudObject>>;
            if (contents == null) throw new InvalidOperationException();

            Writer.WriteStartObject();
            
            foreach (var relloc in contents)
            {
                Writer.WritePropertyName(RelativeLocationToString(relloc.Key));
                Writer.WriteStartArray();

                foreach (var mudObject in relloc.Value.Where(o => o.IsNamedObject && o.IsInstance))
                    Writer.WriteValue(mudObject.GetFullName());

                Writer.WriteEndArray();
            }

            Writer.WriteEndObject();
        }

        public override object ReadValue(Type ValueType, Newtonsoft.Json.JsonReader Reader, MudObject Owner)
        {
            var r = new Dictionary<RelativeLocations, List<MudObject>>();

            Reader.Read();
            while (Reader.TokenType != Newtonsoft.Json.JsonToken.EndObject)
            {
                var relloc = StringToRelativeLocation(Reader.Value.ToString());
                var l = new List<MudObject>();
                Reader.Read();
                Reader.Read();
                while (Reader.TokenType != Newtonsoft.Json.JsonToken.EndArray)
                {
                    var mudObject = Mud.GetObject(Reader.Value.ToString());
                    if (mudObject != null) l.Add(mudObject);
                    mudObject.Location = Owner;
                    Reader.Read();
                }
                Reader.Read();
                r.Upsert(relloc, l);
            }
            Reader.Read();

            return r;
        }
    }

	public class Container : MudObject
	{
        [Persist(typeof(ContainerSerializer))]
        public Dictionary<RelativeLocations, List<MudObject>> Lists { get; set; }

        public RelativeLocations Supported;
        public RelativeLocations Default;

        public Container(RelativeLocations Locations, RelativeLocations Default)
        {
            this.Supported = Locations;
            this.Default = Default;
            this.Lists = new Dictionary<RelativeLocations, List<MudObject>>();
        }

		#region IContainer

        public void Remove(MudObject Object)
        {
            foreach (var list in Lists)
            {
                if (list.Value.Remove(Object))
                    Object.Location = null;
            }
        }

        public int RemoveAll(Predicate<MudObject> Func)
        {
            var r = 0;
            foreach (var list in Lists)
                r += list.Value.RemoveAll(Func);
            return r;
        }

		public void Add(MudObject Object, RelativeLocations Locations)
		{
            if (Locations == RelativeLocations.Default) Locations = Default;

            if ((Supported & Locations) == Locations)
            {
                if (!Lists.ContainsKey(Locations)) Lists.Add(Locations, new List<MudObject>());
                Lists[Locations].Add(Object);
            }
        }

        public EnumerateObjectsControl EnumerateObjects(RelativeLocations Locations, Func<MudObject, RelativeLocations, EnumerateObjectsControl> Callback)
        {
            if (Locations == RelativeLocations.Default) Locations = Default;

            foreach (var list in Lists)
            {
                if ((Locations & list.Key) == list.Key)
                    foreach (var MudObject in list.Value)
                        if (Callback(MudObject, list.Key) == EnumerateObjectsControl.Stop) return EnumerateObjectsControl.Stop;
            }
            return EnumerateObjectsControl.Continue;
        }

        public IEnumerable<MudObject> EnumerateObjects()
        {
            foreach (var list in Lists)
                foreach (var item in list.Value)
                    yield return item;
        }

        public IEnumerable<T> EnumerateObjects<T>() where T: MudObject
        {
            foreach (var list in Lists)
                foreach (var item in list.Value)
                    if (item is T) yield return item as T;
        }

        public IEnumerable<MudObject> EnumerateObjects(RelativeLocations Locations)
        {
            foreach (var list in Lists)
                if ((list.Key & Locations) == list.Key)
                    foreach (var item in list.Value)
                        yield return item;
        }

        public IEnumerable<T> EnumerateObjects<T>(RelativeLocations Locations) where T: MudObject
        {
            foreach (var list in Lists)
                if ((list.Key & Locations) == list.Key)
                    foreach (var item in list.Value)
                        if (item is T) yield return item as T;
        }

        public List<MudObject> GetContents(RelativeLocations Locations)
        {
            return new List<MudObject>(EnumerateObjects(Locations));
        }

        public bool Contains(MudObject Object, RelativeLocations Locations)
        {
            if (Locations == RelativeLocations.Default) Locations = Default;

            if (Lists.ContainsKey(Locations))
                return Lists[Locations].Contains(Object);
            return false;
        }

        public RelativeLocations LocationsSupported { get { return Supported; } }

        public RelativeLocations DefaultLocation { get { return Default; } }

        public RelativeLocations LocationOf(MudObject Object)
        {
            foreach (var list in Lists)
                if (list.Value.Contains(Object)) return list.Key;
            return RelativeLocations.None;
        }

		#endregion
	}
}
