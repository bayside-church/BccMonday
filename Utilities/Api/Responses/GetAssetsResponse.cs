using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Responses
{
    public class GetAssetsResponse
    {
        [JsonProperty("assets", ItemConverterType = typeof(ConcreteConverter<IAsset, Asset>))]
        public List<IAsset> Assets { get; set; }
    }
}
