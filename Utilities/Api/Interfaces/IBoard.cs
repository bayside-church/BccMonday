using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api
{
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

        /// <summary>
        /// The board's items (rows).
        /// </summary>
        IItemsPage ItemsPage { get; set; }

        /// <summary>
        /// the board object type.
        /// </summary>
        string BoardType { get; set; }

        /// <summary>
        /// The workspace that contains this board (null for main workspace).
        /// </summary>
        IWorkspace Workspace { get; set; }

        /// <summary>
        /// Gets a Column by a specified Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IColumn</returns>
        IColumn GetColumn(string id);
    }
}
