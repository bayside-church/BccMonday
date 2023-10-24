
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;

using Rock.Data;
using Rock.Model;

namespace com.baysideonline.BccMonday.Models
{
    [Table("_com_baysideonline_BccMondayItem")]
    [DataContract]
    public class BccMondayItem : Model<BccMondayItem>, IRockEntity
    {
        #region Entity Properites

        //Gets or sets the Monday Item ID
        [DataMember]
        public int MondayItemId { get; set; }

        [DataMember]
        [MaxLength(1024)]
        public string MondayItemName { get; set; }

        //Gets or sets the email associated with the Monday account
        [DataMember]
        [RegularExpression(@"[\w\.\'_%-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+", ErrorMessage = "The Email address is invalid")]
        public string MondayPersonEmail { get; set; }

        //Gets or sets the Person's name associated with the Monday account
        [DataMember]
        public string MondayPersonName { get; set; }

        //Gets or sets the PersonAliasId
        [DataMember]
        public int? PersonAliasId { get; set; }

        //Gets or sets if the Monday Item is open
        [DataMember]
        public bool IsOpen { get; set; }

        public PersonAlias PersonAlias { get; set; }

        #endregion

        #region Entity Configuration
        public partial class BccMondayItemconfiguration : EntityTypeConfiguration<BccMondayItem>
        {
            public BccMondayItemconfiguration()
            {
                this.HasOptional(i => i.PersonAlias).WithMany().HasForeignKey(i => i.PersonAliasId).WillCascadeOnDelete(false);

                //Important
                this.HasEntitySetName("BccMondayItem");
            }
        }
        #endregion
    }
}