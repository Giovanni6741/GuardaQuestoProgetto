using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PariniFSL.Data;

namespace PariniFSL.Pages.Admin;

[Authorize(Roles = "Admin")]
public class EditUserModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public EditUserModel(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public ApplicationUser? User { get; set; }

    public IList<string> Roles { get; set; } = new List<string>();

    public List<SelectListItem> SchoolClasses { get; set; } = new();

    [BindProperty]
    public string SelectedRole { get; set; } = string.Empty;

    [BindProperty]
    public int? SelectedSchoolClassId { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        User = await _userManager.FindByIdAsync(id);

        if (User == null)
        {
            return NotFound();
        }

        // Impedisce all'Admin di modificare il proprio account
        var currentUserId = _userManager.GetUserId(HttpContext.User);

        if (User.Id == currentUserId)
        {
            return Forbid();
        }

        await LoadDataAsync();

        var userRoles = await _userManager.GetRolesAsync(User);

        SelectedRole = userRoles.FirstOrDefault() ?? string.Empty;
        SelectedSchoolClassId = User.SchoolClassId;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        User = await _userManager.FindByIdAsync(id);

        if (User == null)
        {
            return NotFound();
        }

        // Impedisce all'Admin di modificare il proprio account
        var currentUserId = _userManager.GetUserId(HttpContext.User);

        if (User.Id == currentUserId)
        {
            return Forbid();
        }

        await LoadDataAsync();

        // Verifica che il ruolo esista
        if (!await _roleManager.RoleExistsAsync(SelectedRole))
        {
            ModelState.AddModelError(
                nameof(SelectedRole),
                "Ruolo non valido.");

            return Page();
        }

        // Se l'utente è uno studente, la classe è obbligatoria
        if (SelectedRole == "Studente")
        {
            if (!SelectedSchoolClassId.HasValue)
            {
                ModelState.AddModelError(
                    nameof(SelectedSchoolClassId),
                    "Devi selezionare una classe.");

                return Page();
            }

            var schoolClass = await _context.SchoolClasses
                .FindAsync(SelectedSchoolClassId.Value);

            if (schoolClass == null)
            {
                ModelState.AddModelError(
                    nameof(SelectedSchoolClassId),
                    "La classe selezionata non esiste.");

                return Page();
            }
        }
        else
        {
            // Solo gli studenti possono appartenere a una classe
            SelectedSchoolClassId = null;
        }

        // Recupera i ruoli attuali
        var currentRoles = await _userManager.GetRolesAsync(User);

        // Rimuove i vecchi ruoli
        if (currentRoles.Count > 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(
                User,
                currentRoles);

            if (!removeResult.Succeeded)
            {
                foreach (var error in removeResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return Page();
            }
        }

        // Aggiunge il nuovo ruolo
        var addResult = await _userManager.AddToRoleAsync(
            User,
            SelectedRole);

        if (!addResult.Succeeded)
        {
            foreach (var error in addResult.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return Page();
        }

        // Aggiorna la classe
        User.SchoolClassId = SelectedSchoolClassId;

        var updateResult = await _userManager.UpdateAsync(User);

        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return Page();
        }

        return RedirectToPage("Users");
    }

    private async Task LoadDataAsync()
    {
        Roles = await _roleManager.Roles
            .OrderBy(r => r.Name)
            .Select(r => r.Name!)
            .ToListAsync();

        SchoolClasses = await _context.SchoolClasses
            .OrderBy(c => c.Grade)
            .ThenBy(c => c.Section)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Grade}{c.Section}"
            })
            .ToListAsync();
    }
}
