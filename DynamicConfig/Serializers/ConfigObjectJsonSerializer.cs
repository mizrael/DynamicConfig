using System;
using Newtonsoft.Json;

namespace DynamicConfig.Serializers
{
    public class ConfigObjectJsonSerializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Models.ConfigObject).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var config = value as Models.ConfigObject;

            writer.WriteStartObject();

            foreach (var key in config.Keys)
            {
                writer.WritePropertyName(key);
                serializer.Serialize(writer, config[key]);
            }         

            writer.WriteEndObject();
        }
    }
}
