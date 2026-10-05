using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PariniFSL.Data;
using System.ComponentModel.DataAnnotations;

namespace PariniFSL.Pages.Admin;

[Authorize(Roles = "Admin")]
public class CreateUserModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public CreateUserModel(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> Roles { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "L'email è obbligatoria.")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La conferma della password è obbligatoria.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Le password non coincidono.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Devi selezionare un ruolo.")]
        public string Role { get; set; } = string.Empty;
    }

    public async Task OnGetAsync()
    {
        await LoadRolesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadRolesAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var roleExists = await _roleManager.RoleExistsAsync(Input.Role);

        if (!roleExists)
        {
            ModelState.AddModelError(
                "Input.Role",
                "Il ruolo selezionato non esiste.");

            return Page();
        }

        var existingUser = await _userManager.FindByEmailAsync(Input.Email);

        if (existingUser != null)
        {
            ModelState.AddModelError(
                "Input.Email",
                "Esiste già un utente con questa email.");

            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(
            user,
            Input.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return Page();
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            Input.Role);

        if (!roleResult.Succeeded)
        {
            // Se l'assegnazione del ruolo fallisce,
            // eliminiamo l'utente appena creato per evitare
            // di lasciare un account senza ruolo.
            await _userManager.DeleteAsync(user);

            foreach (var error in roleResult.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return Page();
        }

        return RedirectToPage("/Admin/Users");
    }

    private async Task LoadRolesAsync()
    {
        Roles = await _roleManager.Roles
            .OrderBy(r => r.Name)
            .Select(r => new SelectListItem
            {
                Value = r.Name!,
                Text = r.Name!
            })
            .ToListAsync();
    }
}
