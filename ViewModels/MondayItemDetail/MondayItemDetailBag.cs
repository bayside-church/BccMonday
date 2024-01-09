using com.baysideonline.BccMonday.Utilities.Api.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.ViewModels.MondayItemDetail
{
    public class MondayItemDetailBag
    {
        /// <summary>
        /// The Monday.com Item
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// The Monday.com Item's Updates
        /// </summary>
        //public List<Update> Updates { get; set; }

        /// <summary>
        /// The Monday.com Item's Name
        /// </summary>
        public string Name { get; set; }
    }
}
