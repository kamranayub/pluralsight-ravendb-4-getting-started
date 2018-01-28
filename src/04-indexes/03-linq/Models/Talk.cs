using System;
using Sparrow.Json;

namespace Sample.Models
{
    public class Talk
    {
        public Talk()
        {
            Tags = new string[0];
        }

        // e.g. Talks/1
        public string Id { get; set; }

        // Strip prefix
        [JsonIgnore]
        public string ClientId => DocumentId.NoPrefix(Id);

        public string Headline { get; set; }

        public string Description { get; set; }

        public string Event { get; set; }

        public string Speaker { get; set; }

        public DateTime Published { get; set; }

        public string[] Tags { get; set; }
    }

    public class TalkSummary
    {
        public string Id { get; set; }

        // Strip prefix
        public string ClientId => DocumentId.NoPrefix(Id);        

        public string Headline { get; set; }

        public string Description { get; set; }

        public string Speaker { get; set; }

        public string SpeakerName { get; set; }

        public DateTime Published { get; set; }
    }

    public class TalkDetail
    {
        public TalkDetail()
        {
            Tags = new string[0];
        }

        // e.g. Talks/1
        public string Id { get; set; }

        // Strip prefix
        public string ClientId => DocumentId.NoPrefix(Id);

        public string Headline { get; set; }

        public string Description { get; set; }

        public string Event { get; set; }

        public string Speaker { get; set; }

        public string SpeakerName { get; set; }

        public DateTime Published { get; set; }

        public string[] Tags { get; set; }

        public TalkSummary[] SpeakerTalks { get; set; }
    }

    public class NewTalk
    {
        public string Headline { get; set; }

        public string Description { get; set; }

        public string Speaker { get; set; }
    }

    public class UpdatedTalk
    {
        public string Headline { get; set; }

        public string Description { get; set; }

        public string Speaker { get; set; }
    }
}