using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rock.UniversalSearch;

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
                    //color -> status for new API
                    case "status":
                    {
                        columnValue = DeserializeColorColumnValue(jo);
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

                            columnValue = new BoardRelationColumnValue { ItemIds = itemIds };
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

        private StatusColumnValue DeserializeColorColumnValue(JObject jo)
        {
            /*
            string infoStr = (string)jo["additional_info"];
            if (!string.IsNullOrWhiteSpace(infoStr))
            {
                var info = JsonConvert.DeserializeObject<ColorInfo>(infoStr);
                return new StatusColumnValue { Color = info.Color };
            }
            */
            return new StatusColumnValue();
        }

        private FileColumnValue DeserializeFileColumnValue(JObject jo)
        {
            /*
            string valueStr = (string)jo["value"];
            if (!string.IsNullOrWhiteSpace(valueStr))
            {
                var fileInfo = JsonConvert.DeserializeObject<FileValue>(valueStr);
                var assetIds = new List<long>();
                foreach (var file in fileInfo.Files)
                {
                    if (file.FileType != "MONDAY_DOC")
                    {
                        assetIds.Add(file.AssetId);
                    }
                }
                return new FileColumnValue { AssetIds = assetIds };
            }
            */
            return new FileColumnValue();
        }

        private BoardRelationColumnValue DeserializeBoardRelationColumnValue(JObject jo)
        {
            /*
            string valueStr = (string)jo["value"];
            if (!string.IsNullOrWhiteSpace(valueStr))
            {
                var pulseIdValues = JsonConvert.DeserializeObject<BoardRelationValue>(valueStr);
                var itemIds = new List<long>();
                foreach (var linkedPulseId in pulseIdValues.LinkedPulseIds)
                {
                    itemIds.Add(linkedPulseId.LinkedPulseId);
                }
                return new BoardRelationColumnValue { ItemIds = itemIds };
            }
            */
            return new BoardRelationColumnValue();
        }

        // Strongly-typed models for deserialization
        private class ColorInfo
        {
            public string Color { get; set; }
        }
    }
}
