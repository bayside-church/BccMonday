using System;
using System.Collections.Generic;
using System.Linq;

using Rock.Data;
using Rock.Web.Cache;
using Rock.Model;

namespace com.baysideonline.BccMonday.Models
{
    public class BccMondayBoardService : Service<BccMondayBoard>
    {
        public BccMondayBoardService(RockContext context)
        : base(context)
        {

        }
    }
}
