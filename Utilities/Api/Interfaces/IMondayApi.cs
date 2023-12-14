using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api
{

    public class MondayApiResponse<T>
    {

        public MondayStatuses Status { get; set; }

        public T Data { get; set; }

        public bool IsOk()
        {
            return Status == MondayStatuses.OK;
        }

        public static MondayApiResponse<T> CreateOkResponse(T data)
        {
            return new MondayApiResponse<T>()
            {
                Status = MondayStatuses.OK,
                Data = data
            };
        }

        public static MondayApiResponse<T> CreateErrorResponse()
        {
            return new MondayApiResponse<T>()
            {
                Status = MondayStatuses.ERROR,
                Data = default(T)
            };
        }
    }

    public class MondayInitializeResponse
    {

        public MondayStatuses Status { get; set; }

        public string Message { get; set; }

        public bool IsOk()
        {
            return Status == MondayStatuses.OK;
        }
    }

    public interface IMondayApi
    {
        MondayInitializeResponse Initialize();

        IItem GetItem(long id);

        List<IItem> GetItemsByBoard(long boardId, string emailMatchColumnId, string statusColumnId);

        IUpdate AddUpdateToItem(long itemId, string body, long? parentUpdateId = null);

        List<IAsset> GetFilesByAssetIds(List<long> ids);

        bool ChangeColumnValue(long boardId, long itemId, string columnId, string newValue);

        IBoard GetBoard(long id);

        List<IBoard> GetBoards();
    }
}
