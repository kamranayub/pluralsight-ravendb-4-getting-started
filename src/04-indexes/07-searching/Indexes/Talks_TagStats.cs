using System.Linq;
using Raven.Client.Documents.Indexes;
using Sample.Models;

namespace Sample.Indexes
{
    public class Talks_TagStats : AbstractIndexCreationTask<Talk, TagTalkStats>
    {
        public Talks_TagStats()
        {
            Map = talks =>
                from talk in talks
                from tag in talk.Tags
                select new
                {
                    Tag = tag,
                    Count = 1
                };

            Reduce = results =>
                from rr in results
                group rr by rr.Tag into g
                select new
                {
                    Tag = g.Key,
                    Count = g.Sum(r => r.Count)
                };
        }
    }
}