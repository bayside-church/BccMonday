using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using System;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public interface IUpdate
    {
        /// <summary>
        /// The update's unique identifier.
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// The update's html formatted body.
        /// </summary>
        string Body { get; set; }

        /// <summary>
        /// The update's text body.
        /// </summary>
        string TextBody { get; set; }

        /// <summary>
        /// The update's creation date.
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// The Creator's Name.
        /// </summary>
        string CreatorName { get; }

        /// <summary>
        /// The unique identifier of the update creator.
        /// </summary>
        string CreatorId { get; set; }

        /// <summary>
        /// The update's assets/files.
        /// </summary>
        List<IAsset> Assets { get; set; }

        /// <summary>
        /// The update's replies.
        /// </summary>
        List<IUpdate> Replies { get; set; }

        /// <summary>
        /// The update's creator.
        /// </summary>
        IMondayUser Creator { get; set; }
    }
}
