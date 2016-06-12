using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Basel
{
    public static class JsonRecordPersistor
    {
        public static bool Save(this IRecord record, string filename)
        {
            try
            {
                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(filename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, record);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string Serialize(this IRecord record)
        {
            try
            {
                return JsonConvert.SerializeObject(record, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                });
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static IRecord Load(string filename)
        {
            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (IRecord)serializer.Deserialize(file, typeof(IRecord));
            }
        }

        public static IRecord Deserialize(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<Record>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Task<bool> SaveAsync(this IRecord record, string filename)
        {
            return Task.Run(() => Save(record, filename));
        }

        public static Task<IRecord> LoadAsync(string filename)
        {
            return Task.Run(() => Load(filename));
        }
    }
}
