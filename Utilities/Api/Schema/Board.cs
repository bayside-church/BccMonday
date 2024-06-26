﻿using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api.Schema
{
    /// <summary>
    /// A <see href="https://monday.com"/> board
    /// </summary>
    public class Board : IBoard
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("id", Required = Required.Always)]
        public long Id { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

         /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("columns", ItemConverterType = typeof(ConcreteConverter<IColumn, Column>), NullValueHandling = NullValueHandling.Ignore)]
        public List<IColumn> Columns { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("items_page", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ConcreteConverter<IItemsPage, ItemsPage>))]
        public IItemsPage ItemsPage { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("type")]
        public string BoardType { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("workspace", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ConcreteConverter<IWorkspace, Workspace>))]
        public IWorkspace Workspace { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string WorkspaceId { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Description { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ItemTerminology { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int ItemsCount { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<IMondayUser> Owners { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Permissions { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string State { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<IMondayUser> Subscribers { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <inheritdoc/>
        public IColumn GetColumn(string id)
        {
            return Columns?.Find(c => c.Id == id);
        }
    }
}
