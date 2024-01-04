using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace com.baysideonline.BccMonday.Utilities.Api
{

    public class ConcreteConverter<TInterface, TConcrete> : JsonConverter where TConcrete : TInterface
    {
        public override bool CanConvert(Type objectType) => typeof(TInterface) == objectType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => serializer.Deserialize<TConcrete>(reader);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TConcrete concreteInstance = (TConcrete)value;
            serializer.Serialize(writer, concreteInstance);
        }
    }

    public class ListItemConverter<TInterface, TConcrete> : JsonConverter where TConcrete : TInterface
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<TInterface>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray jsonArray = JArray.Load(reader);
            List<TInterface> list = new List<TInterface>();
            foreach (var item in jsonArray)
            {
                list.Add(item.ToObject<TConcrete>());
            }
            return list;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
