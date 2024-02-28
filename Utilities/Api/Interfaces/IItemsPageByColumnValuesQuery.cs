using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Interfaces
{
    public interface IItemsPageByColumnValuesQuery
    {
        /// <summary>
        /// The column's unique identifier
        /// </summary>
        string ColumnId { get; set; }

        /// <summary>
        /// The column values to search items by.
        /// </summary>
        List<string> ColumnValues { get; set; }
    }
}
