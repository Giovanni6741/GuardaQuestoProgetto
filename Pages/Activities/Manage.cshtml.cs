using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PariniFSL.Data;
using PariniFSL.Models;

namespace PariniFSL.Pages.Activities;

[Authorize(Roles = "Admin,Referente,Docente tutor")]
public class ManageModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ManageModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Activity Activity { get; set; } = null!;

    public List<Enrollment> Enrollments { get; set; } = new();

    public List<StudentViewModel> AvailableStudents { get; set; } = new();

    [BindProperty]
    public List<string> SelectedStudentIds { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var activity = await _context.Activities
            .Include(a => a.DocenteReferente)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (activity == null)
        {
            return NotFound();
        }

        Activity = activity;

        await LoadStudentsAsync(id);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var activity = await _context.Activities
            .Include(a => a.DocenteReferente)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (activity == null)
        {
            return NotFound();
        }

        Activity = activity;

        if (SelectedStudentIds.Count == 0)
        {
            ModelState.AddModelError(
                nameof(SelectedStudentIds),
                "Seleziona almeno uno studente.");

            await LoadStudentsAsync(id);

            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return Challenge();
        }

        // Recuperiamo solo utenti che sono effettivamente studenti.
        var students = await _context.Users
            .Where(u =>
                SelectedStudentIds.Contains(u.Id) &&
                u.SchoolClassId != null)
            .ToListAsync();

        // Verifichiamo quali studenti sono già iscritti.
        var alreadyEnrolled = await _context.Enrollments
            .Where(e =>
                e.ActivityId == id &&
                SelectedStudentIds.Contains(e.StudentId))
            .Select(e => e.StudentId)
            .ToListAsync();

        var newStudentIds = students
            .Select(s => s.Id)
            .Except(alreadyEnrolled)
            .ToList();

        foreach (var studentId in newStudentIds)
        {
            _context.Enrollments.Add(new Enrollment
            {
                StudentId = studentId,
                ActivityId = id,
                EnrolledById = currentUser.Id,
                EnrolledAt = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            $"{newStudentIds.Count} studente/i aggiunto/i all'attività.";

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRemoveAsync(int id, string studentId)
    {
        var activity = await _context.Activities
            .FirstOrDefaultAsync(a => a.Id == id);

        if (activity == null)
        {
            return NotFound();
        }

        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e =>
                e.ActivityId == id &&
                e.StudentId == studentId);

        if (enrollment == null)
        {
            TempData["ErrorMessage"] =
                "Lo studente non risulta iscritto a questa attività.";

            return RedirectToPage(new { id });
        }

        _context.Enrollments.Remove(enrollment);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Studente disiscritto con successo.";

        return RedirectToPage(new { id });
    }

    private async Task LoadStudentsAsync(int activityId)
    {
        Enrollments = await _context.Enrollments
            .Where(e => e.ActivityId == activityId)
            .Include(e => e.Student)
                .ThenInclude(u => u.SchoolClass)
            .Include(e => e.EnrolledBy)
            .OrderBy(e => e.Student.UserName)
            .ToListAsync();

        var enrolledStudentIds = Enrollments
            .Select(e => e.StudentId)
            .ToList();

        AvailableStudents = await _context.Users
            .Where(u =>
                u.SchoolClassId != null &&
                !enrolledStudentIds.Contains(u.Id))
            .Include(u => u.SchoolClass)
            .OrderBy(u => u.SchoolClass!.Grade)
            .ThenBy(u => u.SchoolClass!.Section)
            .ThenBy(u => u.UserName)
            .Select(u => new StudentViewModel
            {
                Id = u.Id,
                UserName = u.UserName ?? "",
                Email = u.Email ?? "",
                ClassName = u.SchoolClass != null
                    ? u.SchoolClass.Grade + u.SchoolClass.Section
                    : ""
            })
            .ToListAsync();
    }

    public class StudentViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string ClassName { get; set; } = string.Empty;
    }
}
