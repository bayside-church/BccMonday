using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class FileColumnValue : AbstractColumnValue
    {
        public List<IFile> Files { get; set; }

        public List<long> AssetIds { get; set; }

       
        public override Control CreateControl(Page page)
        {
            var root = new LiteralControl();
            var stringWriter = new StringWriter();
            var writer = new HtmlTextWriter(stringWriter);

            var api = new MondayApi();
            var response = api.GetFilesByAssetIds(this.AssetIds);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "mr-3");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "font-weight-bold");
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.Write(this.Column.Title);
            writer.RenderEndTag();
            writer.AddStyleAttribute("min-width", "200px");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (response != null)
            {
                foreach (var file in response)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, file.PublicUrl);
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.Write(file.Name);
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                }
            }
            
            writer.RenderEndTag();
            writer.RenderEndTag();
            
            var html = writer.InnerWriter.ToString();
            root.Text = html;

            return root;
        }
    }
}
