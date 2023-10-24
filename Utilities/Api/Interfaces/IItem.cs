using System;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public interface IItem
    {
        long Id { get; set; }

        string Name { get; set; }

        DateTime CreatedAt { get; set; }

        IBoard Board { get; set; }

        long? BoardId { get; set; }

        List<Interfaces.AbstractColumnValue> ColumnValues { get; set; }

        List<IUpdate> Updates { get; set; }

        string GetRequestorEmail(string emailMatchColumnId);
    }
}
