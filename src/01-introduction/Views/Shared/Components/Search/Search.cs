using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sample.Components
{
    [ViewComponent]
    public class Search : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync() {
            string search = Request.Query["search"];

            return View((object)search);
        }
    }
}