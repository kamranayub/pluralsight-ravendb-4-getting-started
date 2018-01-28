using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Raven.Client.Documents;
using Sample.Services;
using Sample.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Sample.Pages
{
    public class IndexPage : PageModel
    {
        private readonly TalkServiceProvider _talkService;
        public IndexPage(TalkServiceProvider talkService)
        {
            _talkService = talkService;
        }

        public TalkSummary[] Talks { get; set; }

        public IEnumerable<SelectListItem> Speakers { get; set; }

        public IEnumerable<SelectListItem> Tags { get; set; }

        public string Speaker { get; set; }

        public string Tag { get; set; }

        public int NextPage { get; set; }

        public bool HasNextPage { get; set; }

        public async Task<IActionResult> OnGetAsync(string search, string speaker, string tag, int show = 1)
        {
            if (string.IsNullOrWhiteSpace(speaker) &&
                string.IsNullOrWhiteSpace(tag) &&
                string.IsNullOrWhiteSpace(search))
            {
                Talks = await _talkService.TryRaven(s => s.GetTalkSummaries(show));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                Talks = await _talkService.TryRaven(s => s.SearchTalks(search, show));
            }

            if (!string.IsNullOrWhiteSpace(speaker))
            {
                Talks = await _talkService.TryRaven(s => s.GetTalksBySpeaker(speaker, show));
                Speaker = speaker;
            }

            if (!string.IsNullOrWhiteSpace(tag))
            {
                Talks = await _talkService.TryRaven(s => s.GetTalksByTag(tag, show));
                Tag = tag;
            }

            var speakers = await _talkService.TryRaven(s => s.GetSpeakerTalkStats());
            var tags = await _talkService.TryRaven(s => s.GetTagTalkStats());

            Speakers = speakers.OrderBy(stat => stat.SpeakerName).Select(stat =>
                new SelectListItem()
                {
                    Value = stat.Id,
                    Text = $"{stat.SpeakerName} ({stat.Count})"
                });

            Tags = tags.OrderBy(stat => stat.Count).Select(stat =>
                new SelectListItem()
                {
                    Value = stat.Tag,
                    Text = $"{stat.Tag} ({stat.Count})"
                });

            NextPage = show + 1;
            HasNextPage = Talks.Length >= Constants.PageSize;

            return Page();
        }
    }
}