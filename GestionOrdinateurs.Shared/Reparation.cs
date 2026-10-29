using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionOrdinateurs.Shared;

[Table("Reparation")]
public class Reparation
{
    public int Id { get; set; }

    // Ordinateur concerné par la réparation.
    public Ordinateur UnOrdinateur { get; set; } = null!;

    public string TypeReparation { get; set; } = string.Empty;

    public DateTime DateDebut { get; set; }

    public string Statut { get; set; } = string.Empty;
    
    public string Technicien { get; set; } = string.Empty;

}