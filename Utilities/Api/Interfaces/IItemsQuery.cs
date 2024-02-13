using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Interfaces
{
    public interface IItemsQuery
    {
        [JsonProperty("operator")]
        public string Operator { get; set; }

        [JsonProperty("order_by")]
        public string OrderBy { get; set; }


        [JsonProperty("rules")]
        public string Rules { get; set; }
    }

    //enum "and", "or"
    public interface IItemsQueryOperator
    {

    }

    public interface IItemsQueryRule
    {
        /// <summary>
        /// The unique identifier of the column to filter by.
        /// </summary>
        string ColumnId { get; set; }

        /// <summary>
        /// The comparison attribute.
        /// </summary>
        string CompareAttribute { get; set; }

        /// <summary>
        /// The column value to filter by. This can be a string or index value depending on the column type.
        /// </summary>
        ICompareValue CompareValue { get; set; }

        /// <summary>
        /// The condition for value comparison. The default is any_of
        /// </summary>
        IItemsQueryRuleOperator Operator { get; set; }
    }

    public interface IItemsQueryRuleOperator
    {
        string Value { get; set; }
    }

    public interface IItemsQueryOrderBy
    {
        /// <summary>
        /// The unique identifier of the column to filter or sort by.
        /// You can also enter "__creation_log__" or "__last_updated__" to chronologically sort results by their
        /// last updated or creation date (oldest to newest).
        /// </summary>
        string ColumnId { get; set; }

        /// <summary>
        /// The direction to sort items in. The default is asc
        /// </summary>
        string Direction { get; set; }
    }
}
