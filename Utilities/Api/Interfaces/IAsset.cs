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
    }
}
