using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api.Schema
{
    /// <summary>
    /// A monday.com workspace
    /// </summary>
    public class Workspace : IWorkspace
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime CreatedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Kind { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<IMondayUser> Subscribers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
