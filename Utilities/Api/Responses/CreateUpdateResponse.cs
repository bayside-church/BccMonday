using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Responses
{
    public class CreateUpdateResponse
    {
        [JsonProperty("create_update", ItemConverterType = typeof(ConcreteConverter<IUpdate, Update>))]
        public IUpdate Update { get; set; }
    }
}
