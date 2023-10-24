﻿using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class ItemListColumnValue : AbstractColumnValue
    {
        public List<long> ItemIds { get; set; }

        public override Control CreateControl(Page page)
        {
            var root = new LiteralControl();
            var stringWriter = new StringWriter();
            var writer = new HtmlTextWriter(stringWriter);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "mr-3");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "font-weight-bold");
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.Write(Title);
            writer.RenderEndTag();
            writer.AddStyleAttribute("min-width", "200px");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            foreach(var linkedItemId in ItemIds)
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
