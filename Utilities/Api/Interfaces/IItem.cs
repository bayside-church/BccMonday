using System;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public interface IItem
    {
        /// <summary>
        /// the item's unique identifier.
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// The item's name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// the item's create date.
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// The board that contains this item.
        /// </summary>
        IBoard Board { get; set; }

        long? BoardId { get; set; }

        /// <summary>
        /// The item's column values.
        /// </summary>
        List<Interfaces.AbstractColumnValue> ColumnValues { get; set; }

        /// <summary>
        /// The item's updates.
        /// </summary>
        List<IUpdate> Updates { get; set; }

        string GetRequestorEmail(string emailMatchColumnId);
    }
}
