using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PariniFSL.Data;

namespace PariniFSL.Pages.Admin;

[Authorize(Roles = "Admin")]
public class UsersModel : PageModel
{
    public string? CurrentUserId { get; set; }
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

    public Dictionary<string, IList<string>> UserRoles { get; set; } = new();

    public async Task OnGetAsync()
    {
        CurrentUserId = _userManager.GetUserId(User);

        Users = _userManager.Users
            .OrderBy(u => u.Email)
            .ToList();

        foreach (var user in Users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            UserRoles[user.Id] = roles;
        }
    }
}
