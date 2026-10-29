using Microsoft.AspNetCore.Mvc;
using GestionOrdinateurs.Shared;
using GestionOrdinateurs.Services;
using Microsoft.AspNetCore.Authorization;
namespace GestionOrdinateurs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdinateursController : ControllerBase
{
    private readonly IOrdinateurService _service;
    private readonly ILogger<OrdinateursController> _Logger;
    private readonly IConfiguration _configuration;

    // reçoit les services utiles
    public OrdinateursController(IOrdinateurService service, ILogger<OrdinateursController> logger, IConfiguration configuration)
    {   
        _service = service;
        _Logger = logger;
        _configuration = configuration;
    }

    // liste les ordinateurs
    [HttpGet]
    [Authorize]
    public async Task<IEnumerable<Ordinateur>> Get()
    {
        _Logger.LogInformation("Récupération de la liste des ordinateurs.");
        return await _service.ListerParc();
    }

    // cherche un ordinateur
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetComputerById(int id)
    {
        _Logger.LogInformation($"Récupération de l'ordinateur avec l'ID {id}");
        var ordinateur = await _service.GetById(id);

        if (ordinateur == null)
        {
            _Logger.LogWarning($"Aucun ordinateur trouvé avec l'ID {id}");
            return NotFound($"Aucun ordinateur trouvé avec l'ID {id}");
        }
        return Ok(ordinateur);
    }

    // ajoute un ordinateur
    [HttpPost]
    [Authorize(Roles = "Admin")]

    public async Task<IActionResult> Post([FromBody] Ordinateur ordinateur)
    {
        bool ajoute = await _service.AjouterOrdinateur(ordinateur);
        if (!ajoute)
        {
            _Logger.LogWarning($"Ajout impossible : un ordinateur avec l'ID {ordinateur.Id} existe déjà.");
            return Conflict($"Un ordinateur avec l'ID {ordinateur.Id} existe déjà.");
        }
        _Logger.LogInformation($"Ajout de l'ordinateur avec l'ID {ordinateur.Id}");
        return CreatedAtAction(
            nameof(GetComputerById),
            new { id = ordinateur.Id },
            ordinateur
        );
    }

    // modifie un ordinateur
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]

    public async Task<IActionResult> Put(int id, [FromBody] Ordinateur ordinateur)
    {
        bool modifier = await _service.ModifierOrdinateur(id, ordinateur);
        if (!modifier)
        {
            _Logger.LogWarning($"Modification impossible : ID {id} introuvable.");
            return NotFound($"Impossible de modifier : ID {id} introuvable.");
        }
        _Logger.LogInformation($"Modification de l'ordinateur avec l'ID {id}");
        return Ok();
    }

    // supprime un ordinateur
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]

    public async Task<IActionResult> Delete(int id)
    {
        bool supprime = await _service.SupprimerOrdinateur(id);
        if (!supprime)
        {
            _Logger.LogWarning($"Suppression impossible : ID {id} introuvable.");
            return NotFound($"Impossible de supprimer : ID {id} introuvable.");
        }
        _Logger.LogInformation($"Suppression de l'ordinateur avec l'ID {id}");
        return Ok();
    }
    // cherche par marque
    [HttpGet("marque/{marque}")]
    [Authorize]
    public async Task<IActionResult> GetByMarque(string marque)
    {
        var ordinateurs = await _service.RechercherParMarque(marque);
        if (ordinateurs.Count == 0)
        {
            _Logger.LogWarning($"Aucun ordinateur trouvé pour la marque {marque}");
            return NotFound($"aucun ordi trouvé pour la marque {marque}.");
        }
        _Logger.LogInformation($"Récupération des ordinateurs de la marque {marque}");
        return Ok(ordinateurs);
    }
    // affiche les infos de l'application
    [HttpGet("info")]
    [Authorize]
    public IActionResult retournerInfo() {
        var appName = _configuration["Application:Name"];
        var appVersion = _configuration["Application:Version"];
        var infoApp = new { Name = appName, Version = appVersion};
        _Logger.LogInformation($"Récupération des informations de l'application : {appName} v{appVersion}");
        return Ok(infoApp);
    }
    
    [HttpGet("disponibles")]
    [Authorize]
    public async Task<IActionResult> ListerOrdinateursDisponibles()
    {
        var ordinateurs = await _service.ListerOrdinateursDisponibles();
        return Ok(ordinateurs);
    }
    
  
    [HttpGet("prix-decroissant")]
    [Authorize]
    public async Task<IActionResult> GetParPrixDecroissant()
    {
        var ordinateurs = await _service.ListerParPrixDecroissant();

        return Ok(ordinateurs);
    }
    
    [HttpGet("{id}/diagnostic")]
    [Authorize]
    public async Task<IActionResult> Diagnostiquer(int id)
    {
        var ordinateur = await _service.GetById(id);
        if (ordinateur == null) return NotFound("Ordinateur introuvable");
        var diagnostic = _service.Diagnostiquer(ordinateur);
        return Ok(diagnostic);
    }
    
    [HttpGet("statistiques")]
    [Authorize]
    public async Task<IActionResult> GetStatistiques()
    {
        var (total, enStock, valeurStock, reparations) = await _service.CalculerStatistiques();
        return Ok(new { total, enStock, valeurStock, reparations });
    }
    
    [HttpGet("inventaire/nom/{nom}")]
    [Authorize]
    public async Task<IActionResult> Chercher(string nom)
    {
        var inventaire = new InventaireOrdinateurs();
        inventaire.Ordinateurs = (await _service.ListerParc()).ToList();
        var ordinateur = inventaire[nom];
        if (ordinateur == null) {
            return NotFound("Ordinateur introuvable");
        }
        return Ok(ordinateur);
    }

}