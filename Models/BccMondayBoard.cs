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
    [Table("_com_baysideonline_BccMondayBoard")]
    [DataContract]
    public class BccMondayBoard : Model<BccMondayBoard>, IRockEntity
    {
        #region Entity Properties
        [DataMember]
        [Required]
        public long MondayBoardId { get; set; }

        [DataMember]
        public string MondayBoardName { get; set; }

        [DataMember]
        [Required]
        public string EmailMatchColumnId { get; set; }

        [DataMember]
        [MaxLength(255)]
        public string MondayStatusColumnId { get; set; }

        [DataMember]
        [MaxLength(255)]
        public string MondayStatusCompleteValue { get; set; }

        [DataMember]
        [MaxLength(255)]
        public string MondayStatusClosedValue { get; set; }

        [DataMember]
        [MaxLength(255)]
        public string MondayStatusApprovedValue { get; set; }

        [DataMember]
        [Required]
        public bool ShowApprove { get; set; }
        #endregion

        #region Virtual Properties
        public virtual ICollection<BccMondayBoardDisplayColumn> BoardDisplayColumns { get; set; }
        #endregion
    }

    public partial class BccMondayBoardConfiguration : EntityTypeConfiguration<BccMondayBoard>
    {
        public BccMondayBoardConfiguration()
        {
            this.HasMany(x => x.BoardDisplayColumns)
                .WithRequired(x => x.BccMondayBoard)
                .HasForeignKey(x => x.BccMondayBoardId);
        }
    }
}
