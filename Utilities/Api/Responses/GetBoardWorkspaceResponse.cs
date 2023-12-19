using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Responses
{
    public class GetBoardWorkspaceResponse
    {
        [JsonProperty("boards")]
        public List<WorkspaceBoard> Boards {  get; set; }
    }

    public class WorkspaceBoard
    {
        [JsonProperty("workspace")]
        public Workspace Workspace { get; set; }
    }
}
