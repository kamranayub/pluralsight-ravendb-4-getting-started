using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Raven.Client.Documents;
using Sample.Models;

namespace Sample.Pages
{
    public class TalkPage : PageModel
    {
        private readonly TalkServiceProvider _talkService;
        public TalkPage(TalkServiceProvider talkService)
        {
            _talkService = talkService;
        }

        public TalkDetail Talk { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Talk = await _talkService.TryRaven(s => s.GetTalkDetail($"Talks/{id}"));

            if (Talk == null)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            await _talkService.TryRaven(s => s.DeleteTalk($"Talks/{id}"));

            return RedirectToPage("/Index");
        }
    }
}