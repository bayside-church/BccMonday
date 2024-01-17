using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using Rock;
using Rock.Constants;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using Rock.Attribute;

using com.baysideonline.BccMonday.Utilities.Api;
using com.baysideonline.BccMonday.Models;
using Newtonsoft.Json;
using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using com.baysideonline.BccMonday.Utilities.Api.Schema;

namespace RockWeb.Plugins.com_baysideonline.BccMondayUI
{

    [DisplayName("BccMondayItemDetail")]
    [Category("BccMonday")]
    [Description("Displays the details of a Monday.com item")]
    [BooleanField("BccMonday Debug",
        Name = "Enable Debug Mode",
        Key = PageParameterKey.BccMondayDebug,
        Description = "Enabling this will generate exceptions.This is very useful for troubleshooting.",
        IsRequired = true,
        DefaultBooleanValue = false
        )]

    public partial class BccMondayItemDetail : RockBlock
    {
        #region PageParameterKeys
        private static class PageParameterKey
        {
            public const string BccMondayBoardId = "BccMondayBoardId";
            public const string MondayItemId = "MondayItemId";
            public const string BccMondayDebug = "BccMondayDebug";
        }
        #endregion

        #region Fields

        #endregion

        #region Properties

        // set to the value of the page parameter
        public long? MondayItemId { get; set; }

        // set to the value of the item from monday api
        public IItem Item { get; set; }

        public long? BccMondayBoardId { get; set; }

        // set to the value of the board from the database
        public BccMondayBoard Board { get; set; }
        public List<AbstractColumnValue> ItemColumns { get; private set; }

        public int? StatusIndex { get; set; }

        #endregion

        #region Base Control Methods

        // called the first time the block is loaded to the page
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.BlockUpdated += Block_BlockUpdated;

            const bool _canApproveItem = false;
            const bool _canCloseItem = false;
            bbtnApprove.Visible = _canApproveItem;
            bbtnApprove.Visible = _canCloseItem;
        }

        // called on each load of the block
        protected override void OnLoad(EventArgs e)
        {
            dAlertError.Visible = false; // error hidden by default

            if (IsPageParamIdValid(PageParameterKey.MondayItemId))
            {
                MondayItemId = long.Parse(PageParameter(PageParameterKey.MondayItemId));
            }
            else
            {
                MondayItemId = null;
            }

            if (IsPageParamIdValid(PageParameterKey.BccMondayBoardId))
            {
                BccMondayBoardId = long.Parse(PageParameter(PageParameterKey.BccMondayBoardId));
            }
            else
            {
                BccMondayBoardId = null;
            }


            if (!IsPostBack)
            {
                if (GetBoard() != null)
                {
                    ShowDetail();
                }
                else
                {
                    ShowError("No BccMondayBoard was found for this item");

                }
            }

            base.OnLoad(e);
        }

        protected bool IsPageParamIdValid(string pageParamIdString)
        {
            return PageParameter(pageParamIdString) != string.Empty && PageParameter(pageParamIdString).All(char.IsDigit);
        }
        /*
        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            if (ViewState["MondayItem"] != null && !ViewState["MondayItem"].Equals("-1"))
            {
                var mondayItem = (string)ViewState["MondayItem"];
                Item = JsonConvert.DeserializeObject<Item>(mondayItem);
            }
            if (ViewState["MondayBoard"] != null && !ViewState["MondayBoard"].Equals("-1"))
            {
                var mondayBoard = (string)ViewState["MondayBoard"];
                Board = JsonConvert.DeserializeObject<BccMondayBoard>(mondayBoard);
            }
            if (ViewState["MondayColumns"] != null && !ViewState["MondayColumns"].Equals("-1"))
            {

                var mondayColumns = (string)ViewState["MondayColumns"];
                ItemColumns = JsonConvert.DeserializeObject<List<AbstractColumnValue>>(mondayColumns, new ColumnValueConverter());
            }

        }
        
        protected override object SaveViewState()
        {
            string mondayItem = JsonConvert.SerializeObject(Item);
            ViewState["MondayItem"] = mondayItem;

            string mondayBoard = JsonConvert.SerializeObject(Board);
            ViewState["MondayBoard"] = mondayBoard;

            string mondayColumns = JsonConvert.SerializeObject(ItemColumns);
            ViewState["MondayColumns"] = mondayColumns;

            return base.SaveViewState();
        }
        */
        #endregion

