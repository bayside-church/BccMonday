using Newtonsoft.Json;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class Column : IColumn
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("settings_str")]
        public string Options { get; set; }

        public List<string> GetLabels()
        {
            var labels = new List<string>();

            var settings = JsonConvert.DeserializeObject<dynamic>(this.Options);
            if (settings != null && settings["labels"] != null)
            {
                foreach (var label in settings["labels"])
                {
                    string labelText = label.Value;

                    labels.Add(labelText);
                }
            }
            return labels;
        }
        
        public override bool Equals(object obj)
        {
            return Equals(obj as Column);
        }

        public bool Equals(IColumn other)
        {
            return other != null &&
                Title == other.Title &&
                Id == other.Id;
        }

        public override int GetHashCode()
        {
            var hashCode = -830139866;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            return hashCode;
        }
    }
}
