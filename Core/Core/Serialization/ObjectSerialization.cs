using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace RMUD
{
    public static partial class Core
    {
        /// <summary>
        /// Flatten and serialize a MudObject into a string.
        /// </summary>
        /// <param name="Object"></param>
        /// <returns></returns>
        public static String SerializeObject(MudObject Object)
        {
            var dest = new System.IO.StringWriter();
            var jsonWriter = new JsonTextWriter(dest);

            jsonWriter.WriteStartObject();
            foreach (var property in EnumeratePersistentProperties(Object))
            {
                jsonWriter.WritePropertyName(property.Item1.Name);
                property.Item2.WriteValue(property.Item1.GetValue(Object, null), jsonWriter, Object);
            }
            jsonWriter.WriteEndObject();
            return dest.ToString();
        }

        /// <summary>
        /// Parse a string, and deserialize it into the supplied object.
        /// </summary>
        /// <param name="Object">The MudObject to write deserialized data to</param>
        /// <param name="Data"></param>
        public static void DeserializeObject(MudObject Object, String Data)
        {
            var persistentProperties = new List<Tuple<System.Reflection.PropertyInfo, PersistAttribute>>(EnumeratePersistentProperties(Object));
            var jsonReader = new JsonTextReader(new System.IO.StringReader(Data));

            jsonReader.Read();
            jsonReader.Read();
            while (jsonReader.TokenType != JsonToken.EndObject)
            {
                var propertyName = jsonReader.Value.ToString();

                var prop = persistentProperties.FirstOrDefault(t => t.Item1.Name == propertyName);
                if (prop == null) throw new InvalidOperationException();
                jsonReader.Read();

                prop.Item1.SetValue(Object, prop.Item2.ReadValue(prop.Item1.PropertyType, jsonReader, Object), null);

            }

            jsonReader.Close();
        }

        private static IEnumerable<Tuple<System.Reflection.PropertyInfo, PersistAttribute>> EnumeratePersistentProperties(MudObject Object)
        {
            return
                Object.GetType().GetProperties()
                .Where(pi => pi.GetCustomAttributes(true).Count(a => a is PersistAttribute) >= 1)
                .Select(pi => Tuple.Create(pi, pi.GetCustomAttributes(true).FirstOrDefault(a => a is PersistAttribute) as PersistAttribute));
        }
    }
}