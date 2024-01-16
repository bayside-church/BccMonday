using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

namespace com.baysideonline.BccMonday.Utilities.Api.Schema
{
    public class FileColumnValue : AbstractColumnValue
    {

        public List<long> AssetIds { get; set; }

        [JsonProperty("files")]
        public List<FileAssetValue> Files { get; set; }
       
        public override Control CreateControl(Page page)
        {
            var root = new LiteralControl();
            var stringWriter = new StringWriter();
            var writer = new HtmlTextWriter(stringWriter);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "mr-3");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "font-weight-bold");
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.Write(this.Column.Title);
            writer.RenderEndTag();
            writer.AddStyleAttribute("min-width", "200px");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            foreach(var file in this.Files)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.AddAttribute(HtmlTextWriterAttribute.Href, file.Asset.PublicUrl);
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(file.Name);
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

    public class FileAssetValue
    {
        /// <summary>
        /// The asset associated with the file.
        /// </summary>
        [JsonProperty("asset")]
        public Asset Asset { get; set; }

        /// <summary>
        /// The asset's id.
        /// </summary>
        [JsonProperty("asset_id")]
        public long AssetId { get; set; }

        /// <summary>
        /// the file's name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
