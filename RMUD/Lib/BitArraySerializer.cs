using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public class BitArraySerializer : PersistentValueSerializer
    {
        public BitArraySerializer()
        {
            TargetType = typeof(System.Collections.BitArray);
        }

        public override void WriteValue(object Value, Newtonsoft.Json.JsonWriter Writer, MudObject Owner)
        {
            var array = Value as System.Collections.BitArray;
            if (array == null) throw new InvalidOperationException();

            var builder = new StringBuilder();
            for (int i = 0; i < array.Length; ++i)
                if (array[i] == true) builder.Append("1");
                else builder.Append("0");

            Writer.WriteValue(builder.ToString());
        }

        public override object ReadValue(object StoredValue, Newtonsoft.Json.JsonReader Reader, MudObject Owner)
        {
            var value = Reader.Value.ToString();
            var r = new System.Collections.BitArray(value.Length);
            for (int i = 0; i < value.Length; ++i)
                r[i] = (value[i] == '1');
            Reader.Read();
            return r;
        }
    }
}
