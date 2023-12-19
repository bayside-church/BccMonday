using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Responses
{
    public class GetBoardsResponse
    {
        [JsonProperty("boards", ItemConverterType = typeof(ConcreteConverter<IBoard, Board>))]
        public List<IBoard> Boards { get; set; }
    }
}
