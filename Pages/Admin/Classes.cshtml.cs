using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PariniFSL.Data;
using PariniFSL.Models;

namespace PariniFSL.Pages.Admin;

[Authorize(Roles = "Admin")]
public class ClassesModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ClassesModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<SchoolClass> Classes { get; set; }
        = new List<SchoolClass>();

    public async Task OnGetAsync()
    {
        Classes = await _context.SchoolClasses
            .Include(c => c.Students)
            .OrderBy(c => c.Grade)
            .ThenBy(c => c.Section)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var schoolClass = await _context.SchoolClasses
            .FindAsync(id);

        if (schoolClass == null)
            return NotFound();

        _context.SchoolClasses.Remove(schoolClass);

        await _context.SaveChangesAsync();

        return RedirectToPage();
    }
}
