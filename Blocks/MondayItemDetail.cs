using com.baysideonline.BccMonday.Models;
using com.baysideonline.BccMonday.Utilities.Api;
using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using com.baysideonline.BccMonday.Utilities.Api.Schema;
using com.baysideonline.BccMonday.ViewModels.MondayItemDetail;
using Rock.Blocks;
using Rock.Data;
using Rock.ViewModels.Blocks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Blocks
{
    /// <summary>
    /// Displays the details of a particular group.
    /// </summary>
    /// <seealso cref="Rock.Blocks.RockObsidianBlockType" />

    [DisplayName("Monday Item Detail")]
    [Category("com_baysideonline > BccMonday")]
    [Description("Monday.com Item Detail block")]
    //[SupportedSiteTypes(Rock.Model.SiteType.Web)]

    #region Block Attributes


    #endregion

    [Rock.SystemGuid.EntityTypeGuid("65ea330e-dc04-4bcc-87b5-829f0d7880bd")]
    [Rock.SystemGuid.BlockTypeGuid("457a0eec-ae0e-40e5-8391-f00d9da5594d")]
    public class MondayItemDetail : RockObsidianBlockType
    {
        public override string BlockFileUrl => $"/Plugins/com_baysideonline/BccMonday/Blocks/MondayItemDetail.obs";

        #region Keys

        private static class PageParameterKey
        {
            public const string BccMondayBoardId = "BccMondayBoardId";
            public const string MondayItemId = "MondayItemId";
            public const string BccMondayDebug = "BccMondayDebug";
        }

        #endregion Keys

        #region Methods

        /// <inheritdoc/>
        public override object GetObsidianBlockInitialization()
        {
            using (var rockContext = new RockContext())
            {
                //var box = new MondayItemDetailBag();
                var box = new DetailBlockBox<MondayItemDetailBag, MondayItemDetailOptionsBag>();

                SetBoxInitialEntityState(box, rockContext);

                box.Options = GetBoxOptions(box.IsEditable, rockContext);

                return box;
            }
        }

        /*
         * UI options that can be sent to the client:
         * - Show Status Button (Approved,Closed, Both)
         * - Put the color into the Updates
         * - Bind the Item's column values into a for loop for placeholders
         * - When the user creates an update or changes the status of the item,
         *      then should they refetch all of it or just modify part of the item client-side on success?
         */

        private MondayItemDetailOptionsBag GetBoxOptions( bool isEditable, RockContext context)
        {
            var options = new MondayItemDetailOptionsBag
            {
                ShowStatusButton = true
            };
            return options;
        }

        private void SetBoxInitialEntityState(DetailBlockBox<MondayItemDetailBag, MondayItemDetailOptionsBag> box, RockContext context)
        {
            var item = GetItem();
            item.ColumnValues = GetDisplayColumnValues(item);

            var updates = item.Updates;
            var columnValues = item.ColumnValues;
            var itemName = item.Name;
            var createdAt = item.CreatedAt;


            box.Entity = new MondayItemDetailBag {
                Item = item,
                Name = item.Name
            };
        }

        /// <summary>
        /// Gets the Display Column Values from the database
        /// </summary>
        /// <param name="item">The Monday.com Item</param>
        /// <returns>A list of Column Values that are allowed to be displayed.</returns>
        public List<AbstractColumnValue> GetDisplayColumnValues(Item item)
        {
            using (var context = new RockContext())
            {
                var columnService = new BccMondayBoardDisplayColumnService(context);
                var displayColumns = columnService
                    .Queryable()
                    .Where(c => c.BccMondayBoard.MondayBoardId == item.BoardId)
                    .ToList();

                var valuesToDisplay = item.ColumnValues
                    .Where(c => displayColumns.Find(d => d.MondayColumnId.Equals(c.ColumnId)) != null)
                    .ToList();

                return valuesToDisplay;
            }
        }

        /// <summary>
        /// Gets the Item instance
        /// </summary>
        /// <remarks>Loads Item from API</remarks>
        /// <returns>The item specified by the page parameter</returns>
        public Item GetItem()
        {
            var mondayItemId = long.Parse(PageParameter(PageParameterKey.MondayItemId));

            var api = new MondayApi();
            var item = api.GetItem(mondayItemId);
            item.Updates.RemoveAll(u => u.Creator.CreatorId.Equals("-4"));
            return (Item)item;
        }

        public BccMondayBoard GetBoard()
        {
            using (var context = new RockContext())
            {
                var boardService = new BccMondayBoardService(context);
                var item = GetItem();
                var mondayBoardId = item.Board.Id;

                var board = boardService
                    .Queryable()
                    .FirstOrDefault(b => b.MondayBoardId == mondayBoardId);
                return board;
            }
        }

        #endregion

        #region Block Actions

        ///// <summary>
        ///// Runs contained in the box.
        ///// </summary>
        ///// <param name="RunLavaBag">Runs the lava that is contained in the bag</param>
        ///// <returns>A string with the processed lava</returns>
        //[BlockAction]
        /*public BlockActionResult RunLava(RunLavaBag runLavaBag)
        {
            try
            {
                var results = runLavaBag.Lava.ResolveMergeFields(null);
                return ActionOk(results);
            }
            catch (Exception e)
            {
                return ActionBadRequest(e.Message);
            }
        }
        */

        #endregion
    }
}
