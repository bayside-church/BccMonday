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
        /// The board object type.
        /// </summary>
        string BoardType { get; set; }

        /// <summary>
        /// The workspace that contains this board (null for main workspace).
        /// </summary>
        IWorkspace Workspace { get; set; }

        /// <summary>
        /// The board's worksapce unique identifier (null for main workspace).
        /// </summary>
        string WorkspaceId { get; set; }

        /// <summary>
        /// The board's description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The Board's item nickname, one of a predefined set of values, or a custom user value.
        /// </summary>
        string ItemTerminology { get; set; }

        /// <summary>
        /// The number of items on the board
        /// </summary>
        int ItemsCount { get; set; }

        /// <summary>
        /// List of user board owners
        /// </summary>
        List<IMondayUser> Owners { get; set; }

        /// <summary>
        /// The board's permissions.
        /// </summary>
        string Permissions { get; set; }

        /// <summary>
        /// The board's state (all / active / archived / deleted).
        /// </summary>
        string State { get; set; }

        /// <summary>
        /// The board's subscribers
        /// </summary>
        List<IMondayUser> Subscribers { get; set; }

        /// <summary>
        /// Gets a Column by a specified Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IColumn</returns>
        IColumn GetColumn(string id);
    }
}
