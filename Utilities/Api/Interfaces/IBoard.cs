using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public interface IBoard
    {
        long Id { get; set; }

        string Name { get; set; }

        List<IColumn> Columns { get; set; }

        List<IItem> Items { get; set; }

        IColumn GetColumn(string id);
    }
}
