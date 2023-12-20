using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class BoardRelationColumnValue : AbstractColumnValue
    {
        /// <summary>
        /// A string representing all the names of the linked items, separated by commas
        /// </summary>
        [JsonProperty("display_value")]
        public string DisplayValue { get; set; }

        /// <summary>
        /// The linked items IDs
        /// </summary>
        [JsonProperty("linked_item_ids")]
        public List<long> LinkedItemIds { get; set; }

        public override Control CreateControl(Page page)
        {
            var root = new LiteralControl();
            var stringWriter = new StringWriter();
            var writer = new HtmlTextWriter(stringWriter);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "mr-3");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "font-weight-bold");
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.Write(Column.Title);
            writer.RenderEndTag();
            writer.AddStyleAttribute("min-width", "200px");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            foreach(var linkedItemId in LinkedItemIds)
            {
                var url = page.Request.Url.ToString();
                var query = page.Request.Url.Query;

                if (query.Length > 0)
                    url = url.Replace(page.Request.Url.Query, "");

                url += "?MondayItemId=" + linkedItemId;

                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.AddAttribute(HtmlTextWriterAttribute.Href, url);
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write("Item");
                writer.RenderEndTag();
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
            writer.RenderEndTag();
            
            var html = writer.InnerWriter.ToString();
            root.Text = html;

            return root;
        }
    }
}
