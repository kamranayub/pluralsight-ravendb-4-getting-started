using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sample.Models;
using Raven.Client.Documents;
using System.Linq;
using Sample.Indexes;
using Raven.Client.Documents.Session;
using Raven.Client.Documents.Queries;
using Raven.Client.Exceptions;

namespace Sample.Services
{
    public class RavenTalkService : ITalkService
    {
        private readonly IDocumentStore store;

        public RavenTalkService(IDocumentStoreHolder storeHolder)
        {
            this.store = storeHolder.Store;
        }

        public async Task<Talk> CreateTalk(NewTalk talk)
        {
            using (var session = this.store.OpenAsyncSession())
            {
                var newTalk = new Talk()
                {
                    Headline = talk.Headline,
                    Description = talk.Description,
                    Speaker = talk.Speaker
                };

                await session.StoreAsync(newTalk);
                await session.SaveChangesAsync();

                return newTalk;
            }
        }

        public async Task<TalkDetail> GetTalkDetail(string id)
        {
            using (var session = this.store.OpenAsyncSession())
            {
                var talk = await session.Include<Talk>(t => t.Speaker).LoadAsync(id);
                var speaker = await session.LoadAsync<Speaker>(talk.Speaker);

                // load other talks by speaker
                var speakerTalks = await session.Query<Talk, Talks_BySpeaker>()
                    .Where(t => t.Speaker == talk.Speaker && t.Id != id)
                    .Take(10)
                    .Select(t => new TalkSummary()
                    {
                        Id = t.Id,
                        Headline = t.Headline,
                        Description = t.Description,
                        Published = t.Published,
                        Speaker = t.Speaker,
                        SpeakerName = RavenQuery.Load<Speaker>(t.Speaker).Name
                    })
                    .ToListAsync();

                return new TalkDetail()
                {
                    Id = talk.Id,
                    Headline = talk.Headline,
                    Description = talk.Description,
                    Event = talk.Event,
                    Published = talk.Published,
                    Tags = talk.Tags,
                    Speaker = talk.Speaker,
                    SpeakerName = speaker.Name,
                    SpeakerTalks = Enumerable.ToArray(speakerTalks)
                };
            }
        }

        public async Task<Speaker[]> GetSpeakers()
        {
            using (var session = this.store.OpenAsyncSession())
            {
                var speakers = await session.Advanced.LoadStartingWithAsync<Speaker>(
                    "Speakers/", start: 0, pageSize: Constants.PageSize);

                return speakers.ToArray();
            }
        }

        public async Task<(UpdatedTalk Talk, string Version)> GetTalkForEditing(string id)
        {
            using (var session = this.store.OpenAsyncSession())
            {
                var talk = await session.LoadAsync<Talk>(id);
                var version = session.Advanced.GetChangeVectorFor(talk);
                var updatedTalk = new UpdatedTalk()
                {
                    Headline = talk.Headline,
                    Description = talk.Description,
                    Speaker = talk.Speaker
                };

                return (Talk: updatedTalk, Version: version);
            }
        }

