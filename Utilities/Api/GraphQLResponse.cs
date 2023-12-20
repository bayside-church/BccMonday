using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class GraphQLResponse<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("errors")]
        public List<GraphQlError> Errors { get; set; }
    }

    public class GraphQLMutation<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("errors")]
        public List<GraphQlError> Errors { get; set; }
    }

    public class GraphQlError
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        // Add other properties of the error object if needed
    }
}
