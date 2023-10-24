using Rock.Model;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    interface IMondayFileApi
    {
        MondayInitializeResponse Initialize();
        MondayApiResponse<IFile> AddFileToColumn(long itemId, string columnId, BinaryFile binaryFile);
        MondayApiResponse<IFile> AddFileToUpdate(long updateId, BinaryFile binaryFile);

    }
}
