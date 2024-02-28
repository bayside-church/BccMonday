using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Interfaces
{
    /// <summary>
    /// A set of parameters to filter, sort, and control the scope of the baords query.
    /// Use this to customize the results based on specific criteria.
    /// Please note that you can't use query_params and cursor in the same request.
    /// We recommend using query_params for the cinitial request and
    /// cursor for paginated requests
    /// </summary>
    public interface IItemsQuery
    {
        /// <summary>
        /// The conditions between query rules. The default is "and".
        /// </summary>
        string Operator { get; set; }

        /// <summary>
        /// The attributes to sort results by
        /// </summary>
        List<IItemsQueryOrderBy> OrderBy { get; set; }

        /// <summary>
        /// The rules to filter your queries
        /// </summary>
        List<IItemsQueryRule> Rules { get; set; }
    }

    //enum "and", "or"
    public interface IItemsQueryOperator
    {

    }

    /// <summary>
    /// The rules to filter your queries
    /// </summary>
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
        string CompareValue { get; set; }

        /// <summary>
        /// The condition for value comparison. The default is any_of
        /// </summary>
        ItemsQueryRuleOperator Operator { get; set; }
    }

    public enum ItemsQueryRuleOperator
    {
        any_of,
        not_any_of,
        is_empty,
        is_not_empty,
        greater_than,
        greater_than_or_equals,
        lower_than,
        lower_than_or_equal,
        between,
        not_contains_text,
        contains_text,
        contains_terms,
        starts_with,
        ends_with,
        within_the_next,
        within_the_last
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
        ItemsOrderByDirection Direction { get; set; }
    }

    public enum ItemsOrderByDirection
    {
        asc,
        desc
    }
}
