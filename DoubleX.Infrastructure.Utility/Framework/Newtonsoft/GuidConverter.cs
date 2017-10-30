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
    /// JSON.NET Guid //http://www.cnblogs.com/Leo_wl/p/4805925.html
    /// [JsonConverter(typeof(GuidConverter))]  
    /// public Guid Id { get; set; }  
    /// </summary>
    public class GuidConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType==typeof(Guid);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader == null || (reader != null && reader.Value.ToString() == ""))
                {
                    return Guid.Empty;
                }
                var value = Guid.Empty;
                Guid.TryParse(reader.Value.ToString(), out value);
                return value;
            }
            catch
            {
                //如果传进来的值造成异常，则赋值一个初值  
                return Guid.Empty;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                value = Guid.Empty;

            serializer.Serialize(writer, GuidHelper.Get(value).ToString());
        }
    }
}
