using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Web.UI.WebControls;

using Rock;
using Rock.Data;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using com.baysideonline.BccMonday.Models;
using com.baysideonline.BccMonday.Utilities.Api;
using Newtonsoft.Json;
using Rock.Security;
using Rock.Model;
using com.baysideonline.BccMonday.Utilities.Api.Schema;

namespace RockWeb.Plugins.com_baysideonline.BccMondayUI
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName("BccMondayBoardDetail")]
    [Category("BccMonday")]
    [Description("Display and edit the details of a BccMondayBoard entity.")]
    [LinkedPage("ListPage")]
    [BooleanField("BccMonday Debug",
        Name = "Enable Debug Mode",
        Key = PageParameterKey.BccMondayDebug,
        Description = "Enabling this will generate exceptions.This is very useful for troubleshooting.",
        IsRequired = true,
        DefaultBooleanValue = false
        )]
    public partial class BccMondayBoardDetail : Rock.Web.UI.RockBlock
    {

        #region Attribute Keys

        #endregion Attribute Keys

        #region PageParameterKeys

        private static class PageParameterKey
        {
            public const string BccMondayBoardId = "BccMondayBoardId";
            public const string BccMondayDebug = "BccMondayDebug";
        }

        #endregion PageParameterKeys

        #region Fields
        private bool _canAddEditDelete = false;
        #endregion

        #region Properties

        private List<BccMondayBoardDisplayColumn> DisplayColumns { get; set; }
        private List<IColumn> boardColumnsList { get; set; }

        private int? BccMondayBoardId { get; set; }

        #endregion

        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            gEditDisplayColumns.DataKeyNames = new[] { "Id" };
            gEditDisplayColumns.GridRebind += gEditdisplayDisplayColumns_GridRebind;
            gEditDisplayColumns.Actions.ShowAdd = true;
            gEditDisplayColumns.Actions.AddClick += gEditDisplayColumns_Add;

            // This event gets fired after block settings are updated. It's nice to repaint the screen if these settings would alter it.
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger(upnlContent);

            //Check User Permissions
            _canAddEditDelete = IsUserAuthorized(Authorization.EDIT);
            lbEdit.Visible = _canAddEditDelete;
            lbDelete.Visible = _canAddEditDelete;
            btnSync.Visible = _canAddEditDelete;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            BccMondayBoardId = PageParameter(PageParameterKey.BccMondayBoardId).AsIntegerOrNull();

            //if (BccMondayBoardId.HasValue)
            DisplayColumns = GetDisplayColumns();

            if (!Page.IsPostBack)
            {
                if (BccMondayBoardId.HasValue && BccMondayBoardId.Value != 0)
                    ShowDetail();
                else if (BccMondayBoardId.HasValue && BccMondayBoardId.Value == 0)
                    ShowEditDetails(null);
                else
                    upnlContent.Visible = false;
            }
        }

        protected override object SaveViewState()
        {
            string json = JsonConvert.SerializeObject(DisplayColumns);
            ViewState["DisplayColumns"] = json;
            return base.SaveViewState();
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);

            if (ViewState["DisplayColumns"] != null && !ViewState["DisplayColumns"].Equals("-1"))
            {
                string json = (string)ViewState["DisplayColumns"];
                DisplayColumns = JsonConvert.DeserializeObject<List<BccMondayBoardDisplayColumn>>(json);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles the lbEdit_Click event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbEdit_Click(object sender, EventArgs e)
        {
            ShowEditDetails(GetBoard());
        }

        /// <summary>
        /// Handles the lbDelete_Click
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbDelete_Click(object sender, EventArgs e)
        {
            var context = new RockContext();
            var boardService = new BccMondayBoardService(context);
            var boardDisplayService = new BccMondayBoardDisplayColumnService(context);

            var board = boardService
                .Queryable()
                .FirstOrDefault(b => b.Id == BccMondayBoardId.Value);

            if (board?.BoardDisplayColumns != null)
            {
                var displayColumns = board.BoardDisplayColumns;
                boardDisplayService.DeleteRange(displayColumns);
            }
            boardService.Delete(board);
            DebugLogException(string.Format("board: {0} " +
                                "Id: {1} " +
                                "deleted in Method: lbDelete_Click() " +
                                "by {2}" +
                                "with email: {3}", board.MondayBoardName, board.Id, CurrentPerson.FullName, CurrentPerson.Email));
            context.SaveChanges();
            NavigateToLinkedPage("ListPage");
        }

        /// <summary>
        /// Handles the lbSave_Click event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbSave_Click(object sender, EventArgs e)
        {
            var context = new RockContext();
            var boardService = new BccMondayBoardService(context);
            dAlertError.Visible = false;
            if (BccMondayBoardId.HasValue)
            {
                var mondayBoardName = dtbMondayBoardName.Text;
                var mondayBoardIdStr = BccMondayBoardId != 0 ?
                    tbEditMondayBoardId.Text : ddlMondayBoardId.SelectedValue;
                var emailMatchColumnId = ddlEmailMatchColumnId.SelectedValue;
                var mondayStatusColumnId = ddlMondayStatusColumnId.SelectedValue;
                var mondayStatusClosedValue = ddlMondayStatusClosedValue.SelectedValue;
                var mondayStatusCompleteValue = ddlMondayStatusCompleteValue.SelectedValue;
                var mondayStatusApprovedValue = ddlMondayStatusApprovedValue.SelectedValue;
                bool mondayShowApprove = cbShowApprove.Checked;

                if (mondayBoardName.IsNullOrWhiteSpace())
                {
                    maWarning.Show("Name is required", ModalAlertType.Alert);
                    return;
                }


                var board = boardService.Queryable()
                    .Where(b => b.Id == BccMondayBoardId.Value)
                    .FirstOrDefault();

                if (!mondayBoardName.IsNotNullOrWhiteSpace())
                {
                    mondayBoardName = ddlMondayBoardId.SelectedItem.Text;
                }

                if (board != null)
                {
                    board.MondayBoardName = mondayBoardName;

                    long mondayBoardId;
                    long.TryParse(mondayBoardIdStr, out mondayBoardId);

                    board.MondayBoardId = mondayBoardId;
                    board.EmailMatchColumnId = emailMatchColumnId;
                    board.MondayStatusColumnId = mondayStatusColumnId;
                    board.MondayStatusClosedValue = mondayStatusClosedValue;
                    board.MondayStatusCompleteValue = mondayStatusCompleteValue;
                    board.MondayStatusApprovedValue = mondayStatusApprovedValue;
                    board.ShowApprove = mondayShowApprove;

                    var columnService = new BccMondayBoardDisplayColumnService(context);
                    var bccColumns = columnService.Queryable().Where(b => b.BccMondayBoardId == board.Id).ToList();

                    if (DisplayColumns != null)
                    {
                        columnService.DeleteRange(bccColumns);
                        columnService.AddRange(DisplayColumns);
                    }
                    //board.SaveAttributeValues(context);
                    board.LoadAttributes();
                    avcAttributesEdit.GetEditValues(board);
                    var attributeValues = board.AttributeValues;
                    var attributes = board.Attributes;

                    context.WrapTransaction(() =>
                    {
                        context.SaveChanges();
                        board.SaveAttributeValues(context);
                    });
                    DebugLogException(string.Format("{0} bccColumns  " +
                                "deleted in Method: lbSave_Click() " +
                                "by {1}" +
                                "with email: {2}", bccColumns.Count, CurrentPerson.FullName, CurrentPerson.Email));
                }
                else if (BccMondayBoardId.Value == 0)
                {
                    var newBoard = new BccMondayBoard()
                    {
                        MondayBoardName = mondayBoardName,
                        MondayBoardId = long.Parse(mondayBoardIdStr), // TODO: handle parse error
                        EmailMatchColumnId = emailMatchColumnId,
                        MondayStatusColumnId = mondayStatusColumnId,
                        MondayStatusClosedValue = mondayStatusClosedValue,
                        MondayStatusCompleteValue = mondayStatusCompleteValue,
                        MondayStatusApprovedValue = mondayStatusApprovedValue,
                        ShowApprove = mondayShowApprove
                    };

                    boardService.Add(newBoard);

                    var columnService = new BccMondayBoardDisplayColumnService(context);
                    if (DisplayColumns != null)
                    {
                        columnService.AddRange(DisplayColumns);
                    }
                    context.WrapTransaction(() =>
                    {
                        context.SaveChanges();
                        newBoard.SaveAttributeValues(context);
                    });
                    BccMondayBoardId = newBoard.Id;
                }
            }

            var queryParams = new Dictionary<string, string>
            {
                ["BccMondayBoardId"] = BccMondayBoardId?.ToString()
            };

            NavigateToPage(RockPage.Guid, queryParams);
        }

        /// <summary>
        /// Handles the lbCancel_Click event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbCancel_Click(object sender, EventArgs e)
        {
            int? id = PageParameter(PageParameterKey.BccMondayBoardId).AsIntegerOrNull();
            ViewState["DisplayColumns"] = string.Empty;
            DisplayColumns = null;

            dAlertError.Visible = false;
            if (id.HasValue && id != 0)
                ShowDetail();
            else
                NavigateToParentPage();
        }

        /// <summary>
        /// Handles the dlgDisplayColumns_SaveClick event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void dlgDisplayColumns_SaveClick(object sender, EventArgs e)
        {


            var columns = lbDisplayColumns.Items.OfType<ListItem>().Where(l => l.Selected).ToList();
            var newObjects = new List<BccMondayBoardDisplayColumn>();
            foreach (var column in columns)
            {
                newObjects.Add(new BccMondayBoardDisplayColumn()
                {
                    BccMondayBoardId = BccMondayBoardId.Value,
                    MondayColumnId = column.Value,
                    MondayColumnTitle = column.Text
                });
            }

            DisplayColumns?.AddRange(newObjects);

            var board = GetBoard();
            var mondayBoardId = board?.MondayBoardId ?? long.Parse(ddlMondayBoardId.SelectedValue);
            var inColumnsButNotGrid = GetGridColumns(mondayBoardId);
            if (inColumnsButNotGrid.Count == 0)
            {
                gEditDisplayColumns.Actions.ShowAdd = false;
            }

            BindEditDisplayColumnsGrid();
            dlgDisplayColumns.Hide();
        }

        /// <summary>
        /// Synchronizes the BccMondayBoard data with Monday.com
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnSync_Click(object sender, EventArgs e)
        {
            using (var context = new RockContext())
            {
                var boardService = new BccMondayBoardService(context);

                var dbBoard = GetBoard();
                var mondayBoardId = dbBoard.MondayBoardId;

                var api = new MondayApi();
                var apiBoard = api.GetBoard(mondayBoardId);

                if (apiBoard != null)
                {
                    if (!string.Equals(dbBoard.MondayBoardName, apiBoard.Name))
                    {
                        dbBoard.MondayBoardName = apiBoard.Name;
                        context.Entry(dbBoard).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    var apiBoardEmailMatchColumn = apiBoard.Columns
                        .FirstOrDefault(c => c.Id == dbBoard.EmailMatchColumnId);

                    if (apiBoardEmailMatchColumn == null)
                    {
                        ShowError("ERROR: The Email Match Column no longer exists in Monday.com");
                        DebugLogException(String.Format("ERROR: The Email Match Column no longer exists in Monday.com " +
                            "for Board Id: {0} " +
                            "with MondayBoardId: {1}" +
                            "for User: {2} " +
                            "with Email {3}", dbBoard.Id, dbBoard.MondayBoardId, CurrentPerson.FullName, CurrentPerson.Email));
                    }

                    var displayColumnService = new BccMondayBoardDisplayColumnService(context);
                    var displayedColumnsList = displayColumnService
                        .Queryable()
                        .Where(c => c.BccMondayBoardId == BccMondayBoardId.Value)
                        .ToList();

                    var columns = apiBoard.Columns;

                    // loop over columns
                    foreach (var dbCol in displayedColumnsList)
                    {
                        var col = columns
                            .Where(c => c.Id == dbCol.MondayColumnId)
                            .FirstOrDefault();

                        if (col != null)
                        {
                            if (!string.Equals(dbCol.MondayColumnTitle, col.Title))
                            {
                                dbCol.MondayColumnTitle = col.Title;
                            }
                            if (!string.Equals(dbCol.MondayColumnType, col.Type))
                            {
                                dbCol.MondayColumnType = col.Type;
                            }
                        }
                        else
                        {
                            displayColumnService.Delete(dbCol);
                            DebugLogException(string.Format("dbCol Type: {0} " +
                                "Id: {1} " +
                                "deleted in Method: btnSync_Click() " +
                                "by {2}" +
                                "with email: {3}", dbCol.TypeName, dbCol.Id, CurrentPerson.FullName, CurrentPerson.Email));
                        }
                        context.SaveChanges();
                    }

                    ShowDetail();
                }
                else
                {
                    // TODO: api response was not ok, show error
                }
            }
        }

        /// <summary>
        /// Handles the gEditDisplayColumns_Delete for the Edit Grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">the <see cref="RowEventArgs"/> instance containing the event data.</param>
        protected void gEditDisplayColumns_Delete(object sender, RowEventArgs e)
        {
            using (var context = new RockContext())
            {
                var key = e.RowKeyId;
                var index = e.RowIndex;

                var service = new BccMondayBoardDisplayColumnService(context);
                var column = service
                    .Queryable()
                    .FirstOrDefault(c => c.Id == key);

                if (DisplayColumns != null)
                {
                    DisplayColumns.RemoveAt(index);
                    gEditDisplayColumns.Actions.ShowAdd = true;
                }

                BindEditDisplayColumnsGrid();
            }
        }

        /// <summary>
        /// Handles the gEditDisplayColumns_add event for the Edit Grid.
        /// </summary>
        /// <param name="sender">the source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void gEditDisplayColumns_Add(object sender, EventArgs e)
        {
            var board = GetBoard();
            var mondayBoardId = GetBoard() != null ? board.MondayBoardId : long.Parse(ddlMondayBoardId.SelectedValue);

            var inColumnsButNotGrid = GetGridColumns(mondayBoardId);

            if (inColumnsButNotGrid != null && inColumnsButNotGrid.Count > 0)
            {
                lbDisplayColumns.DataSource = inColumnsButNotGrid;
                lbDisplayColumns.DataBind();
                dlgDisplayColumns.Show();
            }
            else
            {
                ShowError("This board does not exist on Monday.com");
            }
        }

        protected void ddlMondayBoardId_SelectedIndexChanged(object sender, EventArgs e)
        {
            //TODO(Noah): Null check?
            long mondayBoardId = long.Parse(ddlMondayBoardId.SelectedValue);

            BindMondayColumns(mondayBoardId);
            BindEditDisplayColumnsGrid();

            dtbMondayBoardName.Text = ddlMondayBoardId.SelectedItem.Text;
        }

        protected void ddlMondayStatusColumnId_SelectedIndexChanged(object sender, EventArgs e)
        {
            long mondayBoardId;

            if (tbEditMondayBoardId.Text.IsNotNullOrWhiteSpace())
            {
                mondayBoardId = long.Parse(tbEditMondayBoardId.Text);
            }
            else
            {
                mondayBoardId = long.Parse(ddlMondayBoardId.SelectedValue);
            }
            string statusColumnId = ddlMondayStatusColumnId.SelectedValue;

            BindMondayColumnOptions(mondayBoardId, statusColumnId);
        }

        #endregion

        #region Methods


        /// <summary>
        /// Displays a modal alert with a message detailing the error
        /// </summary>
        /// <param name="message">String detailing the occurred error</param>
        public void ShowError(string message)
        {
            dAlertError.Visible = true;
            lError.Text = message;
        }

        /// <summary>
        /// Displays the details of the BccMondayBoard
        /// </summary>
        public void ShowDetail()
        {
            var context = new RockContext();

            pnlEditDetails.Visible = false;
            pnlViewDetails.Visible = true;
            var board = GetBoard();

            if (board != null)
            {
                lMondayBoardName.Text = board.MondayBoardName;
                lMondayBoardId.Text = board.MondayBoardId.ToString();
                lEmailMatchColumnId.Text = board.EmailMatchColumnId;
                lStatusColumnId.Text = board.MondayStatusColumnId;
                lTitle.Text = board.MondayBoardName;
                lApprovedStatusValue.Text = board.MondayStatusApprovedValue;
                lClosedStatusValue.Text = board.MondayStatusClosedValue;
                lCompleteStatusValue.Text = board.MondayStatusCompleteValue;
                lShowApprove.Text = board.ShowApprove ? "(Will be displayed)" : "(Not displayed)";

                lWorkSpace.Text = new MondayApi().GetWorkspace(board.MondayBoardId);
                var columns = GetDisplayColumns();
                if (columns != null && columns.Count > 0)
                {
                    lDisplayColumns.Text = string.Join(", ", columns.Select(c => c.MondayColumnTitle));
                }
                else
                {
                    lDisplayColumns.Text = "No column chosen to display";
                }

                board.LoadAttributes();
                avcAttributes.Visible = false;
                var viewableAttributes = board.Attributes.Where(a => a.Value.IsAuthorized(Authorization.VIEW, this.CurrentPerson)).Select(a => a.Key).ToList();// : new List<string>();
                if (viewableAttributes.Any())
                {
                    avcAttributes.Visible = true;
                    avcAttributes.ExcludedAttributes = board.Attributes.Where(a => !viewableAttributes.Contains(a.Key)).Select(a => a.Value).ToArray();
                    avcAttributes.AddDisplayControls(board);
                }
            }
            else
            {
                ShowError("Error: Unable to find BccMondayBoard with specified id in the database");
            }
        }

        /// <summary>
        /// Displays the edit details for the block, if authorized
        /// </summary>
        /// <param name="board"> A unique BccMondayBoard object</param>
        private void ShowEditDetails(BccMondayBoard board)
        {
            pnlEditDetails.Visible = true;
            pnlViewDetails.Visible = false;

            if (board != null)
            {
                dtbMondayBoardName.Text = board.MondayBoardName;
                tbEditMondayBoardId.Text = board.MondayBoardId.ToString();
                tbEditMondayBoardId.Visible = true;
                tbEditMondayBoardId.Attributes.Add("readonly", "readonly");
                ddlMondayBoardId.Visible = false;
                cbShowApprove.Checked = board.ShowApprove;

                board.LoadAttributes();
                avcAttributesEdit.Visible = false;
                var editableAttributes = board.Attributes.Where(a => a.Value.IsAuthorized(Authorization.EDIT, this.CurrentPerson)).Select(a => a.Key).ToList();
                if (editableAttributes.Any())
                {
                    avcAttributesEdit.Visible = true;
                    avcAttributes.ExcludedAttributes = board.Attributes.Where(a => !editableAttributes.Contains(a.Key)).Select(a => a.Value).ToArray();
                    avcAttributesEdit.AddEditControls(board);
                }

                ddlMondayStatusCompleteValue.ClearSelection();
                ddlMondayStatusClosedValue.ClearSelection();
                ddlMondayStatusApprovedValue.ClearSelection();

                BindMondayColumns(board.MondayBoardId);
                BindMondayColumnOptions(board.MondayBoardId, board.MondayStatusColumnId);
                BindEditDisplayColumnsGrid();
            }
            else
            {
                dtbMondayBoardName.Text = "";
                BindMondayBoards(0);
            }
        }

        private void BindEditDisplayColumnsGrid()
        {
            var bccColumns = new BccMondayBoardDisplayColumnService(new RockContext()).Queryable()
                .Where(c => c.BccMondayBoardId == BccMondayBoardId.Value).ToList();

            var board = GetBoard();
            var mondayBoardId = board != null ? board.MondayBoardId : long.Parse(ddlMondayBoardId.SelectedValue);
            var inColumnsButNotGrid = GetGridColumns(mondayBoardId);

            gEditDisplayColumns.Actions.ShowAdd = true;
            if (inColumnsButNotGrid.Count == 0)
            {
                gEditDisplayColumns.Actions.ShowAdd = false;
            }
            gEditDisplayColumns.DataSource = DisplayColumns ?? bccColumns;
            gEditDisplayColumns.DataBind();
        }

        private void gEditdisplayDisplayColumns_GridRebind(object sender, GridRebindEventArgs e)
        {
            BindEditDisplayColumnsGrid();
        }

        /// <summary>
        /// Retrieves a BccMondayBoard from an Id
        /// </summary>
        /// <returns>BccMondayBoard entity with the specified page parameter Id</returns>
        private BccMondayBoard GetBoard()
        {
            using (var context = new RockContext())
            {
                var service = new BccMondayBoardService(context);
                var id = BccMondayBoardId.Value;

                return service
                    .Queryable()
                    .AsNoTracking()
                    .FirstOrDefault(b => b.Id == id);
            }
        }

        private List<BccMondayBoardDisplayColumn> GetDisplayColumns()
        {
            if (ViewState["DisplayColumns"] != null && !ViewState["DisplayColumns"].Equals("null"))
            {
                var json = (string)ViewState["DisplayColumns"];
                return JsonConvert.DeserializeObject<List<BccMondayBoardDisplayColumn>>(json);
            }
            else
            {
                using (var context = new RockContext())
                {
                    var service = new BccMondayBoardDisplayColumnService(context);

                    return service.Queryable()
                        .Where(c => c.BccMondayBoardId == BccMondayBoardId.Value)
                        .ToList();
                }
            }
        }

        private void BindMondayBoards(int boardId)
        {
            using (var context = new RockContext())
            {
                var api = new MondayApi();
                var result = api.GetBoards();

                if (result != null)
                {
                    List<Board> boards = result;

                    var boardService = new BccMondayBoardService(context);
                    var bccBoardList = boardService.Queryable().ToList();
                    List<Board> temp = boards;
                    temp.RemoveAll(el => bccBoardList
                        .Exists(b => b.MondayBoardId == el.Id
                        && b.Id != boardId)
                        || el.Name.Equals("monday Doc")
                        || el.Name.Equals("Duplicate of monday Doc"));

                    ddlMondayBoardId.DataSource = temp;
                    ddlMondayBoardId.DataBind();
                }
            }
        }

        private void BindMondayColumns(long boardId)
        {
            var board = GetBoard();

            using (var context = new RockContext())
            {
                var api = new MondayApi();
                var result = api.GetBoard(boardId);

                if (result != null)
                {
                    IBoard apiBoard = result;

                    ddlEmailMatchColumnId.DataSource = apiBoard.Columns;
                    ddlEmailMatchColumnId.DataBind();

                    ddlMondayStatusColumnId.DataSource = apiBoard.Columns;
                    ddlMondayStatusColumnId.DataBind();

                    // if board is from db, then set the dropdowns accordingly
                    if (board != null)
                    {
                        var emailValue = ddlEmailMatchColumnId.Items.FindByValue(board.EmailMatchColumnId);
                        var statusValue = ddlEmailMatchColumnId.Items.FindByValue(board.MondayStatusColumnId);

                        ddlEmailMatchColumnId.SelectedIndex = ddlEmailMatchColumnId.Items.IndexOf(emailValue);
                        ddlMondayStatusColumnId.SelectedIndex = ddlMondayStatusColumnId.Items.IndexOf(statusValue);
                    }
                }
                else
                {
                    // TODO: display error
                }
            }
        }

        private void BindMondayColumnOptions(long boardId, string statusColumnId)
        {
            var api = new MondayApi();
            var result = api.GetBoard(boardId);

            if (result != null)
            {
                var apiBoard = result;
                var columns = apiBoard.Columns.ConvertAll(c => (Column)c);
                var statusColumn = (Column)apiBoard.GetColumn(statusColumnId);

                if (statusColumn != null)
                {
                    var options = statusColumn.GetLabels();
                    ddlMondayStatusCompleteValue.DataSource = options;
                    ddlMondayStatusClosedValue.DataSource = options;
                    ddlMondayStatusApprovedValue.DataSource = options;

                    ddlMondayStatusCompleteValue.DataBind();
                    ddlMondayStatusClosedValue.DataBind();
                    ddlMondayStatusApprovedValue.DataBind();

                    var board = GetBoard();
                    if (board != null)
                    {
                        var mondayHasApprovedValue = options.Contains(board.MondayStatusApprovedValue);
                        var mondayHasClosedValue = options.Contains(board.MondayStatusClosedValue);
                        var mondayHasCompleteValue = options.Contains(board.MondayStatusCompleteValue);

                        // set values if board is coming from db
                        if (mondayHasClosedValue && mondayHasCompleteValue)
                        {
                            ddlMondayStatusCompleteValue.SelectedValue = board.MondayStatusCompleteValue;
                            ddlMondayStatusClosedValue.SelectedValue = board.MondayStatusClosedValue;
                            if (board.MondayStatusApprovedValue.IsNotNullOrWhiteSpace() && mondayHasApprovedValue)
                            {
                                ddlMondayStatusApprovedValue.SelectedValue = board.MondayStatusApprovedValue;
                            }
                        }
                        else
                        {
                            ShowError("The status options for this Board have been changed in Monday.com. Please update all status values.");
                        }
                    }
                }
                else
                {
                    ddlMondayStatusCompleteValue.DataSource = new Dictionary<string, string>();
                    ddlMondayStatusClosedValue.DataSource = new Dictionary<string, string>();
                    ddlMondayStatusApprovedValue.DataSource = new Dictionary<string, string>();

                    // TODO: display error
                }

                ddlMondayStatusCompleteValue.DataBind();
                ddlMondayStatusClosedValue.DataBind();
                ddlMondayStatusApprovedValue.DataBind();
                ddlMondayStatusApprovedValue.Items.Insert(0, new ListItem(string.Empty, string.Empty));
            }
            else
            {
                // TODO: display error
            }
        }

        private List<IColumn> GetGridColumns(long mondayBoardId)
        {
            var api = new MondayApi();
            var result = api.GetBoard(mondayBoardId);

            if (result != null)
            {
                var columns = result.Columns;
                var boardColumnsList = new List<Column>();

                if (DisplayColumns != null)
                {
                    foreach (var el in DisplayColumns)
                    {
                        boardColumnsList.Add(new Column
                        {
                            Id = el.MondayColumnId,
                            Title = el.MondayColumnTitle,
                            Type = el.MondayColumnType
                        });
                    }
                }

                return columns.Except(boardColumnsList).ToList();
            }

            return new List<IColumn>();
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

        #endregion
    }
}
