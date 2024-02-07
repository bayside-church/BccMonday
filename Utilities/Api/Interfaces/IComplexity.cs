using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api.Interfaces
{
    public interface IComplexity
    {
        /// <summary>
        /// The remainder of complexity after the query's execution.
        /// </summary>
        int After { get; set; }

        /// <summary>
        /// The remainder of complexity before the query's execution.
        /// </summary>
        int Before { get; set; }

        /// <summary>
        /// The specific query's complexity
        /// </summary>
        int Query { get; set; }

        /// <summary>
        /// How long in seconds before the complexity budget is reset.
        /// </summary>
        int ResetInXSeconds { get; set; }
    }
}
