using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sample.Models;
using Raven.Client.Documents;
using System.Linq;
using Raven.Client.Documents.Session;
using Raven.Client.Documents.Queries;

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
                var talk = await session.Include<Talk>(
                    t => t.Speaker).LoadAsync(id);
                var speaker = await session.LoadAsync<Speaker>(talk.Speaker);

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
                    SpeakerTalks = new TalkSummary[] { }
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
                var updatedTalk = new UpdatedTalk()
                {
                    Headline = talk.Headline,
                    Description = talk.Description,
                    Speaker = talk.Speaker
                };

                return (Talk: updatedTalk, Version: null);
            }
        }   

        public async Task<Talk> UpdateTalk(string id, UpdatedTalk talk, string version)
        {
            using (var session = this.store.OpenAsyncSession())
            {
                // Load existing talk
                var existingTalk = await session.LoadAsync<Talk>(id);

                // Update properties
                existingTalk.Headline = talk.Headline;
                existingTalk.Description = talk.Description;
                existingTalk.Speaker = talk.Speaker;

                await session.SaveChangesAsync();

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

        public async Task<SpeakerTalkStats[]> GetSpeakerTalkStats()
        {
            throw new NotImplementedException("TODO: Implement GetSpeakerTalkStats");
        }

        public async Task<TagTalkStats[]> GetTagTalkStats()
        {
            throw new NotImplementedException("TODO: Implement GetTagTalkStats");
        }

        public async Task<TalkSummary[]> GetTalkSummaries(int page = 1)
        {
            throw new NotImplementedException("TODO: Implement GetTalkSummaries");
        }

        public async Task<TalkSummary[]> GetTalksBySpeaker(string speaker, int show)
        {
            throw new NotImplementedException("TODO: Implement GetTalksBySpeaker");
        }

        public async Task<TalkSummary[]> GetTalksByTag(string tag, int show)
        {
            throw new NotImplementedException("TODO: Implement GetTalksByTag");
        }

        public async Task<TalkSummary[]> SearchTalks(string search, int page = 1)
        {
            throw new NotImplementedException("TODO: Implement SearchTalks");
        }
    }
}