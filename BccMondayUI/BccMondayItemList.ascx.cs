using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Model;
using Rock.Attribute;
using Rock.Data;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using Rock.Security;

using com.baysideonline.BccMonday.Models;
using com.baysideonline.BccMonday.Utilities.Api;
using Newtonsoft.Json;
using com.baysideonline.BccMonday.Utilities.Api.Schema;

namespace RockWeb.Plugins.com_baysideonline.BccMondayUI
{
    [DisplayName("BccMondayItemList")]
    [Category("BccMonday")]
    [Description("Displays a list of items from a monday.com board based on a set of filters")]
    [LinkedPage("Detail Page")]
    [BooleanField("BccMonday Debug",
        Name = "Enable Debug Mode",
        Key = PageParameterKey.BccMondayDebug,
        Description = "Enabling this will generate exceptions.This is very useful for troubleshooting.",
        IsRequired = true,
        DefaultBooleanValue = false
        )]
    public partial class BccMondayItemList : RockBlock
    {
        #region PageParameterKeys

        private static class PageParameterKey
        {
            public const string BccMondayBoardId = "BccMondayBoardId";
            public const string BccMondayDebug = "BccMondayDebug";
        }

        #endregion PageParameterKeys

        #region Preference Keys

        private static class PreferenceKeys
        {
            public const string Requestor = "requestor";
            public const string ShowAllItems = "show-all-items";
            public const string SelectedBoard = "selected-board";
        }
        #endregion

        #region Fields

        //Holds whether or not the current person can add, edit, and delete.
        private bool _canAddEditDelete = false;
        private bool _canFilterEmail = false;
        private bool _canShowAllItems = false;

        #endregion

        #region Properties

        // loaded in OnLoad(), stores the BccMondayBoardId page param
        public int BccMondayBoardId { get; set; }

        // loaded in OnLoad(), stores the BccMondayBoard object from db
        public BccMondayBoard Board { get; set; }


        public List<Item> Items { get; set; }
        #endregion

        #region Base Control Methods

        // called on initial load of the block
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _canAddEditDelete = _canFilterEmail = _canShowAllItems = IsUserAuthorized(Authorization.EDIT);

            gMondayList.Actions.ShowAdd = false;
            gMondayList.Actions.ShowMergeTemplate = false;
            gMondayList.Actions.ShowExcelExport = false;

            gfMondayList.ApplyFilterClick += gfMondayList_ApplyFilterClick;
            gfMondayList.DisplayFilterValue += gfMondayList_DisplayFilterValue;


            ppMondayEmail.Enabled = _canFilterEmail;

            // DataKeyNames will be mapped to MondayItemId.
            // This is for the Detail class to find the correct Monday Item by it's MondayItemId.
            gMondayList.DataKeyNames = new[] { "Id" };
            gMondayList.GridRebind += gMondayList_GridRebind;

            // this event gets fired after block settings are updated.
            // it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger(upnlContent);

            cbShowAll.Visible = _canShowAllItems;
        }

        // called on every load (including post back)
        protected override void OnLoad(EventArgs e)
        {
            // will not display block if board or Id is not found
            if (LoadBoard())
            {
                lTitle.Text = Board.MondayBoardName;

                if (!Page.IsPostBack)
                {
                    SetFilter();
                    BindGrid();
                }
            }
            else
            {
                ShowError("Unable to find a BccMondayBoard with the specified Id.");
            }

            base.OnLoad(e);
        }

        /*
        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            if (ViewState["MondayItems"] != null && !ViewState["MondayItems"].Equals("-1"))
            {
                string mondayItems = (string)ViewState["MondayItems"];
                Items = JsonConvert.DeserializeObject<List<Item>>(mondayItems);
            }
        }*/

        /*
        protected override object SaveViewState()
        {
            string mondayItems = JsonConvert.SerializeObject(Items);
            ViewState["MondayItems"] = mondayItems;
            return base.SaveViewState();
        }
        */

        #endregion

        #region Events

