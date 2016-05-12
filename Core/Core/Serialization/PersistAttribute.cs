﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace RMUD
{
    /// <summary>
    /// Apply this attribute to the members of a MudObject to mark them as persistent.
    /// </summary>
    public class PersistAttribute : Attribute
    {
        internal ValueSerializer Serializer = null;

        public PersistAttribute(Type SerializerType = null)
        {
            if (SerializerType != null)
                Serializer = Activator.CreateInstance(SerializerType) as ValueSerializer;
        }

        public void WriteValue(Object Value, JsonWriter Writer, MudObject Owner)
        {
            if (Serializer != null) Serializer.WriteValue(Value, Writer, Owner);
            else PersistAttribute._WriteValue(Value, Writer, Owner);
        }

        public static void _WriteValue(Object Value, JsonWriter Writer, MudObject Owner)
        {
            var name = Value.GetType().Name;
            ValueSerializer serializer = null;
            if (ValueSerializer.GlobalSerializers.TryGetValue(name, out serializer))
            {
                Writer.WriteStartObject();
                Writer.WritePropertyName("$type");
                Writer.WriteValue(name);
                Writer.WritePropertyName("$value");
                serializer.WriteValue(Value, Writer, Owner);
                Writer.WriteEndObject();
            }
            else Writer.WriteValue(Value); //Hope...
        }

        public Object ReadValue(Type ValueType, JsonReader Reader, MudObject Owner)
        {
            if (Serializer != null) return Serializer.ReadValue(ValueType, Reader, Owner);
            return _ReadValue(ValueType, Reader, Owner);
        }

        public static Object _ReadValue(Type ValueType, JsonReader Reader, MudObject Owner)
        {
            Object r = null;

            if (Reader.TokenType == JsonToken.String) { r = Reader.Value.ToString(); Reader.Read(); }
            else if (Reader.TokenType == JsonToken.Integer) { r = Convert.ToInt32(Reader.Value.ToString()); Reader.Read(); }
            else if (Reader.TokenType == JsonToken.Boolean) { r = Convert.ToBoolean(Reader.Value.ToString()); Reader.Read(); }
            else 
            {
                ValueSerializer serializer = null;
                if (ValueType != null && ValueSerializer.GlobalSerializers.TryGetValue(ValueType.Name, out serializer))
                    return serializer.ReadValue(ValueType, Reader, Owner);
                else if (Reader.TokenType == JsonToken.StartObject)
                {
                    Reader.Read();
                    if (Reader.TokenType != JsonToken.PropertyName || Reader.Value.ToString() != "$type") throw new InvalidOperationException();
                    Reader.Read();
                    if (!ValueSerializer.GlobalSerializers.TryGetValue(Reader.Value.ToString(), out serializer))
                        throw new InvalidOperationException();
                    Reader.Read();
                    Reader.Read();
                    r = serializer.ReadValue(ValueType, Reader, Owner);
                    Reader.Read();
                }
                else throw new InvalidOperationException();
            }
            return r;
        }
    }
}