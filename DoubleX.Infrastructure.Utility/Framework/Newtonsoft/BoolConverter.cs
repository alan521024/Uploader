using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace DoubleX.Infrastructure.Utility
{

    /// <summary>
    /// JSON.NET Guid //http://stackoverflow.com/questions/14427596/convert-an-int-to-bool-with-json-net
    /// [JsonConverter(typeof(BoolConverter))]  
    /// public Guid Id { get; set; }  
    /// </summary>
    public class BoolConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader == null || (reader != null && reader.Value.ToString() == ""))
                {
                    return false;
                }
                return BoolHelper.Get(reader.Value);
            }
            catch
            {
                return false;
            }
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((bool)value) ? 1 : 0);
        }
    }
}
