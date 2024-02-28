using Rock.Blocks;
using Rock.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Blocks
{
    /// <summary>
    /// Displays a list of BccMonday Boards.
    /// </summary>
    /// <seealso cref="Rock.Blocks.RockBlockType" />

    [DisplayName("Monday Board List")]
    [Category("com_baysideonline > BccMonday")]
    [Description("Monday.com Board List block")]
    [SupportedSiteTypes(Rock.Model.SiteType.Web)]

    #region Block Attributes


    #endregion

    [Rock.SystemGuid.EntityTypeGuid("dbb031cd-f635-459d-8918-9f363cfdc9e2")]
    [Rock.SystemGuid.BlockTypeGuid("321c1cef-4a25-4092-98bf-df668c38c134")]
    public class MondayBoardList : RockBlockType
    {
        public override string ObsidianFileUrl => $"/Plugins/com_baysideonline/BccMonday/Blocks/MondayBoardList.obs";

        #region Keys

        private static class PageParameterKey
        {
            public const string BccMondayBoardId = "BccMondayBoardId";
        }

        #endregion Keys

        #region Methods

        /// <inheritdoc/>
        public override object GetObsidianBlockInitialization()
        {
            using (var rockContext = new RockContext())
            {
                var box = 1;// new MondayBoardListBox();

                return box;
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
