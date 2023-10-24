using System;
using Newtonsoft.Json;

namespace com.baysideonline.BccMonday.Utilities.Api
{

    public class ConcreteConverter<I, T> : JsonConverter
    {
        public override bool CanConvert(Type objectType) => typeof(I) == objectType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => serializer.Deserialize<T>(reader);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => serializer.Serialize(writer, value);
    }
}
