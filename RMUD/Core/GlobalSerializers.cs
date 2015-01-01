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
        public static Dictionary<String, PersistentValueSerializer> GlobalSerializers = new Dictionary<String, PersistentValueSerializer>();

        private static void AddGlobalSerializer(PersistentValueSerializer Serializer)
        {
            GlobalSerializers.Upsert(Serializer.TargetType.Name, Serializer);
        }

    }
}