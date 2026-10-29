
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.Services;

public class OrdinateurServiceTest
{
    [Fact]
    public async Task AjouterOrdinateur_AjouteBienUnOrdinateur()
    {
        using var contexte = CreerContexte();

        var service = new OrdinateurService(contexte, new JournalService());
        var ordinateur = new Ordinateur("Ordi Test", EtatOrdinateur.EnService, new Caracteristiques("Dell", 16, "Ryzen 5", 512, TypeOrdinateur.Portable));
        ordinateur.Stock = 5;
        ordinateur.Prix = 899.99m;
        bool resultat = await service.AjouterOrdinateur(ordinateur);
        Assert.True(resultat);
        Assert.NotNull(await service.GetById(ordinateur.Id));
    }

    [Fact]
    public async Task SupprimerOrdinateur_Existant_RetourneTrue()
    {
        using var contexte = CreerContexte();
        var service = new OrdinateurService(contexte, new JournalService());
        var ordinateur = new Ordinateur("Ordi Test", EtatOrdinateur.EnService, new Caracteristiques("Dell", 16, "Ryzen 5", 512, TypeOrdinateur.Portable));
        ordinateur.Stock = 1;
        ordinateur.Prix = 500m;
        Assert.True(await service.AjouterOrdinateur(ordinateur));
        bool resultat = await service.SupprimerOrdinateur(ordinateur.Id);
        Assert.True(resultat);
        Assert.Null(await service.GetById(ordinateur.Id));
    }

    [Fact]
    public async Task SupprimerOrdinateur_Inexistant_RetourneFalse()
    {
        using var contexte = CreerContexte();
        var service = new OrdinateurService(contexte, new JournalService());
        bool resultat = await service.SupprimerOrdinateur(777);
        Assert.False(resultat);
    }

    [Fact]
    public async Task CreerReparation_AjouteBienUneReparation()
    {
        using var contexte = CreerContexte();
        var ordinateur = new Ordinateur("Ordi Test", EtatOrdinateur.EnService, new Caracteristiques("Dell", 16, "Intel i7", 512, TypeOrdinateur.Portable));
        contexte.Ordinateurs.Add(ordinateur);
        await contexte.SaveChangesAsync();
        var service = new ReparationService(contexte, new JournalService());
        var reparation = await service.CreerReparation(ordinateur);
        Assert.NotNull(reparation);
        Assert.Equal("Terminée", reparation.Statut);
        Assert.Equal(ordinateur.Id, reparation.UnOrdinateur.Id);
        Assert.Single(await contexte.Reparations.ToListAsync());
    }

    private ParcInformatiqueContext CreerContexte()
    {
        var options = new DbContextOptionsBuilder<ParcInformatiqueContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new ParcInformatiqueContext(options);
    }
}
