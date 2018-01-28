using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sample.Models;

namespace Sample.Pages
{
    public class CreatePage : PageModel
    {
        private readonly TalkServiceProvider _talkService;
        public CreatePage(TalkServiceProvider talkService)
        {
            _talkService = talkService;
        }

        public async Task<IActionResult> OnGetAsync() {
            var speakers = await _talkService.TryRaven(s => s.GetSpeakers());

            Speakers = speakers.Select(s => new SelectListItem() {
                Value = s.Id, Text = s.Name });

            return Page();
        }

        public IEnumerable<SelectListItem> Speakers { get; set; }

        [BindProperty]
        public NewTalk Talk { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var talk = await _talkService.TryRaven(s => s.CreateTalk(Talk));

            return RedirectToPage("/Talk", new { id = talk.ClientId });
        }
    }
}