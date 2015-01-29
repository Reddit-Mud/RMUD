using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class DictionaryStringObjectSerializer : PersistentValueSerializer
    {
        public override void WriteValue(object Value, Newtonsoft.Json.JsonWriter Writer, MudObject Owner)
        {
            var contents = Value as Dictionary<String, Object>;
            if (contents == null) throw new InvalidOperationException();

            Writer.WriteStartObject();

            foreach (var pair in contents)
            {
                Writer.WritePropertyName(pair.Key);
                PersistAttribute._WriteValue(pair.Value, Writer, Owner);
            }

            Writer.WriteEndObject();
        }

        public override object ReadValue(Type ValueType, Newtonsoft.Json.JsonReader Reader, MudObject Owner)
        {
            var r = new Dictionary<String, Object>();

            Reader.Read();
            while (Reader.TokenType != Newtonsoft.Json.JsonToken.EndObject)
            {
                var name = Reader.Value.ToString();
                Reader.Read();
                var value = PersistAttribute._ReadValue(null, Reader, Owner);
                r.Upsert(name, value);
            }
            Reader.Read();

            return r;
        }
    }
}
