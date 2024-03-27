using com.baysideonline.BccMonday.Utilities.Api.Config;
using com.baysideonline.BccMonday.Utilities.Api.Schema;
using Rock.Model;
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

        Item GetItem(long id);

        List<Item> GetItemsByBoard(long boardId, string emailMatchColumnId, string statusColumnId);

        Update AddUpdateToItem(long itemId, string body, long? parentUpdateId = null);

        List<Asset> GetFilesByAssetIds(List<long> ids);

        StatusColumnValue ChangeColumnValue(ColumnChangeOptions options);

        Board GetBoard(long id);

        List<Board> GetBoards();

        Asset AddFileToUpdate(long updateId, BinaryFile binaryFile);
    }
}
