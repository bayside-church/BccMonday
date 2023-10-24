using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using System;
using System.IO;
using System.Web.UI;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class ColumnValue : AbstractColumnValue
    {

        public override Control CreateControl(Page page)
        {
            var root = new LiteralControl();

            var text = string.IsNullOrWhiteSpace(this.Text) ? "Unknown" : this.Text;

            var stringWriter = new StringWriter();
            var writer = new HtmlTextWriter(stringWriter);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "mr-3");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "font-weight-bold");
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.Write(this.Title);
            writer.RenderEndTag();
            writer.AddStyleAttribute("min-width", "200px");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "bcc-monday-column-empty");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "copy-link-area btn-group");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(text);
            writer.RenderEndTag();
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.Title);
            writer.AddAttribute("onClick", "btnToIconFunction(id)");
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn btn-primary btn-copy js-tooltip js-copy");
            writer.AddAttribute("data-toggle", "tooltip");
            writer.AddAttribute("data-placement", "bottom");
            writer.AddAttribute("data-copy", text);
            writer.AddAttribute(HtmlTextWriterAttribute.Title, "Copy");
            writer.RenderBeginTag(HtmlTextWriterTag.Button);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.Title + " icon");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "far fa-clone");
            writer.RenderBeginTag(HtmlTextWriterTag.I);
            writer.RenderEndTag();      // </i>
            writer.RenderEndTag();      //</button>
            writer.RenderEndTag();      //</div>
            writer.RenderEndTag();      //<div>
            writer.RenderEndTag();      //</div>
            
            var html = writer.InnerWriter.ToString();
            root.Text = html;

            return root;
        }
    }
}
