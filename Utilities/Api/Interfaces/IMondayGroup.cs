using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Interfaces
{
    public interface IMondayGroup
    {
        /// <summary>
        /// Is the group archived or not.
        /// </summary>
        bool Archived { get; set; }

        /// <summary>
        /// The group's color.
        /// </summary>
        string Color { get; set; }

        /// <summary>
        /// Is the group deleted or not.
        /// </summary>
        bool Deleted { get; set; }

        /// <summary>
        /// The group's unique identifier.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// The group's position in the board.
        /// </summary>
        string Position { get; set; }

        /// <summary>
        /// The group's title.
        /// </summary>
        string Title { get; set; };
    }
}
