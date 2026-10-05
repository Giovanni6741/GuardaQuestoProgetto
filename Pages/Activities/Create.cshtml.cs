using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PariniFSL.Data;
using PariniFSL.Models;

namespace PariniFSL.Pages.Activities;

[Authorize(Roles = "Admin,Referente")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public Activity Activity { get; set; } = new();

    public SelectList Referenti { get; set; } = null!;

    public async Task OnGetAsync()
    {
        await LoadReferentiAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // BREAKPOINT 1
        // Metti qui un breakpoint e verifica che il POST arrivi.

        if (!ModelState.IsValid)
        {
            // BREAKPOINT 2
            // Se ti fermi qui, ModelState NON è valido.
            // Espandi ModelState nella finestra Locals.

            await LoadReferentiAsync();

            return Page();
        }

        // BREAKPOINT 3
        // Se arrivi qui, ModelState è valido.
        // Controlla qui il contenuto di Activity.

        _context.Activities.Add(Activity);

        // BREAKPOINT 4
        // L'attività è stata aggiunta al DbContext,
        // ma non è ancora stata salvata nel database.

        await _context.SaveChangesAsync();

        // BREAKPOINT 5
        // Se arrivi qui, SaveChangesAsync() è terminato
        // correttamente.

        return RedirectToPage("Index");
    }

    private async Task LoadReferentiAsync()
    {
        // BREAKPOINT 6
        // Controlla che il caricamento dei referenti venga eseguito.

        var users = await _userManager.GetUsersInRoleAsync("Referente");

        // BREAKPOINT 7
        // Controlla il contenuto di users.

        Referenti = new SelectList(
            users,
            "Id",
            "UserName");
    }
}
