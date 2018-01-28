using System.Linq;
using Raven.Client.Documents.Indexes;
using Sample.Models;

namespace Sample.Indexes
{
    public class Talks_BySpeaker : AbstractIndexCreationTask<Talk>
    {
        public Talks_BySpeaker()
        {
            Map = talks =>
                from talk in talks
                select new
                {
                    talk.Speaker
                };
        }
    }
}