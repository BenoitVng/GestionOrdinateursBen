
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionOrdinateurs.Shared;


public enum EtatOrdinateur
{
    EnService,
    EnPanne,
    EnMaintenance,
    HorsService,
    Lent,
    Reparable
}

[Table("Ordinateurs")]
public class Ordinateur
{
    public int Id { get; set; }

    public string Nom { get; set; } = string.Empty;

    public EtatOrdinateur Etat { get; set; }

    [DefaultValue(0)]
    public int Stock { get; set; }

    [DefaultValue(0)]
    public decimal Prix { get; set; }

    public Caracteristiques Caracteristiques { get; set; } = new();
    public List<Reparation> Reparations { get; set; } = new();
    public bool Vendu {get; set;} = false;

    public Ordinateur() { }

    public Ordinateur(
        string nom,
        EtatOrdinateur etat,
        Caracteristiques caracteristiques)
    {
        Nom = nom;
        Etat = etat;
        Caracteristiques = caracteristiques;
    }

    public void changerEtat(EtatOrdinateur nouvelEtat)
    {
        Etat = nouvelEtat;
    }

    public void changerNom(string nouveauNom)
    {
        Nom = nouveauNom;
    }

    public void changerMarque(string nouvelleMarque)
    {
        Caracteristiques.Marque = nouvelleMarque;
    }

    public void changerRAM(int nouvelleRAM)
    {
        Caracteristiques.RamGo = nouvelleRAM;
    }

    public void changerProcesseur(string nouveauProcesseur)
    {
        Caracteristiques.Processeur = nouveauProcesseur;
    }

    // Accesseurs
    public int getId() => Id;

    public string getNom() => Nom;

    public string getMarque() => Caracteristiques.Marque;

    public int getRAM() => Caracteristiques.RamGo;

    public string getProcesseur() => Caracteristiques.Processeur;

    public EtatOrdinateur getEtatOrdinateur() => Etat;
}
