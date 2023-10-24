namespace com.baysideonline.BccMonday.Utilities.Api
{
    public enum MondayStatuses
    {
        OK,
        ERROR,
        UNAUTHORIZED,   //Ensure your API key is valid and passed in the 'Authorization' header
        BADREQUEST,     //The structure of your query string was passed incorrectly
        INTERNALSERVERERROR,    //Some formatting within your query string is incorrect
        PARSEERROR,     //formatting is incorect. ensure valid string and all brackets are closed
        USERUNAUTHORIZEDEXCEPTION,  //Check is user has permission to access or edit the given resource
        INVALIDCOLUMNIDEXCEPTION,   //Column ID is invalid. Check that you have access
        INVALIDBOARDIDEXCEPTION,    //Board ID is invalid. Check that you have access
        CREATEBOARDEXCEPTION,       //Error in creating board
        RESOURCENOTFOUNDEXCEPTION,  //Ensure the ID of the item, group, or board you're querying exists
        ITEMSLIMITATIONEXCEPTION,   //10,000 item limit on a board
        ITEMNAMETOOLONGEXCEPTION,   //Ensure item name is between 1 and 255 characters
        COLUMNVALUEEXCEPTION,       
        CORRECTEDVALUEEXCEPTION
    }
}
