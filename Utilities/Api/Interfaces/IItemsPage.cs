using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Interfaces
{
    public interface IItemsPage
    {
        /// <summary>
        /// An opaque cursor that represents the position in the list after the last returned item.
        /// Use this cursor for pagination to fetch the next set of items.
        /// If the cursor is null, there are no more items to fetch.
        /// </summary>
        string Cursor { get; set; }

        /// <summary>
        /// The items associated with the cursor.
        /// </summary>
        List<IItem> Items { get; set; }
    }
}
