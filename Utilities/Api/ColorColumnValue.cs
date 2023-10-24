using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using System.IO;
using System.Web.UI;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class ColorColumnValue : AbstractColumnValue
    {
        public string Color { get; set; }

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
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "text-center");
            writer.AddStyleAttribute("background-color", this.Color);
            writer.AddStyleAttribute("text-overflow", "ellipsis");
            writer.AddStyleAttribute("white-space", "nowrap");
            writer.AddStyleAttribute("overflow", "hidden");
            writer.AddStyleAttribute("padding", "11px 22px");
            writer.AddStyleAttribute("min-width", "200px");
            writer.AddStyleAttribute("max-width", "250px");
            writer.AddStyleAttribute("color", "#ffffff");
            writer.AddStyleAttribute("border-radius", "10px");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(text);
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();

            var html = writer.InnerWriter.ToString();
            root.Text = html;

            return root;
        }
    }
}
