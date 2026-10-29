using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.Services;

public record HistoriqueReparationRecord(
    int OrdinateurId,
    string NomOrdinateur,
    string Technicien,
    string TypeReparation,
    DateTime DateIntervention
);

public class ReparationService
{
    private readonly ParcInformatiqueContext _context;
    private readonly JournalService _journalService;
    private object _lock = new object();

    public ReparationService(ParcInformatiqueContext context, JournalService journalService)
    {
        _context = context;
        _journalService = journalService;
        
    }

    public async Task<Reparation> CreerReparation(Ordinateur ordinateur)
    {
        var test = new Reparation
        {
            UnOrdinateur = ordinateur,
            TypeReparation = "Réparation processeur",
            DateDebut = DateTime.Now,
            Statut = "En cours"
        };

        lock(_lock)
        {
            _context.Reparations.Add(test);
        }
        //Méthode de création d'une réparation en base. (à créér par vous)
        await Task.Delay(2000);
        
        lock(_lock)
        {
            test.Statut = "Terminée";
            _context.SaveChanges();
        }
        Console.WriteLine(
            $"{ordinateur.Nom} réparé");
        _journalService.Ecrire($"{DateTime.Now:dd/MM/yyyy} - {ordinateur.Nom} réparé", "Reparations");

        return test;
    }

    public async Task SimulerReparationAsync(Ordinateur ordinateur)
    {
        var reparation = await CreerReparation(ordinateur);

        await Task.Delay(2000);
        reparation.Statut = "Terminée";
        await _context.SaveChangesAsync();

        Console.WriteLine($"{ordinateur.Nom} réparé");
    }
    /*public async Task<List<Reparation>> ListerReparationsAsync()
    {
        return await _context.Reparations
            .Include(reparation => reparation.UnOrdinateur)
            .ToListAsync();
    }*/
    public async Task<List<HistoriqueReparationRecord>> ListerHistoriqueReparationsAsync()
    {
        return await _context.Reparations.Include(reparation => reparation.UnOrdinateur)
        .Select(reparation => new HistoriqueReparationRecord(
                reparation.UnOrdinateur.Id,
                reparation.UnOrdinateur.Nom,
                reparation.Technicien,
                reparation.TypeReparation,
                reparation.DateDebut)).ToListAsync();
    }
}