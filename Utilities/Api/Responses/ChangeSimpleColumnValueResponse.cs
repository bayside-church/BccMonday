using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Responses
{
    public class ChangeSimpleColumnValueResponse
    {
        [JsonProperty("change_simple_column_value")]
        public IItem Item { get; set; }
    }
}
