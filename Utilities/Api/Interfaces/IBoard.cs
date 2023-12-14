using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    /// <summary>
    /// A monday.com board
    /// </summary>
    public interface IBoard
    {
        /// <summary>
        /// The unique identifier of the board.
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// The board's name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The board's visible columns.
        /// </summary>
        List<IColumn> Columns { get; set; }

        List<IItem> Items { get; set; }

        /// <summary>
        /// Gets a Column by a specified Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IColumn</returns>
        IColumn GetColumn(string id);
    }
}
