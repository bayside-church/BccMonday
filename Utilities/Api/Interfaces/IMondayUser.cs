namespace com.baysideonline.BccMonday.Utilities.Api.Interfaces
{
    public interface IMondayUser
    {
        /// <summary>
        /// The user's name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The user's unique identifier
        /// </summary>
        string CreatorId { get; set; }
    }
}
