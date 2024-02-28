using com.baysideonline.BccMonday.Utilities.Api.Schema;
using Newtonsoft.Json;
using System;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public interface IAsset
    {
        /// <summary>
        /// The file's unique identifier.
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// The file's size in bytes.
        /// </summary>
        long Size { get; set; }

        /// <summary>
        /// The file's name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// public url to the asset, valid for 1 hour.
        /// </summary>
        string PublicUrl { get; set; }

        /// <summary>
        /// url to view the asset in thumbnail mode. Only available for images.
        /// </summary>
        string UrlThumbnail { get; set; }

        /// <summary>
        /// The file's creation date.
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// The file's extension
        /// </summary>
        int FileExtension { get; set; }

        /// <summary>
        /// original geometry of the asset.
        /// </summary>
        string OriginalGeometry { get; set; }

        /// <summary>
        /// The user who uploaded the file
        /// </summary>
        MondayUser Uploader { get; set; }

        /// <summary>
        /// url to view the asset
        /// </summary>
        string Url { get; set; }
    }
}
