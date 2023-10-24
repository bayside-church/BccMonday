using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace com.baysideonline.BccMonday.Utilities.Api.Interfaces
{
    public class ColumnValueConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(AbstractColumnValue).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            var cvType = (string)jo["type"];

            AbstractColumnValue columnValue = new ColumnValue();
            if (!string.IsNullOrWhiteSpace(cvType))
            {
                switch(cvType)
                {
                    //TODO(Noah): move these into constructors?
                    case "color":
                    {
                        var color = "";
                        var infoStr = (string)jo["additional_info"];
                        if (!string.IsNullOrWhiteSpace(infoStr))
                        {
                            var info = JsonConvert.DeserializeObject<dynamic>(infoStr);
                            color = info["color"].Value;
                        }
                        columnValue = new ColorColumnValue { Color = color };
                        break;
                    }
                    case "file":
                    {
                        var assetIds = new List<long>();
                        var valueStr = (string)jo["value"];
                        if (!string.IsNullOrWhiteSpace(valueStr))
                        {
                            var fileInfo = JsonConvert.DeserializeObject<dynamic>(valueStr);
                            var files = fileInfo["files"];
                            if (files != null)
                            {
                                foreach (var asset in files)
                                {
                                    if (asset["fileType"].Value == "MONDAY_DOC")
                                    {
                                        continue;
                                    }
                                    assetIds.Add(asset["assetId"].Value);
                                }
                            }
                        }
                        //TODO(Noah): Grab the actual files from Monday.com to store inside the FileColumnValue
                        //NOTE(Noah): This could go inside of the CreateControl() method
                        columnValue = new FileColumnValue { AssetIds = assetIds };
                        break;
                    }
                    case "board-relation":
                    {
                        var pulseIdValuesStr = (string)jo["value"];
                        if (!string.IsNullOrWhiteSpace(pulseIdValuesStr))
                        {
                            var pulseIdValues = JsonConvert.DeserializeObject<dynamic>(pulseIdValuesStr);
                            var itemIds = new List<long>();
                            if (pulseIdValues != null)
                            {
                                if (pulseIdValues["linkedPulseIds"] != null && pulseIdValues["linkedPulseIds"].Count > 0)
                                {
                                    foreach (var linkedPulseId in pulseIdValues["linkedPulseIds"])
                                    {
                                        itemIds.Add(linkedPulseId["linkedPulseId"].Value);
                                    }
                                }
                            }

                            columnValue = new ItemListColumnValue { ItemIds = itemIds };
                        }
                        
                        break;
                    }
                    default:
                    {
                        columnValue = new ColumnValue();
                        break;
                    }
                }
            }

            serializer.Populate(jo.CreateReader(), columnValue);
            return columnValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
