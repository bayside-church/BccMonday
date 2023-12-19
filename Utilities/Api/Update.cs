using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class Update : IUpdate
    {
        [JsonProperty("id")]
        public long Id { get; set; }


        private string _body { get; set; }

        [JsonProperty("body")]
        public string Body
        {
            get => _body;
            set => _body = RemoveEmailFromText(value);
        }

        private string _textBody { get; set; }

        [JsonProperty("text_body")]
        public string TextBody
        {
            get => _textBody;
            set => _textBody = RemoveEmailFromText(value);
        }

        private DateTime _createdAt;
        [JsonProperty("created_at")]
        public DateTime CreatedAt
        {
            get => _createdAt.ToLocalTime();
            set => _createdAt = value.ToLocalTime();
        }

        [JsonProperty("creator")]
        public MondayCreator Creator { get; set; }


        private string _creatorName { get; set; }
        public string CreatorName => _creatorName ?? Creator.Name;

        [JsonProperty("creator_id")]
        public string CreatorId { get; set; }

        [JsonProperty("assets", ItemConverterType =typeof(ConcreteConverter<IAsset, Asset>))]
        public List<IAsset> Files { get; set; }

        [JsonProperty("replies", ItemConverterType = typeof(ConcreteConverter<IUpdate, Update>))]
        public List<IUpdate> Replies { get; set; }

        // returns true if the update is created by a user that shouldn't be displayed in Rock
        // - Automations user (id = -4)
        private bool IsNotBlacklistedUser(dynamic data)
        {
            if (data["creator"]["id"] != null && data["creator"]["id"].Value == -4)
                return false;

            return true;
        }

        private string GetCreatorNameFromBody(string body)
        {
            if (!string.IsNullOrWhiteSpace(body))
            {
                var match = Regex.Match(body, @"\[\[.*\]\]");

                if (string.IsNullOrWhiteSpace(match.Value))
                    return null;

                var name = match.Value;
                name = name.Replace(@"[[", "");
                name = name.Replace(@"]]", "");

                return name;
            }

            return null;
        }

        private string RemoveEmailFromText(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var emailMatch = Regex.Match(text, @"\[\[.*\]\]");
                var uFEFFMatch = new Regex(@"\uFEFF");

                if (!string.IsNullOrWhiteSpace(emailMatch.Value))
                {
                    this._creatorName = emailMatch.Value;
                    _creatorName = _creatorName.Replace(@"[[", "");
                    _creatorName = _creatorName.Replace(@"]]", "");

                    text = text.Substring(emailMatch.Length);
                    text = text.Trim();
                }
                text = uFEFFMatch.Replace(text, "", 1, 0);
                text = uFEFFMatch.Replace(text, "\n");

                return text;
            }

            return null;
        }
    }

    public class MondayCreator
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string CreatorId { get; set; }
    }
}
