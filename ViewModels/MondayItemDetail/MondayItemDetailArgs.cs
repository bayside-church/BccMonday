using com.baysideonline.BccMonday.Utilities.Api.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.ViewModels.MondayItemDetail
{
    public class MondayItemDetailArgs
    {
        public long MondayItemId { get; set; }
        public long MondayBoardId { get; set; }

        public Board Board { get; set; }
    }
}
