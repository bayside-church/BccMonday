
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
    /// Displays the details of a particular group.
    /// </summary>
    /// <seealso cref="Rock.Blocks.RockObsidianBlockType" />

    [DisplayName("Monday Board Detail")]
    [Category("com_baysideonline > BccMonday")]
    [Description("Monday.com Board Detail block")]
    //[SupportedSiteTypes(Rock.Model.SiteType.Web)]

    #region Block Attributes


    #endregion

    [Rock.SystemGuid.EntityTypeGuid("2fbadd22-d7c6-4ca9-918c-42016d9058c5")]
    [Rock.SystemGuid.BlockTypeGuid("f5002d5b-ed2c-4a25-af74-66ac25af2c73")]
    public class MondayBoardDetail : RockObsidianBlockType
    {
        public override string BlockFileUrl => $"/Plugins/com_baysideonline/BccMonday/Blocks/MondayBoardDetail.obs";

        #region Keys

        #endregion Keys

        #region Methods

        /// <inheritdoc/>
        public override object GetObsidianBlockInitialization()
        {
            using (var rockContext = new RockContext())
            {
                var box = 1;// new MondayBoardDetailBox();

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
