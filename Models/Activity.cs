using System.ComponentModel.DataAnnotations;
using PariniFSL.Data;

namespace PariniFSL.Models;

public class Activity
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Il titolo è obbligatorio.")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descrizione è obbligatoria.")]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il soggetto ospitante è obbligatorio.")]
    public string SoggettoOspitante { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il numero di ore è obbligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Le ore devono essere maggiori di zero.")]
    public int? OreComplessive { get; set; }

    [Required(ErrorMessage = "La sede dell'attività è obbligatoria.")]
    public string SedeAttivita { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il periodo è obbligatorio.")]
    public string Periodo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il docente referente è obbligatorio.")]
    public string DocenteReferenteId { get; set; } = string.Empty;

    public ApplicationUser? DocenteReferente { get; set; }

    [Required(ErrorMessage = "La tipologia è obbligatoria.")]
    public string Tipologia { get; set; } = string.Empty;
}
