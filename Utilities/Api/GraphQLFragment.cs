using Rock.IpAddress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class GraphQLFragment
    {
        #region ColumnValues
        public const string MirrorValueStr = @"... on MirrorValue {
                        display_value
                      }";

        public const string ButtonValueStr = @"... on ButtonValue {
                        id
                        color
                        buttonLabel: label
                      }";

        public const string BoardRelationValueStr = @"... on BoardRelationValue {
                        value
                        display_value
                        linked_items {
                          id
                          relative_link
                        }
                        linked_item_ids
                      }";

        public const string DateValueStr = @"... on DateValue {
                        id
                        date
                        time
                      }";

        public const string StatusValueStr = @"... on StatusValue {
                        id
                        value
                        index
                        statusLabel: label
                        is_done
                        label_style {
                          color
                          border
                        }
                      }";

        public const string EmailValueStr = @"... on EmailValue {
                        id
                        email
                      }";

        public const string FileValueStr = @"... on FileValue {
                        id
                        files {" + FileDocValueStr + FileAssetValueStr + @"
                        }
                    }";

        public const string FileDocValueStr = @"... on FileDocValue {
                            file_id
                            url
                          }";

        public const string FileAssetValueStr = @"... on FileAssetValue {
                            asset_id
                            asset {
                              url
                              name
                              url_thumbnail
                              public_url
                            }
                          }";

        public const string ColumnQueryStr = @"column {
                        id
                        title
                        settings_str
                        description
                        type
                      }";

        public const string ColumnValueQueryStr = @"column_values {
                      id
                      text
                      type
                      value" + FileValueStr + StatusValueStr + EmailValueStr + MirrorValueStr + DateValueStr + BoardRelationValueStr + ButtonValueStr + ColumnQueryStr + @"
                    }";
        #endregion

        #region Updates

        public const string UpdatePropsStr = @"
                        id
                        body
                        text_body
                        created_at
                        creator_id
                        creator {" + UserPropsStr + @"
                        }";

        public const string UpdateQueryStr = @"updates {" + UpdatePropsStr + @"
                      assets {" + AssetPropsStr + @"
                      }
                      replies {" + UpdatePropsStr + @"                        
                      }
                    }";
        #endregion

        #region MondayUser

        public const string UserPropsStr = @"
                    id
                    name";
        #endregion

        #region Assets
        public const string AssetPropsStr = @"id
                        public_url
                        name
                        file_size
                        url_thumbnail";
        #endregion

        #region Boards
        #endregion

        #region Workspace
        #endregion

        #region Item
        #endregion

    }
}