        // handles the Block_BlockUpdated event
        protected void Block_BlockUpdated(object sender, EventArgs e)
        {

        }

        // handles the gMondayList_GridRebind event
        private void gMondayList_GridRebind(object sender, EventArgs e)
        {
            BindGrid();
        }

        /// <summary>
        /// Handles when the user clicks on a row in the grid.
        /// </summary>
        /// <remarks>Handles the gMondayList_RowSelected event</remarks>
        protected void gMondayList_RowSelected(object sender, RowEventArgs e)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "MondayItemId", e.RowKeyValue.ToString() }
                ,{ "BccMondayBoardId", Board.MondayBoardId.ToString() }
            };

            NavigateToLinkedPage("DetailPage", queryParams);
        }

        /// <summary>
        /// Handles the gfMondayList_ApplyFilterClick
        /// </summary>
        /// <remarks>Stores the user selected filter options</remarks>
        protected void gfMondayList_ApplyFilterClick(object sender, EventArgs e)
        {
            int personId = ppMondayEmail.PersonId ?? CurrentPerson.Id;
            var preferences = GetBlockPersonPreferences();

            preferences.SetValue(PreferenceKeys.Requestor, _canFilterEmail ? personId.ToString() : string.Empty);
            preferences.SetValue(PreferenceKeys.ShowAllItems, cbShowAll.Checked.ToString());
            preferences.Save();

            if (string.Equals(ddlBoardOption.SelectedValue, "Closed"))
            {
                gMondayList.DataSource = Items;
                gMondayList.DataBind();
                mdDialog.Show();
            }
            else
            {
                preferences.SetValue(PreferenceKeys.SelectedBoard, ddlBoardOption.SelectedValue);
                preferences.Save();
                BindGrid();
            }
        }

        /// <summary>
        /// Handles the gfMondayList_DisplayFilterValue
        /// </summary>
        /// <remarks> Displays the filter options</remarks>
        protected void gfMondayList_DisplayFilterValue(object sender, GridFilter.DisplayFilterValueArgs e)
        {
            switch (e.Key)
            {
                case "Requestor":
                    string requestorName = CurrentPerson.FullName;

                    if (_canFilterEmail)
                    {
                        int? personId = ppMondayEmail.PersonId;
                        if (personId.HasValue)
                        {
                            var requestor = new PersonService(new RockContext())
                                .Get(personId.Value);

                            if (requestor != null)
                                requestorName = requestor.FullName;
                        }
                    }

                    e.Value = requestorName;
                    break;
                case "Selected Board":
                    Console.WriteLine("");
                    break;
                case "Show All Items":
                    if (!_canShowAllItems)
                    {
                        e.Value = string.Empty;
                    }
                    break;
                default:
                    e.Value = string.Empty;
                    break;
            }
        }

        // handles the gfMondayList_ClearFilterClick
        protected void gfMondayList_ClearFilterClick(object sender, EventArgs e)
        {
            gfMondayList.DeleteFilterPreferences();
            BindGrid();
        }

        #endregion

        #region Methods

        // displays an error message instead of the block UI
        public void ShowError(string message)
        {
            pnlLists.Visible = false;
            dAlertError.Visible = true;
            lError.Text = message;
        }

        /// <summary>
        ///  Binds the filter to the specified values
        /// </summary>
        /// <remarks> Applies the selected filter options to the item list </remarks>
        protected void SetFilter()
        {
            var preferences = GetBlockPersonPreferences();
            var requestorId = preferences.GetValue(PreferenceKeys.Requestor).AsIntegerOrNull();

            if (!(_canFilterEmail && requestorId.HasValue))
            {
                requestorId = CurrentPersonId;
                preferences.SetValue(PreferenceKeys.Requestor, requestorId?.ToString());
            }

            if (requestorId.HasValue && requestorId.Value != 0)
            {
                var requestor = new PersonService(new RockContext()).Get(requestorId.Value);
                if (requestor != null)
                {
                    ppMondayEmail.SetValue(requestor);
                }
            }

            var boardPreference = preferences.GetValue(PreferenceKeys.SelectedBoard);
            var showAllPreference = preferences.GetValue(PreferenceKeys.ShowAllItems);

            cbShowAll.Checked = !string.IsNullOrWhiteSpace(showAllPreference) && bool.Parse(showAllPreference);

            ddlBoardOption.SetValue(!string.IsNullOrWhiteSpace(boardPreference) ? boardPreference : "Open");
        }

        /// <summary>
        /// Populates the iem list with the applied filters
        /// </summary>
        /// <remarks> Binds the gMondayList Grid Control. </remarks>
        private void BindGrid()
        {
            using (var context = new RockContext())
            {
                var sortProperty = gMondayList.SortProperty;

                var api = new MondayApi();

                var requestorId = ppMondayEmail.PersonId ?? CurrentPersonId.Value;
                var requestorPerson = new PersonService(context)
                        .Get(requestorId);
                var requestorEmail = requestorPerson.Email;

                var boardOption = ddlBoardOption.SelectedValue;
                var chosenBoard = GetChosenBoard(boardOption);

                if (requestorEmail.IsNotNullOrWhiteSpace() && chosenBoard.HasValue)
                {
                    var result = api.GetItemsByBoard(chosenBoard.Value,
                        Board.EmailMatchColumnId, Board.MondayStatusColumnId);

                    //Check if list is empty/null
                    if (result != null && result.Count > 0)
                    {
                        var items = result;

                        var statusIndex = items[0].ColumnValues
                                        .FindIndex(c => c.ColumnId == Board.MondayStatusColumnId);

                        if (statusIndex == -1)
                        {
                            string statusError = string.Format("This board's (Name: {0} ~ Id: {1}) query has no status column matching {2}",
                                Board.MondayBoardName, Board.MondayBoardId, Board.MondayStatusColumnId);
                            ExceptionLogService.LogException(new Exception(statusError, new Exception("BccMonday")));
                            gMondayList.DataSource = new List<Item>();
                            gMondayList.DataBind();
                            return;
                        }

                        // if the closed board is selected
                        if (boardOption.Equals("Closed"))
                        {
                            items = items.Where(i =>
                                string.Equals(i.ColumnValues[statusIndex].Text, Board.MondayStatusClosedValue) ||
                                string.Equals(i.ColumnValues[statusIndex].Text, Board.MondayStatusCompleteValue)
                            ).ToList();
                        }
                        else
                        {
                            items.RemoveAll(i =>
                                string.Equals(i.ColumnValues[statusIndex].Text, Board.MondayStatusClosedValue) ||
                                string.Equals(i.ColumnValues[statusIndex].Text, Board.MondayStatusCompleteValue)
                            );
                        }

                        //if the admin wants to see all items
                        if (!(_canFilterEmail && cbShowAll.Checked))
                        {
                            items = items.Where(i =>
                            {
                                var requestorEmailValue = i.GetRequestorEmail(Board.EmailMatchColumnId);

                                return string.Equals(requestorEmailValue, requestorEmail, StringComparison.OrdinalIgnoreCase);
                            })
                            .ToList();
                        }

                        items = SortItems(sortProperty, items, statusIndex);

                        Items = items.OfType<Item>().ToList();
                        gMondayList.DataSource = Items;
                        gMondayList.DataBind();
                        return;
                    }
                }
                gMondayList.DataSource = new List<Item>();
                gMondayList.DataBind();

            }
        }

        // loads the BccMondayBoardId page param into the property. Returns false on fail
        private bool LoadBccMondayBoardId()
        {
            string pageParameterStr = PageParameter(PageParameterKey.BccMondayBoardId);

            if (!String.IsNullOrWhiteSpace(pageParameterStr))
            {
                int? bccMondayBoardId = pageParameterStr.AsIntegerOrNull();

                if (bccMondayBoardId.HasValue)
                {
                    BccMondayBoardId = bccMondayBoardId.Value;
                    return true;
                }
            }

            return false;
        }

        // loads the BccMondayBoard from the db into the property. Returns false on fail
        private bool LoadBoard()
        {
            if (LoadBccMondayBoardId())
            {
                using (var context = new RockContext())
                {
                    var service = new BccMondayBoardService(context);

                    // Show if BccMondayBoard exists or closedBoard Id exists
                    BccMondayBoard board = service
                        .Queryable()
                        .Where(b => b.Id == BccMondayBoardId)
                        .FirstOrDefault();

                    if (board != null)
                    {
                        Board = board;
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        /// <summary>
        /// Creates the Status badge for each row in the grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gMondayList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var rowIndex = e.Row.RowIndex;
                var statusIndex = Items[0].ColumnValues.FindIndex(c => c.ColumnId == Board.MondayStatusColumnId);
                var columnValue = Items[rowIndex].ColumnValues[statusIndex] as StatusColumnValue;

                var root = new LiteralControl();
                var stringWriter = new StringWriter();
                var writer = new HtmlTextWriter(stringWriter);

                //TODO(Noah): The color will always be lightgrey if the Column is null.
                //  Fixed in new updates. It parses the "additional_info" property
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "text-center");
                writer.AddStyleAttribute("background-color", columnValue?.LabelStyle.Color ?? string.Empty);
                writer.AddStyleAttribute("text-overflow", "ellipsis");
                writer.AddStyleAttribute("white-space", "nowrap");
                writer.AddStyleAttribute("overflow", "hidden");
                writer.AddStyleAttribute("padding", "0 8px");
                writer.AddStyleAttribute("max-width", "250px");
                writer.AddStyleAttribute("color", "#ffffff");
                writer.AddStyleAttribute("border-radius", "10px");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write(columnValue?.Text);
                writer.RenderEndTag();
                writer.RenderEndTag();

                var html = writer.InnerWriter.ToString();
                root.Text = html;

                e.Row.Cells[1].Controls.Add(root);
            }
        }

        /// <summary>
        /// Sorts the item list depending on the SortProperty
        /// </summary>
        /// <remarks> The default is "Created At" Descending, then by Name Ascending. </remarks>
        public List<Item> SortItems(SortProperty sortProperty, List<Item> items, int statusIndex)
        {
            if (sortProperty == null)
            {
                return items.OrderByDescending(i => i.CreatedAt).ThenBy(i => i.Name).ToList();
            }

            switch (sortProperty.Property)
            {
                case "Name" when sortProperty.DirectionString.Equals("ASC"):
                    return items.OrderBy(i => i.Name).ToList();
                case "Name":
                    return items.OrderByDescending(i => i.Name).ToList();
                case "Created At" when sortProperty.DirectionString.Equals("ASC"):
                    return items.OrderBy(i => i.CreatedAt).ToList();
                case "Created At":
                    return items.OrderByDescending(i => i.CreatedAt).ToList();
                case "Status" when sortProperty.DirectionString.Equals("ASC"):
                    return items.OrderBy(i => i.ColumnValues[statusIndex].Text).ToList();
                case "Status":
                    return items.OrderByDescending(i => i.ColumnValues[statusIndex].Text).ToList();
                default:
                    return items;
            }
        }

        public long? GetChosenBoard(string selectedBoard)
        {
            if (selectedBoard.Equals("Open"))
            {
                return Board.MondayBoardId;
            }

            Board.LoadAttributes();
            var closedBoardId = Board.GetAttributeValue("MondayClosedBoardId");
            return closedBoardId.IsNotNullOrWhiteSpace() ? long.Parse(closedBoardId) : Board.MondayBoardId;
        }

        protected void mdDialog_SaveClick(object sender, EventArgs e)
        {
            mdDialog.Hide();
            var preferences = GetBlockPersonPreferences();
            preferences.SetValue(PreferenceKeys.SelectedBoard, ddlBoardOption.SelectedValue);
            preferences.Save();
            SetFilter();
            BindGrid();
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
