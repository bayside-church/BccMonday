﻿using com.baysideonline.BccMonday.Utilities.Api.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace com.baysideonline.BccMonday.Utilities.Api.Schema
{
    public class Update : IUpdate
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        private string _body { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("body")]
        public string Body
        {
            get => _body;
            set => _body = RemoveEmailFromText(value);
        }

        private string _textBody { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("text_body")]
        public string TextBody
        {
            get => _textBody;
            set => _textBody = RemoveEmailFromText(value);
        }

        private DateTime _createdAt;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreatedAt
        {
            get => _createdAt.ToLocalTime();
            set => _createdAt = value.ToLocalTime();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("creator")]
        [JsonConverter(typeof(ConcreteConverter<IMondayUser, MondayUser>))]
        public IMondayUser Creator { get; set; }


        private string _creatorName { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string CreatorName => _creatorName ?? Creator.Name;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("creator_id")]
        public string CreatorId { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("assets", ItemConverterType =typeof(ConcreteConverter<IAsset, Asset>))]
        public List<Asset> Assets { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [JsonProperty("replies", ItemConverterType = typeof(ConcreteConverter<IUpdate, Update>))]
        public List<IUpdate> Replies { get; set; }

        /// <summary>
        /// returns true if the update is created by a user that shouldn't be displayed in Rock. Automations user (id = -4)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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
                var brMatch = new Regex(@"<br>");

                if (!string.IsNullOrWhiteSpace(emailMatch.Value))
                {
                    this._creatorName = emailMatch.Value;
                    _creatorName = _creatorName.Replace(@"[[", "");
                    _creatorName = _creatorName.Replace(@"]]", "");

                    var preText = text.Substring(0, emailMatch.Index);
                    var postText = text.Substring(emailMatch.Index + emailMatch.Length);
                    text = preText + postText;
                    text = uFEFFMatch.Replace(text, "", 1, 0);
                    text = uFEFFMatch.Replace(text, "\n");
                    text = brMatch.Replace(text, "", 1);
                }
                text = uFEFFMatch.Replace(text, "", 1, 0);
                text = uFEFFMatch.Replace(text, "\n");

                return text;
            }

            return null;
        }
    }
}
