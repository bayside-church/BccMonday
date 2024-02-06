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

    [DisplayName("Monday Item List")]
    [Category("com_baysideonline > BccMonday")]
    [Description("Monday.com Item List block")]
    [SupportedSiteTypes(Rock.Model.SiteType.Web)]

    #region Block Attributes


    #endregion

    [Rock.SystemGuid.EntityTypeGuid("e46605f9-1fc2-431b-91fe-0d81ea56e2cd")]
    [Rock.SystemGuid.BlockTypeGuid("f7afda16-2df6-4137-9f16-1422f366781f")]
    public class MondayItemList : RockObsidianBlockType
    {
        public override string BlockFileUrl => $"/Plugins/com_baysideonline/BccMonday/Blocks/MondayItemList.obs";

        #region Keys

        #endregion Keys

        #region Methods

        /// <inheritdoc/>
        public override object GetObsidianBlockInitialization()
        {
            using (var rockContext = new RockContext())
            {
                var box = 1;// new MondayItemListBox();

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
