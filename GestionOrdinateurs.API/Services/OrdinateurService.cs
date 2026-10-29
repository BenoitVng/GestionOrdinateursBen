
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.Controllers;

namespace GestionOrdinateurs.Services;

public class OrdinateurService : IOrdinateurService
{
    private readonly ParcInformatiqueContext _context;
    private readonly JournalService _journalService;

    public OrdinateurService(ParcInformatiqueContext context, JournalService journalService)
    {
        _context = context;
        _journalService = journalService;
        OrdinateurAjoute += (sender, e) => AfficherMessageOutput($"Ordinateur ajouté : {e.Ordinateur.Nom} - ID {e.Ordinateur.Id}");
        OrdinateurSupprime += (sender, e) => AfficherMessageOutput($"Ordinateur supprimé : {e.Ordinateur.Nom} - ID {e.Ordinateur.Id}");
        OrdinateurMisAJour += (sender, e) => AfficherMessageOutput($"Ordinateur mis à jour : {e.Ordinateur.Nom} - ID {e.Ordinateur.Id}");
    }

    public async Task<List<Ordinateur>> ListerParc()
    {
        return await _context.Ordinateurs.ToListAsync();
    }

    public async Task<bool> AjouterOrdinateur(Ordinateur ordinateur)
    {
        if (ordinateur.Id != 0 && await GetById(ordinateur.Id) != null) return false;
        _context.Ordinateurs.Add(ordinateur);
        await _context.SaveChangesAsync();
        _journalService?.Ecrire($"{DateTime.Now:dd/MM/yyyy} - Ordinateur ajouté : {ordinateur.Nom}", "Ordinateurs");
        OrdinateurAjoute?.Invoke(this, new OrdinateurAjouteEventArgs(ordinateur));
        return true;
    }

    public async Task<bool> SupprimerOrdinateur(int id)
    {
        var ordi = await GetById(id);
        if (ordi == null) return false;
        _context.Ordinateurs.Remove(ordi);
        await _context.SaveChangesAsync();
        _journalService?.Ecrire($"{DateTime.Now:dd/MM/yyyy} - Ordinateur supprimé : {ordi.Nom}", "Ordinateurs");
        OrdinateurSupprime?.Invoke(this, new OrdinateurSupprimeEventArgs(ordi));
        return true;
    }

    public async Task<Ordinateur?> GetById(int id)
    {
        return await _context.Ordinateurs.FirstOrDefaultAsync(obj => obj.Id == id);
    }

    public async Task<bool> ModifierOrdinateur(int id, Ordinateur ordiMisAJour)
    {
        var ordiExistant = await GetById(id);
        if (ordiExistant == null) return false;
        ordiExistant.changerNom(ordiMisAJour.Nom);
        ordiExistant.changerEtat(ordiMisAJour.Etat);
        ordiExistant.Caracteristiques.Marque = ordiMisAJour.Caracteristiques.Marque;
        ordiExistant.Caracteristiques.RamGo = ordiMisAJour.Caracteristiques.RamGo;
        ordiExistant.Caracteristiques.Processeur = ordiMisAJour.Caracteristiques.Processeur;
        ordiExistant.Caracteristiques.StockageGo = ordiMisAJour.Caracteristiques.StockageGo;
        ordiExistant.Caracteristiques.TypeOrdi = ordiMisAJour.Caracteristiques.TypeOrdi;
        ordiExistant.Stock = ordiMisAJour.Stock;
        ordiExistant.Prix = ordiMisAJour.Prix;
        await _context.SaveChangesAsync();
        _journalService?.Ecrire($"{DateTime.Now:dd/MM/yyyy} - Ordinateur mis à jour : {ordiExistant.Nom}", "Ordinateurs");
        OrdinateurMisAJour?.Invoke(this, new OrdinateurMisAJourEventArgs(ordiExistant));
        return true;
    }

    public async Task<List<Ordinateur>> RechercherParMarque(string marque)
    {
        var marqueRecherchee = marque.Trim().ToLower();
        return await _context.Ordinateurs.Where(obj => obj.Caracteristiques.Marque.ToLower() == marqueRecherchee).ToListAsync();
    }

    public async Task<List<Ordinateur>> ListerOrdinateursDisponibles()
    {
        return await _context.Ordinateurs.Where(obj => obj.Stock > 0).ToListAsync();
    }

    public async Task<List<Ordinateur>> ListerParPrixDecroissant()
    {
        return await _context.Ordinateurs.OrderByDescending(ordinateur => ordinateur.Prix).ToListAsync();
    }
    
    public string Diagnostiquer(Ordinateur ordinateur)
    {
        return ordinateur switch
        {
            {
                Etat: EtatOrdinateur.HorsService
            }
                => "Remplacement",

            {	
                Etat: EtatOrdinateur.Lent,
                Caracteristiques:
                {
                    RamGo: < 8
                }
            }
                => "Ajouter RAM",

            _
                => "Aucune action"
        };
    }
    
    public async Task<(int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations)> CalculerStatistiques()
    {
        var total = await _context.Ordinateurs.CountAsync();
        var valeurStock = await _context.Ordinateurs.SumAsync(o => o.Stock * o.Prix);
        var disponibles = await _context.Ordinateurs.CountAsync(o => o.Stock > 0);        
        var reparations = await _context.Reparations.CountAsync();
        return (total, disponibles, valeurStock, reparations);
    }




    public event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    public event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;
    public event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;

    public void AfficherMessageOutput(string message)
    {
        Console.WriteLine(message);
    }
}
