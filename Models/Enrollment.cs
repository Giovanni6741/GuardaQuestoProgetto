using PariniFSL.Data;

namespace PariniFSL.Models;

public class Enrollment
{
    public int Id { get; set; }

    // Studente iscritto
    public string StudentId { get; set; } = string.Empty;
    public ApplicationUser Student { get; set; } = null!;

    // Attività FSL
    public int ActivityId { get; set; }
    public Activity Activity { get; set; } = null!;

    // Utente che ha effettuato l'iscrizione
    // Può essere Docente tutor, Referente o Admin
    public string EnrolledById { get; set; } = string.Empty;
    public ApplicationUser EnrolledBy { get; set; } = null!;

    // Data e ora dell'iscrizione
    public DateTime EnrolledAt { get; set; }
}