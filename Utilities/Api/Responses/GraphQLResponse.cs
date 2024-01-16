using Newtonsoft.Json;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api.Responses
{
    public class GraphQLResponse<T>
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
