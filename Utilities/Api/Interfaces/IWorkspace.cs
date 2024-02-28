using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        /// <summary>
        /// The workspace's creation date.
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// The workspace's description
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The workspace's kind (open / closed).
        /// </summary>
        string Kind { get; set; }

        /// <summary>
        /// The workspace's state (all / active / archived / deleted).
        /// </summary>
        string State { get; set; }

        /// <summary>
        /// The users subscribed to the workspace.
        /// </summary>
        List<IMondayUser> Subscribers { get; set; }
    }
}
