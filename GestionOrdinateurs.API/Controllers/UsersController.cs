
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ParcInformatiqueContext _context;

    public UsersController(ParcInformatiqueContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var utilisateurs = await _context.Users.ToListAsync();
        return Ok(utilisateurs);
    }

    [HttpGet("login/{login}")]
    public async Task<IActionResult> GetByLogin(string login)
    {
        var utilisateur = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (utilisateur == null) return NotFound("Utilisateur introuvable");
        return Ok(utilisateur);
    }
}