        #region Events

        /// <summary>
        /// Displays the textbox for updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void bbtnNewUpdateOpen_Click(object sender, EventArgs e)
        {
            bbtnNewUpdateCancel.Visible = true;
            bbtnNewUpdateSave.Visible = true;
            bbtnNewUpdateOpen.Visible = false;
            tbNewUpdate.Visible = true;
            fsFile.Visible = true;
            ShowDetail();
        }


        /// <summary>
        /// Posts a new update to Monday.com
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>These updates are allowed to upload files</remarks>
        protected void bbtnNewUpdateSave_Click(object sender, EventArgs e)
        {
            if (tbNewUpdate.Text.IsNotNullOrWhiteSpace())
            {
                using (var context = new RockContext())
                {
                    var api = new MondayApi();

                    var mondayItemId = GetItem().Id;
                    var updateBody = tbNewUpdate.Text;
                    var updateBodyWithUser = updateBody.Insert(0, string.Format("\n[[{0}]] ", CurrentPerson.Email));

                    var sanitizedUpdateBody = updateBodyWithUser.Replace(@"""", @"\""");
                    sanitizedUpdateBody = sanitizedUpdateBody.Replace(@"\", @"\\");

                    var newUpdate = api.AddUpdateToItem(mondayItemId, sanitizedUpdateBody);
                    var updatePosted = newUpdate != null;

                    DebugLogException(string.Format("Monday Item Id: {0}, " +
                        "Current Person: {1}, " +
                        "Email: {2}, " +
                        "Update Posted: {3}, " +
                        "Sanitized Update Body: {4}", mondayItemId, CurrentPerson.FullName, CurrentPerson.Email, updatePosted, sanitizedUpdateBody));

                    if (updatePosted)
                    {
                        IAsset fileUploaded = null;
                        if (fsFile.BinaryFileId.HasValue)
                        {
                            var binaryFileId = fsFile.BinaryFileId.Value;
                            var binaryFileService = new BinaryFileService(context);
                            var binaryFile = binaryFileService.Get(binaryFileId);
                            var fileApi = new MondayApi(MondayApiType.File);
                            fileUploaded = fileApi.AddFileToUpdate(newUpdate.Id, binaryFile);
                            if (fileUploaded == null)
                            {
                                ShowError("Sorry, we were unable to upload the file you selected.");
                                var fileError = string.Format("The file was unable to upload: File Name: {0}," +
                                    " File Size: {1}, " +
                                    " Monday.com Update Id: {2}," +
                                    " Monday.com Item: {{ Name: {3}, Id: {4} }}" +
                                    " Person: {{ Name: {5}, Email: {6} }}",
                                    binaryFile.FileName, binaryFile.FileSize, newUpdate.Id, Item.Name, Item.Id, CurrentPerson.FullName, CurrentPerson.Email);
                                ExceptionLogService.LogException(new Exception(fileError, new Exception("BccMonday")));
                            }
                            binaryFileService.Delete(binaryFile);
                            DebugLogException(string.Format("binaryFile: {0} " +
                                "Id: {1} " +
                                "deleted in Method: bbtnNewUpdateSave_Click() " +
                                "by {2}" +
                                "with email: {3}", binaryFile.FileName, binaryFile.Id, CurrentPerson.FullName, CurrentPerson.Email));
                            context.SaveChanges();
                            fsFile.Controls.Clear();
                        }

                        var files = new List<IAsset>();
                        if (fileUploaded != null)
                        {
                            files.Add(fileUploaded);
                        }

                        newUpdate.Assets = new List<Asset> { (Asset)fileUploaded };
                        newUpdate.Replies = new List<IUpdate>();

                        GetItem().Updates.Insert(0, newUpdate);
                    }
                }

                tbNewUpdate.Text = string.Empty;
                tbNewUpdate.Visible = false;
                bbtnNewUpdateCancel.Visible = false;
                bbtnNewUpdateSave.Visible = false;
                bbtnNewUpdateOpen.Visible = true;
                fsFile.Visible = false;
                ShowDetail();
            }
            else
            {
                string errorMessage = "You must include a comment to upload a file.";
                maUpdateComment.Show(errorMessage, ModalAlertType.Information);
                rptColumns.DataSource = ItemColumns;
                rptColumns.DataBind();
            }
        }

        /// <summary>
        /// Hides the textbox for updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void bbtnNewUpdateCancel_Click(object sender, EventArgs e)
        {
            tbNewUpdate.Visible = false;
            bbtnNewUpdateCancel.Visible = false;
            bbtnNewUpdateSave.Visible = false;
            bbtnNewUpdateOpen.Visible = true;
            tbNewUpdate.Text = string.Empty;
            fsFile.Visible = false;

            if (fsFile.BinaryFileId.HasValue)
            {
                using (var context = new RockContext())
                {
                    var binaryFileId = fsFile.BinaryFileId.Value;
                    var binaryFileService = new BinaryFileService(context);
                    var binaryFile = binaryFileService.Get(binaryFileId);
                    binaryFileService.Delete(binaryFile);
                    DebugLogException(string.Format("binaryFile: {0} " +
                                "Id: {1} " +
                                "deleted in Method: bbtnNewUpdateCancel_Click() " +
                                "by {2}" +
                                "with email: {3}", binaryFile.FileName, binaryFile.Id, CurrentPerson.FullName, CurrentPerson.Email));
                    context.SaveChanges();
                    fsFile.Controls.Clear();
                }
            }

            tbNewUpdate.Text = string.Empty;
            ShowDetail();
        }

        /// <summary>
        /// Handles Item Commands for the Update Repeaters
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        /// <remarks>Responsible for handling open/close/save commands for Replies.</remarks>
        protected void rptUpdates_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            RepeaterItem item = e.Item;

            switch (e.CommandName)
            {
                case "ReplyOpen":
                    HandleReplyOpen(item);
                    break;

                case "ReplySave":
                    HandleReplySave(item);
                    ShowDetail();
                    break;

                case "ReplyCancel":
                    HandleReplyCancel(item);
                    break;
            }
        }

        /// <summary>
        /// Uploads a file to a brand new update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void fsFile_FileUploaded(object sender, FileUploaderEventArgs e)
        {
            rptColumns.DataSource = ItemColumns;
            rptColumns.DataBind();
        }

        /// <summary>
        /// Removes the file from the update and delete it from the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void fsFile_FileRemoved(object sender, FileUploaderEventArgs e)
        {
            var binaryFileId = e.BinaryFileId;
            if (binaryFileId.HasValue)
            {
                using (var context = new RockContext())
                {
                    var binaryFileService = new BinaryFileService(context);
                    var binaryFile = binaryFileService.Get(binaryFileId.Value);
                    binaryFileService.Delete(binaryFile);
                    DebugLogException(string.Format("binaryFile: {0} " +
                                "Id: {1} " +
                                "deleted in Method: fsFile_FileRemoved() " +
                                "by {2}" +
                                "with email: {3}", binaryFile.FileName, binaryFile.Id, CurrentPerson.FullName, CurrentPerson.Email));
                    context.SaveChanges();
                }
                rptColumns.DataSource = ItemColumns;
                rptColumns.DataBind();
            }
            rptColumns.DataSource = ItemColumns;
            rptColumns.DataBind();
        }


        /// <summary>
        ///  Handles the close button click event
        /// </summary>
        /// <remarks> Sets the status column in Monday.com to the configured closed value</remarks>
        protected void bbtnClose_Click(object sender, EventArgs e)
        {
            using (var context = new RockContext())
            {
                var api = new MondayApi();


                var board = GetBoard();
                var item = GetItem();
                var columnValue = api.ChangeColumnValue(item.Board.Id, item.Id,
                    board.MondayStatusColumnId, board.MondayStatusClosedValue);
                if (columnValue == null)
                {
                    ShowError("Api Error: Unable to close item");
                    var closeError = string.Format("Unable to close item: " +
                        "Item: {{ Name: {0}, Id: {1}, Board Id: {2}, Board Name: {3} }}; " +
                        "Closed Status: {4}; Status Column Id: {5}",
                        item.Name, item.Id, board.MondayBoardId, board.MondayBoardName,
                        board.MondayStatusClosedValue, board.MondayStatusColumnId);
                    ExceptionLogService.LogException(new Exception(closeError, new Exception("BccMonday")));
                }
            }
            Item = null;
            ShowDetail();
        }

        /// <summary>
        /// Handles the approve button click event.
        /// </summary>
        /// <remarks> Sets the status column in Monday.com to the configured approved value</remarks>
        protected void bbtnApprove_Click(object sender, EventArgs e)
        {


            using (var context = new RockContext())
            {
                var api = new MondayApi();

                var board = GetBoard();
                var item = GetItem();
                var columnValue = api.ChangeColumnValue(item.Board.Id, item.Id,
                    board.MondayStatusColumnId, board.MondayStatusApprovedValue);
                if (columnValue == null)
                {
                    ShowError("Api Error: Unable to approve item");
                    var approveError = string.Format("Unable to approve item: " +
                        "Item: {{ Name: {0}, Id: {1}, Board Id: {2}, Board Name: {3} }}; " +
                        "Approve Status: {4}; Status Column Id: {5}",
                        item.Name, item.Id, board.MondayBoardId, board.MondayBoardName,
                        board.MondayStatusApprovedValue, board.MondayStatusColumnId);
                    ExceptionLogService.LogException(new Exception(approveError, new Exception("BccMonday")));
                }
                DebugLogException(string.Format("Status: {0}", columnValue.Text));
            }
            Item = null;
            ShowDetail();
        }

        /// <summary>
        /// Binds the Repeater Columns to the Monday.com Item's column values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Creates the control specific to the type of AbstractColumnValue</remarks>
        protected void rptColumns_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var item = e.Item;
            var dataItem = item.DataItem as AbstractColumnValue;

            item.Controls.Add(dataItem.CreateControl(Page));
        }

        // handles BlockUpdated event of the control
        protected void Block_BlockUpdated(object sender, EventArgs e)
        {

        }


        #endregion

        #region Methods

        /// <summary>
        /// Displays a modal alert with error message
        /// </summary>
        public void ShowError(string message)
        {
            pnlItem.Visible = false;
            dAlertError.Visible = true;
            lError.Text = message;
        }

        /// <summary>
        /// Shows the detail view of the item
        /// </summary>
        /// <remarks> Binds the column values, updates grids, and sets title</remarks>
        public void ShowDetail()
        {
            using (var context = new RockContext())
            {
                if (GetItem() != null)
                {
                    lActionTitle.Text = Item.Name + " - " + Item.Board.Name;
                    sCreatedAt.InnerText = Item.CreatedAt.ToString("MMM d, yyyy");
                    if (GetBoard() != null)
                    {
                        bbtnApprove.Visible = ShowStatusButton(Board.MondayStatusApprovedValue) && Board.ShowApprove;
                        bbtnClose.Visible = ShowStatusButton(Board.MondayStatusClosedValue);
                    }

                    // Find all column values associated with that Monday Board
                    var displayColumns = new BccMondayBoardDisplayColumnService(context)
                        .Queryable()
                        .Where(c => c.BccMondayBoard.MondayBoardId == Board.MondayBoardId)
                        .ToList();

                    var valuesToDisplay = Item.ColumnValues
                        .Where(c => displayColumns.Find(d => d.MondayColumnId.Equals(c.ColumnId)) != null)
                        .ToList();

                    ItemColumns = valuesToDisplay;
                    rptColumns.DataSource = valuesToDisplay;
                    rptColumns.DataBind();

                    rptUpdates.DataSource = Item.Updates.ToList();
                    rptUpdates.DataBind();
                }
                else
                {
                    ShowError("Error: This Monday.com Item does not exist.");
                }
            }
        }

        /// <summary>
        /// Displays the textbox for replies.
        /// </summary>
        /// <param name="parentUpdate">The parent update</param>
        protected void HandleReplyOpen(RepeaterItem parentUpdate)
        {
            rptColumns.DataSource = ItemColumns;
            rptColumns.DataBind();
            ((RockTextBox)parentUpdate.FindControl("tbNewReply")).Visible = true;
            ((BootstrapButton)parentUpdate.FindControl("bbtnNewReplySave")).Visible = true;
            ((BootstrapButton)parentUpdate.FindControl("bbtnNewReplyCancel")).Visible = true;
            ((BootstrapButton)parentUpdate.FindControl("bbtnNewReplyOpen")).Visible = false;
        }

        /// <summary>
        /// Saves a new reply to an existing update in Monday.com
        /// </summary>
        /// <param name="parentUpdate">The parent update</param>
        /// <remarks> Replies cannot upload in files, but parent updates can.</remarks>
        protected void HandleReplySave(RepeaterItem parentUpdate)
        {
            var control = ((RockTextBox)parentUpdate.FindControl("tbNewReply"));
            var commentText = control.Text;

            var sanitizedCommentText = commentText.Replace(@"""", @"\""");

            var commentTextWithUser = sanitizedCommentText.Insert(0,
                string.Format("\n[[{0}]] ",
                CurrentPerson.Email));

            if (commentText.IsNotNullOrWhiteSpace())
            {
                using (var context = new RockContext())
                {
                    var api = new MondayApi();
                    var itemId = GetItem().Id;
                    var updateId = GetItem().Updates[parentUpdate.ItemIndex].Id;

                    var newReply = (api.AddUpdateToItem(itemId, commentTextWithUser, updateId));
                    var replyPosted = newReply != null;

                    if (replyPosted)
                    {
                        // add update to local state
                        GetItem().Updates[parentUpdate.ItemIndex].Replies.Add(newReply);
                    }
                }
            }
            ((RockTextBox)parentUpdate.FindControl("tbNewReply")).Text = string.Empty;
            ((RockTextBox)parentUpdate.FindControl("tbNewReply")).Visible = false;
            ((BootstrapButton)parentUpdate.FindControl("bbtnNewReplySave")).Visible = false;
            ((BootstrapButton)parentUpdate.FindControl("bbtnNewReplyCancel")).Visible = false;
            ((BootstrapButton)parentUpdate.FindControl("bbtnNewReplyOpen")).Visible = true;
        }

        /// <summary>
        /// Hides the Reply textbox on "Cancel" click
        /// </summary>
        /// <param name="parentUpdate">The parent update</param>
        protected void HandleReplyCancel(RepeaterItem parentUpdate)
        {
            ShowDetail();
            ((RockTextBox)parentUpdate.FindControl("tbNewReply")).Text = string.Empty;
            ((RockTextBox)parentUpdate.FindControl("tbNewReply")).Visible = false;
            ((BootstrapButton)parentUpdate.FindControl("bbtnNewReplySave")).Visible = false;
            ((BootstrapButton)parentUpdate.FindControl("bbtnNewReplyCancel")).Visible = false;
            ((BootstrapButton)parentUpdate.FindControl("bbtnNewReplyOpen")).Visible = true;
        }

        ///<summary>
        ///Gets a list of replies from an update object
        ///</summary>
        protected List<IUpdate> GetReplies(object update)
        {
            IUpdate u = update as Update;
            return u?.Replies;
        }

        /// <summary>
        /// Gets a list of files from an update
        /// </summary>
        /// <param name="update">The Monday.com update</param>
        /// <returns></returns>
        protected List<Asset> GetFiles(object update)
        {
            IUpdate u = update as Update;
            return u?.Assets;
        }

        // gets the bread crumbs for this item
        public override List<BreadCrumb> GetBreadCrumbs(Rock.Web.PageReference pageReference)
        {
            var breadCrumbs = new List<BreadCrumb>();
            var crumbName = "Item";

            if (GetItem() != null)
                crumbName = ActionTitle.Add(GetItem().Name);


            MondayItemId = PageParameter("MondayItemId").AsIntegerOrNull();

            if (MondayItemId.HasValue)
            {
                breadCrumbs.Add(new BreadCrumb(crumbName, pageReference));
            }

            return breadCrumbs;
        }

        /// <summary>
        /// Gets the Item instance
        /// </summary>
        /// <remarks> Loads Item from API if it has not been loaded already.</remarks>
        /// <returns>The Item specified by the page parameter </returns>
        protected IItem GetItem()
        {
            if (Item != null)
                return Item;

            using (var context = new RockContext())
            {
                var api = new MondayApi();

                if (MondayItemId.HasValue)
                {
                    // TODO(Noah): MondayItemId null check
                    Item = api.GetItem(MondayItemId.Value);
                    var automationUpdates = Item.Updates.Where(u => u.Creator.CreatorId.Equals("-4")).ToList();
                    Item.Updates.RemoveAll(u => u.Creator.CreatorId.Equals("-4"));
                }

                return Item;
            }
        }

        /// <summary>
        /// Gets the BccMondayBoard instance
        /// </summary>
        /// <remarks> Loads Board from database if it has not been loaded already.</remarks>
        /// <returns>The Board associated with the current Item </returns>
        protected BccMondayBoard GetBoard()
        {
            if (Board != null)
                return Board;

            var item = GetItem();
            if (item != null && BccMondayBoardId.HasValue)
            {
                using (var context = new RockContext())
                {
                    var boardService = new BccMondayBoardService(context);
                    var mondayBoardId = item.Board.Id;

                    var board = boardService
                        .Queryable()
                        .FirstOrDefault(b => b.MondayBoardId == BccMondayBoardId.Value);
                    Board = board;
                }

                return Board;
            }

            return null;
        }

        /// <summary>
        /// Determines whether to show Closed/Approved/Both buttons.
        /// </summary>
        /// <param name="statusValue">The searched for status value</param>
        /// <returns>
        /// <c>true</c> if the current status does not equal the searched for value.
        /// <c>false</c> if the status Value is empty, matches, or does not exist.
        /// </returns>
        protected bool ShowStatusButton(string statusValue)
        {
            if (statusValue.IsNullOrWhiteSpace()) return false;

            var statusIndex = Item.ColumnValues.FindIndex(c => c.ColumnId == Board.MondayStatusColumnId);
            if (statusIndex != -1)
            {
                var status = Item.ColumnValues[statusIndex].Text;
                return !status.Equals(statusValue);
            }
            return false;

        }
        #endregion

        /// <summary>
        /// Binds the status color to each Monday.com update.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptUpdates_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var update = e.Item.FindControl("mondayUpdate") as Panel;
            update?.Style.Add("border-color", GetStatusColor());
        }

        private string GetStatusColor()
        {
            if (GetBoard() != null && GetItem() != null)
            {
                StatusIndex = Item.ColumnValues.FindIndex(c => c.ColumnId == Board.MondayStatusColumnId);
                var statusColumnValue = Item.ColumnValues[StatusIndex.Value];
                var currentStatus = statusColumnValue.Text;

                var currentColor = ((StatusColumnValue)statusColumnValue).LabelStyle.Color;
                return currentColor;
            }
            return string.Empty;
        }

        private void DebugLogException(string debugLog)
        {
            var debugFlag = GetAttributeValue(PageParameterKey.BccMondayDebug).AsBoolean();
            if (debugFlag)
            {
                ExceptionLogService.LogException(new Exception(debugLog,
                        new Exception("BccMonday Debug")));
            }
        }
    }
}
