using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api.Schema
{
    public class ItemsPageByColumnValuesQuery : IItemsPageByColumnValuesQuery
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("column_id")]
        public string ColumnId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("column_values")]
        public List<string> ColumnValues { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
