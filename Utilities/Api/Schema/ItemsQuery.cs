using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api.Schema
{
    public class ItemsQuery : IItemsQuery
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("operator")]
        public string Operator { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("order_by")]
        public List<IItemsQueryOrderBy> OrderBy { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("rules")]
        public List<IItemsQueryRule> Rules { get; set; }
    }

    public class ItemsQueryRule : IItemsQueryRule
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("column_id")]
        public string ColumnId { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("compare_attribute")]
        public string CompareAttribute { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("compare_value")]
        public string CompareValue { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("operator")]
        public ItemsQueryRuleOperator Operator { get; set; } = ItemsQueryRuleOperator.any_of;
    }

    public class ItemsQueryOrderBy : IItemsQueryOrderBy
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("column_id")]
        public string ColumnId { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("direction")]
        public ItemsOrderByDirection Direction { get; set; } = ItemsOrderByDirection.asc;
    }
}
