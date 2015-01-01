using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
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
                    var mudObject = MudObject.GetObject(Reader.Value.ToString());
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
}
