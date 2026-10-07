using Microsoft.AspNetCore.Identity;
using PariniFSL.Models;

namespace PariniFSL.Data;

public class ApplicationUser : IdentityUser
{
    public int? SchoolClassId { get; set; }

    public SchoolClass? SchoolClass { get; set; }
}
