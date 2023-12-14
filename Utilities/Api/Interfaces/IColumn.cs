using Newtonsoft.Json;
using System;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public interface IColumn : IEquatable<IColumn>
    {
        string Id { get; set; }

        string Title { get; set; }

        string Type { get; set; }

        bool Equals(object obj);
        int GetHashCode();
    }
}
