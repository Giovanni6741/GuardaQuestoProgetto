using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PariniFSL.Data;
using System.ComponentModel.DataAnnotations;

namespace PariniFSL.Pages.Admin;

[Authorize(Roles = "Admin")]
public class EditClassModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditClassModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public int Id { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [Range(3, 5, ErrorMessage = "L'anno deve essere compreso tra 3 e 5.")]
        public int Grade { get; set; }

        [Required]
        [StringLength(10)]
        public string Section { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var schoolClass = await _context.SchoolClasses
            .FindAsync(id);

        if (schoolClass == null)
        {
            return NotFound();
        }

        Id = schoolClass.Id;

        Input.Grade = schoolClass.Grade;
        Input.Section = schoolClass.Section;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var schoolClass = await _context.SchoolClasses
            .FindAsync(id);

        if (schoolClass == null)
        {
            return NotFound();
        }

        Input.Section = Input.Section.Trim().ToUpperInvariant();

        if (!ModelState.IsValid)
        {
            Id = id;
            return Page();
        }

        var exists = await _context.SchoolClasses
            .AnyAsync(c =>
                c.Id != id &&
                c.Grade == Input.Grade &&
                c.Section == Input.Section);

        if (exists)
        {
            ModelState.AddModelError(
                "Input.Section",
                "Questa classe esiste già.");

            Id = id;
            return Page();
        }

        schoolClass.Grade = Input.Grade;
        schoolClass.Section = Input.Section;

        await _context.SaveChangesAsync();

        return RedirectToPage("/Admin/Classes");
    }
}
