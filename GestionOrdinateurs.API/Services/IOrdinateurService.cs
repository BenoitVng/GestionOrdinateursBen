using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.Services;
public interface IOrdinateurService
{
    Task<List<Ordinateur>> ListerParc();

    Task<bool> AjouterOrdinateur(Ordinateur ordinateur);
    
    Task<bool> SupprimerOrdinateur(int id);

    Task<Ordinateur?> GetById(int id);

    Task<bool> ModifierOrdinateur(int id, Ordinateur ordiMisAJour);

    Task<List<Ordinateur>> RechercherParMarque(string marque);
    Task<List<Ordinateur>> ListerOrdinateursDisponibles();
    Task<List<Ordinateur>> ListerParPrixDecroissant();
    string Diagnostiquer(Ordinateur ordinateur);
    Task<(int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations)> CalculerStatistiques();
}