using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class EnumSerializer<EnumType> : PersistentValueSerializer
    {
        public override void WriteValue(object Value, Newtonsoft.Json.JsonWriter Writer, MudObject Owner)
        {
            Writer.WriteValue(Value.ToString());
        }

        public override object ReadValue(Type ValueType, Newtonsoft.Json.JsonReader Reader, MudObject Owner)
        {
            var r = Enum.Parse(typeof(EnumType), Reader.Value.ToString());
            Reader.Read();
            return r;
        }
    }
}
