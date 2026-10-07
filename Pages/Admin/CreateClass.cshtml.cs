using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PariniFSL.Data;
using PariniFSL.Models;
using System.ComponentModel.DataAnnotations;

namespace PariniFSL.Pages.Admin;

[Authorize(Roles = "Admin")]
public class CreateClassModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateClassModel(ApplicationDbContext context)
    {
        _context = context;
    }

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

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Input.Section = Input.Section.Trim().ToUpperInvariant();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var exists = _context.SchoolClasses.Any(c =>
            c.Grade == Input.Grade &&
            c.Section == Input.Section);

        if (exists)
        {
            ModelState.AddModelError(
                "Input.Section",
                "Questa classe esiste già.");

            return Page();
        }

        var schoolClass = new SchoolClass
        {
            Grade = Input.Grade,
            Section = Input.Section
        };

        _context.SchoolClasses.Add(schoolClass);

        await _context.SaveChangesAsync();

        return RedirectToPage("/Admin/Classes");
    }
}
