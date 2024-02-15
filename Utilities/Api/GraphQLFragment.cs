using DotLiquid;
using Rock.IpAddress;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public static class GraphQLFragment
    {
        #region ColumnValues
        public const string MirrorValueStr = @"
            ... on MirrorValue {
                display_value
            }";

        public static GraphQLQueryBuilder MirrorValueFragment(this GraphQLQueryBuilder builder)
        {
            return builder
               .AddNestedField("... on MirrorValue", q => q
                    .AddField("display_value")
                );
        }

        public const string ButtonValueStr = @"
            ... on ButtonValue {
                id
                color
                buttonLabel: label
            }";

        public static GraphQLQueryBuilder ButtonValueFragment(this GraphQLQueryBuilder builder)
        {
            return builder
                .AddNestedField("... on ButtonValue", q => q
                    .AddField("id")
                    .AddField("color")
                    .AddField("buttonLabel", "label")
                );
        }

        public const string BoardRelationValueStr = @"
            ... on BoardRelationValue {
                value
                display_value
                linked_items {
                    id
                    relative_link
                }
                linked_item_ids
            }";

        public static GraphQLQueryBuilder BoardRelationValueFragment(this GraphQLQueryBuilder builder)
        {
            return builder
                .AddNestedField("... on BoardRelationValue", q => q
                    .AddField("value")
                    .AddField("display_value")
                    .AddField("linked_item_ids")
                    .AddNestedField("linked_items", q1 => q1
                        .AddField("id")
                        .AddField("relative_link")
                    )
                );
        }

        public const string DateValueStr = @"
            ... on DateValue {
                id
                date
                time
            }";

        public static GraphQLQueryBuilder DateValueFragment(this GraphQLQueryBuilder builder)
        {
            return builder
                .AddNestedField("... on DateValue", q => q
                    .AddField("id")
                    .AddField("date")
                    .AddField("time")
                );
        }

        public const string StatusValueStr = @"
            ... on StatusValue {
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

        public static GraphQLQueryBuilder StatusValueFragment(this GraphQLQueryBuilder builder)
        {
            return builder
                .AddNestedField("... on StatusValue", q => q
                    .AddField("id")
                    .AddField("value")
                    .AddField("index")
                    .AddField("statusLabel", "label")
                    .AddField("is_done")
                    .AddNestedField("label_style", q1 => q1
                        .AddField("color")
                        .AddField("border")
                    )
                );
        }

        public const string EmailValueStr = @"
            ... on EmailValue {
                id
                email
            }";

        public static GraphQLQueryBuilder EmailValueFragment(this GraphQLQueryBuilder builder)
        {
            return builder
                .AddNestedField("... on EmailValue", q => q
                    .AddField("id")
                    .AddField("email")
                );
        }

        public const string FileValueStr = @"
            ... on FileValue {
                id
                files {" + FileDocValueStr + FileAssetValueStr + @"
                }
            }";

        public static GraphQLQueryBuilder FileValueFragment(this GraphQLQueryBuilder builder)
        {
            return builder
                .AddNestedField("... on FileValue", q => q
                    .AddField("id")
                    .AddNestedField("files", q1 => q1
                        .FileDocValueFragment()
                        .FileAssetValueFragment()
                        
                     )
                );
        }

        public const string FileDocValueStr = @"
            ... on FileDocValue {
                file_id
                url
            }";

        public static GraphQLQueryBuilder FileDocValueFragment(this GraphQLQueryBuilder builder)
        {
            return builder
                .AddNestedField("... on FileDocValue", q => q
                    .AddField("file_id")
                    .AddField("url")
                );
        }

        public const string FileAssetValueStr = @"
            ... on FileAssetValue {
                asset_id
                asset {" + AssetPropsStr + @"
                }
            }";

        public static GraphQLQueryBuilder FileAssetValueFragment(this GraphQLQueryBuilder builder)
        {
            return builder
                .AddNestedField("... on FileAssetValue", q => q
                    .AddField("asset_id")
                    .AddNestedField("asset", q1 => q1
                        .AddField("id")
                        .AddField("public_url")
                        .AddField("name")
                        .AddField("file_size")
                        .AddField("url_thumbnail")
                    )
                );
        }

        public const string ColumnValuePropsStr = @"
            id
            text
            type
            value";

        public static GraphQLQueryBuilder ColumnValueProps(this GraphQLQueryBuilder builder)
        {
            return builder
                .AddField("id")
                .AddField("text")
                .AddField("type")
                .AddField("value");
        }

        public const string ColumnValueQueryStr = @"
            column_values {"
                + ColumnValuePropsStr + FileValueStr + StatusValueStr
                + EmailValueStr + MirrorValueStr + DateValueStr
                + BoardRelationValueStr + ButtonValueStr + ColumnQueryStr + @"
            }";
        #endregion

        #region Column
        public const string ColumnQueryStr = @"
            column { " + ColumnPropsStr + @"
            }";

        public const string ColumnPropsStr = @"
            id
            title
            settings_str
            description
            type";

        public static GraphQLQueryBuilder ColumnProps(this GraphQLQueryBuilder builder)
        {
            return builder
                .AddField("id")
                .AddField("title")
                .AddField("settings_str")
                .AddField("description")
                .AddField("type");
        }

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

        public static GraphQLQueryBuilder UpdateProps(this GraphQLQueryBuilder builder)
        {
            return builder
                .AddField("id")
                .AddField("body")
                .AddField("text_body")
                .AddField("created_at")
                .AddField("creator_id")
                .AddNestedField("creator", q => q.UserProps()
                );
        }

        public const string ReplyQueryStr = @"
            replies {" + UpdatePropsStr + @"
            }";

        public const string UpdateQueryStr = @"
            updates {"
                + UpdatePropsStr
                + AssetQueryStr
                + ReplyQueryStr + @"
            }";
        #endregion

        #region MondayUser

        public const string UserPropsStr = @"
            id
            name";

        public static GraphQLQueryBuilder UserProps(this GraphQLQueryBuilder builder)
        {
            return builder
                .AddField("id")
                .AddField("name");
        }

        #endregion

        #region Assets
        public const string AssetPropsStr = @"
            id
            public_url
            name
            file_size
            url_thumbnail";

        public static GraphQLQueryBuilder AssetProps(this GraphQLQueryBuilder builder)
        {
            return builder
                .AddField("id")
                .AddField("public_url")
                .AddField("name")
                .AddField("file_size")
                .AddField("url_thumbnail");
        }

        public const string AssetQueryStr = @"assets {" + AssetPropsStr + " }";

        #endregion

        #region Boards
        public const string BoardPropsStr = @"
            id
            name
            type";

        public const string BoardQueryStr = @"boards { " + BoardPropsStr + " }";
        #endregion

        #region Workspace
        public const string WorkspacePropsStr = @"
            id
            name";

        public const string WorkspaceQueryStr = @"workspace { " + WorkspacePropsStr + " }";

        #endregion

        #region Item
        public const string ItemPropsStr = @"
            id
            name
            created_at";

        public const string ItemQueryStr = @"items { " + ItemPropsStr + " }";

        public const string ItemBoardQueryStr = @"
            board { " + BoardPropsStr + " }";
        #endregion

    }
}
