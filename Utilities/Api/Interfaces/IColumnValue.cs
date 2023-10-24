
namespace com.baysideonline.BccMonday.Utilities.Api
{
    public interface IColumnValue
    {
        string ColumnId { get; set; }

        string Text { get; set; }

        string Title { get; set; }

        string Type { get; set; }
        
        string Value { get; set; }

        string AdditionalInfo { get; set; }
    }
}
