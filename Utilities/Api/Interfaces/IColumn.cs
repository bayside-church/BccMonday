using Newtonsoft.Json;
using System;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public interface IColumn : IEquatable<IColumn>
    {
        /// <summary>
        /// The column's unique identifier.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// The column's title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// The column's type.
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// The column's settings in a string form.
        /// </summary>
        string Options { get; set; }

        bool Equals(object obj);
        int GetHashCode();
    }
}
