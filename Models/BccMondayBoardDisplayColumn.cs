using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;

using Rock.Data;
using Rock.Model;

namespace com.baysideonline.BccMonday.Models
{
    [Table("_com_baysideonline_BccMondayBoardDisplayColumn")]
    [DataContract]
    public class BccMondayBoardDisplayColumn : Model<BccMondayBoardDisplayColumn>, IRockEntity
    {
        #region Entity Properties
        [DataMember]
        [Required]
        public string MondayColumnId { get; set; }

        [DataMember]
        [Required]
        public int BccMondayBoardId { get; set; }

        [DataMember]
        public string MondayColumnType { get; set; }

        [DataMember]
        [Required]
        public string MondayColumnTitle { get; set; }

        #endregion

        #region Virtual Properties

        public virtual BccMondayBoard BccMondayBoard { get; set; }

        #endregion
    }

    public partial class BccMondayBoardDisplayColumnConfiguration : EntityTypeConfiguration<BccMondayBoardDisplayColumn>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BccMondayBoardDisplayColumnConfiguration"/> class.
        /// </summary>
        public BccMondayBoardDisplayColumnConfiguration()
        {
            this.HasRequired(x => x.BccMondayBoard).WithMany().HasForeignKey(x => x.BccMondayBoardId).WillCascadeOnDelete(true);
        }
    }
}
