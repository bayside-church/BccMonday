using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Interfaces
{
    public interface IWorkspace
    {
        /// <summary>
        /// The workspace's unique identifier
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// The workspace's name.
        /// </summary>
        string Name { get; set; }
    }
}
