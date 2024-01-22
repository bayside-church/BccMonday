using com.baysideonline.BccMonday.Models;
using com.baysideonline.BccMonday.Utilities.Api;
using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using com.baysideonline.BccMonday.Utilities.Api.Schema;
using com.baysideonline.BccMonday.ViewModels.MondayItemDetail;
using Rock;
using Rock.Blocks;
using Rock.Data;
using Rock.Model;
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
        public override string BlockFileUrl => $"/Plugins/com_baysideonline/BccMonday/Blocks/mondayItemDetail.obs";

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
                var bag = SetInitialState();

                return bag;
            }
        }

        private MondayItemDetailOptionsBag GetBoxOptions( bool isEditable, RockContext context)
        {
            var options = new MondayItemDetailOptionsBag
            {
                ShowStatusButton = true
            };
            return options;
        }

        private MondayItemDetailBag SetInitialState()
        {
            var bag = new MondayItemDetailBag();
            var item = GetItem();
            item.ColumnValues = GetDisplayColumnValues(item);

            var itemBag = new MondayItemBag
            {
                Id = item.Id.ToString(),
                Name = item.Name,
                CreatedAt = item.CreatedAt.ToString(),
                Board = new MondayBoardBag
                {
                    Id = item.Board.Id.ToString(),
                    Name = item.Board.Name,
                },
                Updates = item.Updates.Select(u =>
                    new MondayUpdateBag
                    {
                        Id = u.Id.ToString(),
                        CreatedAt = u.CreatedAt.ToString(),
                        CreatorName = u.CreatorName,
                        Files = u.Assets.Select(a =>
                            new MondayAssetBag
                            {
                                Id = a.Id.ToString(),
                                Name = a.Name,
                                PublicUrl = a.PublicUrl,
                            }).ToList(),
                        TextBody = u.TextBody,
                        Replies = u.Replies.Select(r =>
                            new MondayUpdateBag
                            {
                                Id = r.Id.ToString(),
                                TextBody = r.TextBody,
                                CreatedAt = r.CreatedAt.ToString(),
                                CreatorName = r.CreatorName,
                            }).ToList()
                    }).ToList(),
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
                    if (c is FileColumnValue fileColumnValue)
                    {
                        mondayColumnValueBag.Files = fileColumnValue.Files.Select(f =>
                            new FileBag
                            {
                                AssetId = f.AssetId.ToString(),
                                Asset = new MondayAssetBag
                                {
                                    Id = f.Asset.Id.ToString(),
                                    Name = f.Asset.Name,
                                    PublicUrl = f.Asset.PublicUrl
                                }
                            }
                        ).ToList();
                    }
                    else if (c is StatusColumnValue statusColumnValue)
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
                    else if (c is BoardRelationColumnValue boardRelationColumnValue)
                    {
                        mondayColumnValueBag.DisplayValue = boardRelationColumnValue.DisplayValue;
                        //mondayColumnValueBag.LinkedItems = boardRelationColumnValue.LinkedItems;
                        mondayColumnValueBag.LinkedItemIds = boardRelationColumnValue.LinkedItemIds;
                    }

                    return mondayColumnValueBag;
                }).ToList()
            };

            var board = GetBoard(item.BoardId.Value);
            var statusIndex = item.ColumnValues.FindIndex(c => c.ColumnId == board.MondayStatusColumnId);

            var status = "";
            if (item.ColumnValues.Count > 0)
            {
                status = item.ColumnValues[statusIndex].Text;
            }



            bag = new MondayItemDetailBag {
                Item = itemBag,
                Status = status,
                StatusIndex = statusIndex,
                ShowApprove = ShowStatusButton(board.MondayStatusApprovedValue, item, board) && board.ShowApprove,
                ShowClose = ShowStatusButton(board.MondayStatusClosedValue, item, board),
            };
            return bag;
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

        public BccMondayBoard GetBoard(long mondayBoardId)
        {
            using (var context = new RockContext())
            {
                var boardService = new BccMondayBoardService(context);
                //var item = GetItem();
                //mondayBoardId = item.Board.Id;

                var board = boardService
                    .Queryable()
                    .FirstOrDefault(b => b.MondayBoardId == mondayBoardId);
                return board;
            }
        }

        protected bool ShowStatusButton(string statusValue, Item item, BccMondayBoard board)
        {
            if (statusValue.IsNullOrWhiteSpace()) return false;

            var statusIndex = item.ColumnValues.FindIndex(c => c.ColumnId == board.MondayStatusColumnId);
            if (statusIndex != -1)
            {
                var status = item.ColumnValues[statusIndex].Text;
                return !status.Equals(statusValue);
            }
            return false;

        }

        #endregion

        #region Block Actions

        /// <summary>
        /// Saves an update to the Monday.com Item
        /// </summary>
        /// <returns></returns>
        [BlockAction]
        public BlockActionResult SaveUpdate(MondayItemDetailArgs args, string text, Guid? fileUploaded = null)
        {
            try
            {
                if (text.IsNullOrWhiteSpace())
                {
                    return ActionBadRequest("Update cannot be empty");
                }

                var api = new MondayApi();
                var currentPerson = GetCurrentPerson();
                var itemId = args.MondayItemId;
                var updateBodyWithUser = text.Insert(0, string.Format("\n[[{0}]] ", currentPerson.Email));

                var sanitizedUpdateBody = updateBodyWithUser.Replace(@"""", @"\""");
                sanitizedUpdateBody = sanitizedUpdateBody.Replace(@"\", @"\\");

                var newUpdate = api.AddUpdateToItem(itemId, sanitizedUpdateBody);
                var updatePosted = newUpdate != null;

                if (updatePosted)
                {
                    var updateBag = new MondayUpdateBag
                    {
                        Id = newUpdate.Id.ToString(),
                        TextBody = newUpdate.TextBody.ToString(),
                        CreatorName = newUpdate.CreatorName,
                        CreatedAt = newUpdate.CreatedAt.ToString()
                    };

                    if (fileUploaded.HasValue)
                    {
                        var binaryFileService = new BinaryFileService(new RockContext());
                        var binaryFile = binaryFileService.Get(fileUploaded.Value);
                        var fileApi = new MondayApi(MondayApiType.File);
                        var postedFile = fileApi.AddFileToUpdate(newUpdate.Id, binaryFile);
                        if (postedFile != null)
                        {
                            binaryFileService.Delete(binaryFile);
                            var assetBag = new MondayAssetBag
                            {
                                Id = postedFile.Id.ToString(),
                                Name = postedFile.Name,
                                PublicUrl = postedFile.PublicUrl,
                            };
                            updateBag.Files.Add(assetBag);
                            return ActionOk(updateBag);
                        } else
                        {
                            return ActionBadRequest("Could not upload file");
                        }
                    }
                    return ActionOk(updateBag);
                }

                return ActionBadRequest("Unable to post update");

            }
            catch (Exception ex)
            {
                return ActionBadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Saves a reply to the Monday.com Item
        /// </summary>
        /// <returns></returns>
        [BlockAction]
        public BlockActionResult SaveReply(MondayItemDetailArgs args, string text, long updateId)
        {
            try
            {
                if (text.IsNullOrWhiteSpace())
                {
                    return ActionBadRequest("Update cannot be empty");
                }

                var api = new MondayApi();
                var currentPerson = GetCurrentPerson();
                var itemId = args.MondayItemId;
                var updateBodyWithUser = text.Insert(0, string.Format("\n[[{0}]] ", currentPerson.Email));

                var sanitizedUpdateBody = updateBodyWithUser.Replace(@"""", @"\""");
                sanitizedUpdateBody = sanitizedUpdateBody.Replace(@"\", @"\\");

                var newUpdate = api.AddUpdateToItem(itemId, sanitizedUpdateBody, updateId);
                var updatePosted = newUpdate != null;

                if (updatePosted)
                {
                    return ActionOk(new MondayUpdateBag
                    {
                        Id = newUpdate.Id.ToString(),
                        TextBody = newUpdate.TextBody.ToString(),
                        CreatorName = newUpdate.CreatorName,
                        CreatedAt = newUpdate.CreatedAt.ToString()
                    });
                }

                return ActionBadRequest("Unable to post update");

            }
            catch (Exception ex)
            {
                return ActionBadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Sets the status column in Monday.com to the configured closed value
        /// </summary>
        /// <returns></returns>
        [BlockAction]
        public BlockActionResult ChangeColumnValue(MondayItemDetailArgs args, string statusChange)
        {
            try
            {
                if (statusChange != "Approve" && statusChange != "Close")
                {
                    return ActionBadRequest("Column Value cannot be changed");
                }

                var api = new MondayApi();
                var itemId = args.MondayItemId;
                var boardId = args.MondayBoardId;
                var board = GetBoard(boardId);
                var newStatus = statusChange == "Approve" ? board.MondayStatusApprovedValue : board.MondayStatusClosedValue;

                var columnValue = api.ChangeColumnValue(boardId, itemId, board.MondayStatusColumnId, newStatus);

                if (columnValue == null)
                {
                    return ActionBadRequest("Error: unable to change status of item.");
                }

                return ActionOk(new
                {
                    StatusText = columnValue.Text,
                    Color = columnValue.LabelStyle.Color
                });

            } catch ( Exception ex)
            {
                return ActionBadRequest(ex.Message);
            }
        }

        #endregion
    }
}
