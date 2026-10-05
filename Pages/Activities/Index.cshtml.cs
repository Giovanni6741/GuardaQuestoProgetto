using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PariniFSL.Data;
using PariniFSL.Models;

namespace PariniFSL.Pages.Activities;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Activity> Activities { get; set; } = new List<Activity>();

    public async Task OnGetAsync()
    {
        Activities = await _context.Activities
            .Include(a => a.DocenteReferente)
            .OrderBy(a => a.Title)
            .ToListAsync();
    }
}
