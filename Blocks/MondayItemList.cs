using com.baysideonline.BccMonday.Models;
using com.baysideonline.BccMonday.Utilities.Api;
using com.baysideonline.BccMonday.Utilities.Api.Schema;
using com.baysideonline.BccMonday.ViewModels.MondayItemDetail;
using Newtonsoft.Json;
using Rock;
using Rock.Attribute;
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
    /// Displays a list of Monday.com items.
    /// </summary>
    /// <seealso cref="Rock.Blocks.RockBlockType" />

    [DisplayName("Monday Item List")]
    [Category("com_baysideonline > BccMonday")]
    [Description("Monday.com Item List block")]
    [SupportedSiteTypes(Rock.Model.SiteType.Web)]

    #region Block Attributes
    [BooleanField("BccMonday Debug",
        Name = "Enable Debug Mode",
        Key = AttributeKey.BccMondayDebug,
        Description = "Enabling this will generate exceptions.This is very useful for troubleshooting.",
        IsRequired = true,
        DefaultBooleanValue = false
        )]
    [LavaField("Lava Template",
        Name = "Lava Template",
        Key = AttributeKey.LavaTemplate,
        Description = "The Lava template used to render the list of Monday Items.",
        IsRequired = false
        )]
    #endregion

    [Rock.SystemGuid.EntityTypeGuid("e46605f9-1fc2-431b-91fe-0d81ea56e2cd")]
    [Rock.SystemGuid.BlockTypeGuid("f7afda16-2df6-4137-9f16-1422f366781f")]
    public class MondayItemList : RockBlockType
    {
        public override string ObsidianFileUrl => $"/Plugins/com_baysideonline/BccMonday/Blocks/MondayItemList.obs";

        #region Keys
        private static class AttributeKey
        {
            public const string BccMondayDebug = "BccMondayDebug";
            public const string LavaTemplate = "LavaTemplate";
        }

        private static class PageParameterKey
        {
            public const string BccMondayBoardId = "BccMondayBoardId";
            public const string MondayItemId = "MondayItemId";
            public const string SelectedBoard = "SelectedBoard";
        }

        #endregion Keys

        #region Methods

        /// <inheritdoc/>
        public override object GetObsidianBlockInitialization()
        {
            var requestorEmail = RequestContext.CurrentPerson.Email;

            //Default to the open board
            var boardOption = PageParameter(PageParameterKey.SelectedBoard ?? "Open").Equals("Closed", StringComparison.OrdinalIgnoreCase);
            var bccMondayBoardId = PageParameter(PageParameterKey.BccMondayBoardId).AsIntegerOrNull();
            var bccMondayBoard = GetBccMondayBoard(bccMondayBoardId);

            var chosenBoard = GetChosenBoard(bccMondayBoard, boardOption);

            if (chosenBoard == null)
            {
                return new
                {
                    Content = ""
                };
            }

            var api = new MondayApi();

            List<ItemsPageByColumnValuesQuery> columnValuesQuery = new List<ItemsPageByColumnValuesQuery>
                {
                    new ItemsPageByColumnValuesQuery
                    {
                        ColumnId = bccMondayBoard.EmailMatchColumnId,
                        ColumnValues = new List<string> { requestorEmail }
                    }
                };
            var items = api.GetItemsByBoardAndColumnValues(chosenBoard.Value, columnValuesQuery);
            List<MondayItemBag> itemsBag = new List<MondayItemBag>();
            foreach (var item in items)
            {
                itemsBag.Add(CreateBag(item));
            }
            //var result = JsonConvert.SerializeObject(items);

            var lavaTemplate = GetAttributeValue(AttributeKey.LavaTemplate);
            var mergeFields = new Dictionary<string, object>
            {
                { "items", JsonConvert.SerializeObject(itemsBag) }
            };

            var renderedHtml = lavaTemplate.ResolveMergeFields(mergeFields);

            return new
            {
                Content = renderedHtml
            };
        }

        public BccMondayBoard GetBccMondayBoard(int? Id)
        {
            if (!Id.HasValue)
            {
                return null;
            }

            var bccMondayBoardService = new BccMondayBoardService(new RockContext());
            return bccMondayBoardService.Get(Id.Value) ?? null;
        }

        public long? GetChosenBoard(BccMondayBoard board, bool closed = false)
        {
            if (board == null)
            {
                return null;
            }

            if (closed)
            {
                return board.MondayBoardId;
            }

            board.LoadAttributes();
            var closedBoardId = board.GetAttributeValue("MondayClosedBoardId");
            return closedBoardId.IsNotNullOrWhiteSpace() ? long.Parse(closedBoardId) : board.MondayBoardId;
        }

        public MondayItemBag CreateBag(Item item)
        {
            var itemBag = new MondayItemBag
            {
                Id = item.Id.ToString(),
                Name = item.Name,
                CreatedAt = item.CreatedAt.ToString(),
                ColumnValues = item.ColumnValues.Select(c =>
                {
                    MondayColumnValueBag mondayColumnValueBag = new MondayColumnValueBag
                    {
                        Id = c.ColumnId,
                        Text = c.Text,
                        Type = c.Type,
                        Value = c.Value,
                        Column = new MondayColumnBag
                        {
                            Id = c.Column.Id,
                            Title = c.Column.Title,
                        }
                    };

                    // Check the type and assign properties accordingly
                    if (c is StatusColumnValue statusColumnValue)
                    {
                        //mondayColumnValueBag.Index = statusColumnValue.Index;
                        //mondayColumnValueBag.StatusLabel = statusColumnValue.StatusLabel;
                        //mondayColumnValueBag.IsDone = statusColumnValue.IsDone;
                        mondayColumnValueBag.LabelStyle = new LabelStyle
                        {
                            Color = statusColumnValue.LabelStyle.Color,
                            Border = statusColumnValue.LabelStyle.Border
                        };
                    }
                    
                    return mondayColumnValueBag;
                }).ToList()
            };

            var foo = Newtonsoft.Json.JsonConvert.SerializeObject(itemBag.ColumnValues);

            return itemBag;
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
