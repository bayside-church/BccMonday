using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Rock.Data;
using Rock.Web.UI.Controls;
using Rock.Security;
using Rock.Attribute;
using Rock.Web.UI;
using com.baysideonline.BccMonday.Models;

namespace RockWeb.Plugins.com_baysideonline.BccMondayUI
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName("BccMondayBoardList")]
    [Category("BccMonday")]
    [Description("List of monday.com boards that are integrated with Rock.")]
    [LinkedPage("DetailPage")]
    public partial class BccMondayBoardList : RockBlock
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            var _canAddEditDelete = IsUserAuthorized(Authorization.EDIT);

            gBoardList.GridRebind += gBoardList_GridRebind;
            gBoardList.Actions.ShowAdd = _canAddEditDelete;
            gBoardList.Actions.AddClick += gBoardList_Add;
            gBoardList.Actions.ShowMergeTemplate = false;
            gBoardList.Actions.ShowExcelExport = false;

            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger(upnlContent);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                BindGrid();
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
        /// Handles the GridRebind event of the gPledges control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void gBoardList_GridRebind(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void gBoardList_Add(object sender, EventArgs e)
        {
            int newBoardId = 0;

            var queryParams = new Dictionary<string, string>
            {
                {  "BccMondayBoardId", newBoardId.ToString() }
            };

            NavigateToLinkedPage("DetailPage", queryParams);
        }

        /// <summary>
        /// Navigates to the assigned detail page for the BccMondayBoard
        /// </summary>
        /// <param name="sender">the source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data</param>
        protected void gBoardList_RowSelected(object sender, RowEventArgs e)
        {
            var queryParams = new Dictionary<string, string>
            {
                {  "BccMondayBoardId", e.RowKeyValue.ToString() }
            };

            NavigateToLinkedPage("DetailPage", queryParams);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Binds the grid.
        /// </summary>
        private void BindGrid()
        {
            var context = new RockContext();
            var boardService = new BccMondayBoardService(context);

            var boards = boardService
                .Queryable()
                .ToList();

            // TODO: filter

            // TODO: sort

            gBoardList.DataSource = boards;
            gBoardList.DataBind();
        }

        #endregion
    }
}