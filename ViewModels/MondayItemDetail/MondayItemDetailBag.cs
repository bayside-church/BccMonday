using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using com.baysideonline.BccMonday.Utilities.Api.Schema;
using Rock.Web.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.ViewModels.MondayItemDetail
{
    public class MondayItemDetailBag
    {
        public MondayItemBag Item { get; set; }

        public string Status { get; set; }
        public int StatusIndex { get; set; }

        public bool ShowApprove { get; set; }
        public bool ShowClose { get; set; }
    }

    public class MondayItemBag
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public MondayBoardBag Board { get; set; }
        public List<MondayUpdateBag> Updates { get; set; }
        public List<MondayColumnValueBag> ColumnValues { get; set; }
    }

    public class MondayBoardBag
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class MondayUpdateBag
    {
        public string Id { get; set; }
        public string CreatedAt { get; set; }
        public string CreatorName { get; set; }
        public string TextBody { get; set; }
        public List<MondayUpdateBag> Replies { get; set; }
        public List<MondayAssetBag> Files { get; set; }
    }

    public class MondayAssetBag
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PublicUrl { get; set; }
    }

    public class MondayColumnBag
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }

    public class MondayColumnValueBag
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public MondayColumnBag Column { get; set; }

        //Properties specific to FileColumnValue
        public List<FileBag> Files { get; set; }

        //Properties specific to StatusColumnValue
        public int? Index { get; set; }
        public string StatusLabel { get; set; }
        public bool? IsDone { get; set; }
        public LabelStyle LabelStyle { get; set; }

        //Properties specific to BoardRelationColumnValue
        public string DisplayValue { get; set; }
        public List<LinkedItem> LinkedItems { get; set; }
        public List<long> LinkedItemIds { get; set; }
    }

    public class FileBag
    {
        public string AssetId { get; set; }
        public MondayAssetBag Asset { get; set; }
    }

    public class LabelStyle
    {
        public string Color { get; set; }
        public string Border { get; set; }
    }

    public class LinkedItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