        public async Task<Talk> UpdateTalk(string id, UpdatedTalk talk, string version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version), "Must provide version during update");
            }

            using (var session = this.store.OpenAsyncSession())
            {
                // Load existing talk
                var existingTalk = await session.LoadAsync<Talk>(id);

                // Update properties
                existingTalk.Headline = talk.Headline;
                existingTalk.Description = talk.Description;
                existingTalk.Speaker = talk.Speaker;

                try
                {
                    await session.StoreAsync(existingTalk, version, id);
                    await session.SaveChangesAsync();
                }
                catch (ConcurrencyException cex)
                {
                    throw new ApplicationException(
                        "Tried to update a talk but looks like someone else got there first! " +
                        "Try refreshing the page. Detailed explanation: " + cex.Message, cex);
                }

                return existingTalk;
            }
        }

        public async Task<bool> DeleteTalk(string id)
        {
            using (var session = this.store.OpenAsyncSession())
            {
                session.Delete(id);
                await session.SaveChangesAsync();
                return true;
            }
        }

        public async Task<TalkSummary[]> GetTalkSummaries(int page = 1)
        {
            using (var session = this.store.OpenAsyncSession())
            {
                var actualPage = Math.Max(0, page - 1);

                var talks = await session.Query<Talk>()
                    .Skip(actualPage * Constants.PageSize)
                    .Take(Constants.PageSize)
                    .Select(t => new TalkSummary()
                    {
                        Id = t.Id,
                        Headline = t.Headline,
                        Description = t.Description,
                        Published = t.Published,
                        Speaker = t.Speaker,
                        SpeakerName = RavenQuery.Load<Speaker>(t.Speaker).Name
                    }).ToListAsync();

                return talks.ToArray();
            }
        }

        public async Task<TalkSummary[]> GetTalksBySpeaker(string speaker, int show)
        {
            using (var session = this.store.OpenAsyncSession())
            {
                var actualPage = Math.Max(0, show - 1);

                var talks = await session.Query<Talk, Talks_BySpeaker>()
                    .Where(t => t.Speaker == speaker)
                    .Skip(actualPage * Constants.PageSize)
                    .Take(Constants.PageSize)
                    .Select(t => new TalkSummary()
                    {
                        Id = t.Id,
                        Headline = t.Headline,
                        Description = t.Description,
                        Published = t.Published,
                        Speaker = t.Speaker,
                        SpeakerName = RavenQuery.Load<Speaker>(t.Speaker).Name
                    }).ToListAsync();

                return talks.ToArray();
            }
        }

        public async Task<TalkSummary[]> GetTalksByTag(string tag, int show)
        {
            using (var session = this.store.OpenAsyncSession())
            {
                var actualPage = Math.Max(0, show - 1);

                var talks = await session.Query<Talk, Talks_ByTags>()
                    .Where(t => t.Tags.Contains(tag))
                    .Skip(actualPage * Constants.PageSize)
                    .Take(Constants.PageSize)
                    .Select(t => new TalkSummary()
                    {
                        Id = t.Id,
                        Headline = t.Headline,
                        Description = t.Description,
                        Published = t.Published,
                        Speaker = t.Speaker,
                        SpeakerName = RavenQuery.Load<Speaker>(t.Speaker).Name
                    }).ToListAsync();

                return talks.ToArray();
            }
        }        

        public async Task<SpeakerTalkStats[]> GetSpeakerTalkStats()
        {
            using (var session = this.store.OpenAsyncSession())
            {
                var stats = await session.Query<SpeakerTalkStats, Talks_SpeakerStats>()
                    .Take(1024).ToListAsync();

                return stats.ToArray();
            }
        }

        public async Task<TagTalkStats[]> GetTagTalkStats()
        {
            using (var session = this.store.OpenAsyncSession())
            {
                var stats = await session.Query<TagTalkStats, Talks_TagStats>()
                    .Take(1024).ToListAsync();

                return stats.ToArray();
            }
        }

        public async Task<TalkSummary[]> SearchTalks(string search, int page = 1)
        {
            using (var session = this.store.OpenAsyncSession())
            {
                var actualPage = Math.Max(0, page - 1);
                var normalizedSearch = search.ToLowerInvariant();

                var results = await session.Query<Talk, Talks_Search>()
                    .Search(t => t.Headline, search, boost: 2)
                    .Search(t => t.Description, search)
                    .Skip(actualPage * Constants.PageSize)
                    .Take(Constants.PageSize)
                    .Select(t => new TalkSummary()
                    {
                        Id = t.Id,
                        Headline = t.Headline,
                        Description = t.Description,
                        Published = t.Published,
                        Speaker = t.Speaker,
                        SpeakerName = RavenQuery.Load<Speaker>(t.Speaker).Name
                    })
                    .ToListAsync();

                return results.ToArray();
            }
        }

    }
}