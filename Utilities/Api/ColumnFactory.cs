
namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class ColumnFactory
    {
        public IColumn Create(dynamic data)
        {
            if (data != null)
            {
                var type = data["type"].Value;

                IColumn column = type is "color" ? new ColorColumn() : new Column();
                return column;
            }

            return null;
        }
    }
}
