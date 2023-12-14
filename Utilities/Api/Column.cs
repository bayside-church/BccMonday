using Newtonsoft.Json;
using System.Collections.Generic;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class Column : IColumn
    {
        /// <summary>
        /// The column's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The coilumn's title.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// The column's type.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// The column's settings in a string form.
        /// </summary>
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

        #region Equivalence methods

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

        #endregion
    }
}
