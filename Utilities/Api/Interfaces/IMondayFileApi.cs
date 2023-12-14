using Rock.Model;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    interface IMondayFileApi
    {
        MondayInitializeResponse Initialize();
        MondayApiResponse<IAsset> AddFileToColumn(long itemId, string columnId, BinaryFile binaryFile);
        MondayApiResponse<IAsset> AddFileToUpdate(long updateId, BinaryFile binaryFile);

    }
}
