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
    /// Create a new Column Response
    /// </summary>
    public class CreateColumnResponse
    {
        [JsonProperty("create_column")]
        public Column Column { get; set; }
    }
}
