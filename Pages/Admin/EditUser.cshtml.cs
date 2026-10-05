using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PariniFSL.Data;

namespace PariniFSL.Pages.Admin;

[Authorize(Roles = "Admin")]
public class EditUserModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public EditUserModel(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public ApplicationUser? User { get; set; }

    public IList<string> Roles { get; set; } = new List<string>();

    [BindProperty]
    public string SelectedRole { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        User = await _userManager.FindByIdAsync(id);

        if (User == null)
        {
            return NotFound();
        }

        var currentUserId = _userManager.GetUserId(HttpContext.User);

        if (User.Id == currentUserId)
        {
            return Forbid();
        }

        Roles = await _roleManager.Roles
            .Select(r => r.Name!)
            .ToListAsync();

        var userRoles = await _userManager.GetRolesAsync(User);

        SelectedRole = userRoles.FirstOrDefault() ?? string.Empty;

        return Page();
    }


    public async Task<IActionResult> OnPostAsync(string id)
    {
        User = await _userManager.FindByIdAsync(id);

        if (User == null)
        {
            return NotFound();
        }

        var currentUserId = _userManager.GetUserId(HttpContext.User);

        if (User.Id == currentUserId)
        {
            return Forbid();
        }


        if (!await _roleManager.RoleExistsAsync(SelectedRole))
        {
            ModelState.AddModelError(
                nameof(SelectedRole),
                "Ruolo non valido.");

            Roles = await _roleManager.Roles
                .Select(r => r.Name!)
                .ToListAsync();

            return Page();
        }

        var currentRoles = await _userManager.GetRolesAsync(User);

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

                Roles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .ToListAsync();

                return Page();
            }
        }

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

            Roles = await _roleManager.Roles
                .Select(r => r.Name!)
                .ToListAsync();

            return Page();
        }

        return RedirectToPage("Users");
    }
}

