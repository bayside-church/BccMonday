using com.baysideonline.BccMonday.Utilities.Api.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Responses
{
    /// <summary>
    /// Create a new update Response
    /// </summary>
    public class CreateUpdateResponse
    {
        [JsonProperty("create_update")]
        public Update Update { get; set; }
    }
}
