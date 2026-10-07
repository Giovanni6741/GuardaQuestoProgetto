using PariniFSL.Data;

namespace PariniFSL.Models;

public class SchoolClass
{
    public int Id { get; set; }

    public int Grade { get; set; }

    public string Section { get; set; } = string.Empty;

    public ICollection<ApplicationUser> Students { get; set; }
        = new List<ApplicationUser>();
}
