using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sample.Models;

namespace Sample.Pages
{
    public class EditPage : PageModel
    {
        private readonly TalkServiceProvider _talkService;
        public EditPage(TalkServiceProvider talkService)
        {
            _talkService = talkService;
        }

        [BindProperty]
        public UpdatedTalk Talk { get; set; }

        [BindProperty]
        public string Version { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var talkEdit = await _talkService.TryRaven(s => s.GetTalkForEditing($"Talks/{id}"));

            if (talkEdit.Talk == null)
            {
                return RedirectToPage("/Index");
            }

            Talk = talkEdit.Talk;
            Version = talkEdit.Version;

            var speakers = await _talkService.TryRaven(s => s.GetSpeakers());

            Speakers = speakers.Select(s => new SelectListItem() {
                Value = s.Id, Text = s.Name });

            return Page();
        }

        public IEnumerable<SelectListItem> Speakers { get; set; }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _talkService.TryRaven(s => s.UpdateTalk($"Talks/{id}", Talk, Version));

            return RedirectToPage("/Talk", new { id });
        }
    }
}