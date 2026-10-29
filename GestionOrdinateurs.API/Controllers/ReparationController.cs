using GestionOrdinateurs.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace GestionOrdinateurs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReparationController : ControllerBase
{
    private readonly IOrdinateurService _ordiService;
    private readonly ReparationService _reparationService;
    private readonly StockService _stockService;

    public ReparationController(IOrdinateurService ordiService, ReparationService reparService, StockService stockService)
    {
        _ordiService = ordiService;
        _reparationService = reparService;
        _stockService = stockService;
    }

    [HttpPost("SimulerReparation")]
    [Authorize]
    public async Task<IActionResult> SimulerReparation()
    {
        var ordinateurs = await _ordiService.ListerParc();
        if (ordinateurs.Count < 5) return BadRequest("Il faut au moins 5 ordinateurs.");
        await Task.WhenAll(ordinateurs.Take(5).Select(o => _reparationService.SimulerReparationAsync(o))); // j'utilise Task.WhenAll pour lancer les réparations en parallèle
        return Ok("Les cinq réparations sont terminées.");
    }
    /*[HttpGet("ListerReparations")]
    public async Task<IActionResult> ListerReparations()
    {
        var reparations = await _reparationService.ListerReparationsAsync();
        return Ok(reparations);
    }*/
        
    [HttpGet("/api/reparations/historique")]
    [Authorize]
    public async Task<IActionResult> GetHistorique()
    {
        var historique = await _reparationService.ListerHistoriqueReparationsAsync();
        return Ok(historique);
    }

    [HttpGet("GetStockSSD")]
    [Authorize]
    public IActionResult GetStockSSD()
    {
        Parallel.For(0, 10, i =>
        {
            _stockService.ConsommerSSD();
        });

        int stockRestant = _stockService.StockSSD;

        return Ok(stockRestant);
    }
}