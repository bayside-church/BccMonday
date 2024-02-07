using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Interfaces
{
    public interface IMondayTag
    {
        /// <summary>
        /// The tag's color.
        /// </summary>
        string Color { get; set; }

        /// <summary>
        /// The tag's unique identifier
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// The tag's name.
        /// </summary>
        string Name { get; set; }
    }
}
