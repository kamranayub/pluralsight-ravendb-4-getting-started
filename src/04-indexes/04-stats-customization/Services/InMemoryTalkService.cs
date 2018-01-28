using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using DuoVia.FuzzyStrings;
using Sample.Models;

namespace Sample.Services
{
    public class InMemoryTalkService : ITalkService
    {
        public static IEnumerable<Talk> Talks { get; private set; }
        public static IEnumerable<Speaker> Speakers { get; private set; }

        public Task<Talk> CreateTalk(NewTalk talk)
        {
            var newTalk = new Talk()
            {
                Id = "Talks/" + (Talks.Select(t => int.Parse(DocumentId.NoPrefix(t.Id))).Max() + 1),
                Headline = talk.Headline,
                Description = talk.Description,
                Speaker = talk.Speaker
            };

            Talks = new List<Talk>(Talks)
            {
                newTalk
            };

            return Task.FromResult(newTalk);
        }

        public async Task<TalkDetail> GetTalkDetail(string id)
        {
            var detail = Talks.Select(MapTalkToDetail).FirstOrDefault(t => t.Id == id);

            detail.SpeakerTalks = await GetOtherTalksBySpeaker(id);

            return detail;
        }

        public Task<Talk> UpdateTalk(string id, UpdatedTalk talk, string version)
        {
            var oldTalk = Talks.FirstOrDefault(t => t.Id == id);

            oldTalk.Headline = talk.Headline;
            oldTalk.Description = talk.Description;

            return Task.FromResult(oldTalk);
        }

        public Task<bool> DeleteTalk(string id)
        {
            Talks = Talks.Where(t => t.Id != id);
            return Task.FromResult(true);
        }

        public Task<Speaker[]> GetSpeakers()
        {
            return Task.FromResult(Speakers.ToArray());
        }

        public Task<SpeakerTalkStats[]> GetSpeakerTalkStats()
        {
            var stats = from talk in Talks
                        let speakerName = Speakers.First(s => s.Id == talk.Speaker).Name
                        group talk by speakerName into g
                        select new SpeakerTalkStats()
                        {
                            SpeakerName = g.Key,
                            Id = g.First().Speaker,
                            Count = g.Count()
                        };

            return Task.FromResult(stats.ToArray());
        }

        public Task<TagTalkStats[]> GetTagTalkStats()
        {
            var stats = from talk in Talks
                        from tag in talk.Tags
                        group tag by tag into g
                        select new TagTalkStats()
                        {
                            Tag = g.Key,
                            Count = g.Count()
                        };

            return Task.FromResult(stats.ToArray());
        }

        public Task<(UpdatedTalk Talk, string Version)> GetTalkForEditing(string id)
        {
            var talk = (from t in Talks
                        where t.Id == id
                        select new UpdatedTalk()
                        {
                            Headline = t.Headline,
                            Description = t.Description
                        }).FirstOrDefault();

            return Task.FromResult(
                (Talk: talk, Version: string.Empty));
        }

        public Task<TalkSummary[]> GetTalkSummaries(int page)
        {
            var actualPage = Math.Max(0, page - 1);
            return Task.FromResult(Talks.Skip(actualPage * Constants.PageSize).Take(Constants.PageSize)
                .Select(MapTalkToSummary).ToArray());
        }

        public Task<TalkSummary[]> GetTalksBySpeaker(string speaker, int show)
        {
            var actualPage = Math.Max(0, show - 1);
            var talks = Talks.Where(t => t.Speaker == speaker)
                .Skip(actualPage * Constants.PageSize).Take(Constants.PageSize)
                .Select(MapTalkToSummary).ToArray();

            return Task.FromResult(talks);
        }

        public Task<TalkSummary[]> GetTalksByTag(string tag, int show)
        {
            var actualPage = Math.Max(0, show - 1);
            var talks = Talks.Where(t => t.Tags.Contains(tag))
                .Skip(actualPage * Constants.PageSize).Take(Constants.PageSize)
                .Select(MapTalkToSummary).ToArray();

            return Task.FromResult(talks);
        }

        public Task<TalkSummary[]> SearchTalks(string search, int page)
        {
            var actualPage = Math.Max(0, page - 1);
            var normalizedSearch = search.ToLowerInvariant();
            var talks = Talks.Where(t =>
                t.Headline.ToLowerInvariant().Contains(normalizedSearch) ||
                t.Description.ToLowerInvariant().Contains(normalizedSearch));

            return Task.FromResult(talks.Skip(actualPage * Constants.PageSize).Take(Constants.PageSize)
                .Select(MapTalkToSummary).ToArray());
        }

        private Task<TalkSummary[]> GetOtherTalksBySpeaker(string id)
        {
            var talk = Talks.FirstOrDefault(t => t.Id == id);
            var talks = Talks.Where(t => t.Id != id && t.Speaker == talk.Speaker)
                .Take(10).Select(MapTalkToSummary).ToArray();

            return Task.FromResult(talks);
        }


        public static TalkSummary MapTalkToSummary(Talk t)
        {
            return new TalkSummary()
            {
                Id = t.Id,
                Description = t.Description,
                Headline = t.Headline,
                Published = t.Published,
                Speaker = t.Speaker,
                SpeakerName = Speakers.First(s => s.Id == t.Speaker).Name
            };
        }

        public static TalkDetail MapTalkToDetail(Talk t)
        {
            return new TalkDetail()
            {
                Headline = t.Headline,
                Description = t.Description,
                Event = t.Event,
                Id = t.Id,
                Published = t.Published,
                Speaker = t.Speaker,
                SpeakerName = Speakers.First(s => s.Id == t.Speaker).Name,
                Tags = t.Tags
            };
        }

        public static void Load(List<Talk> talks, List<Speaker> speakers)
        {
            Talks = talks;
            Speakers = speakers;
        }
    }
}