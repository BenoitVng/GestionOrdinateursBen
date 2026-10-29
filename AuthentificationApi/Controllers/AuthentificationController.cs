
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using GestionOrdinateurs.Shared;

namespace AuthentificationApi.Controllers;

public class LoginRequest
{
    public string Login { get; set; } = string.Empty;
}

[ApiController]
[Route("auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthenticationController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest demande)
    {
        if (string.IsNullOrWhiteSpace(demande.Login))
        {
            return BadRequest("Login obligatoire");
        }
        var client = _httpClientFactory.CreateClient("GestionOrdinateurs");
        using var response = await client.GetAsync($"api/Users/login/{Uri.EscapeDataString(demande.Login.Trim())}");
        if (response.StatusCode == HttpStatusCode.NotFound) {
            return NotFound("Utilisateur n'existe pas");
        }
        if (!response.IsSuccessStatusCode) {
            return StatusCode(502, "Erreur de communication avec l'API");
        }
        var user = await response.Content.ReadFromJsonAsync<User>();
        if (user == null) {
            return NotFound("Utilisateur n'existe pas");
        }
        var jwtKey = _configuration["Jwt:Key"] ?? "";
        var key = Encoding.UTF8.GetBytes(jwtKey);
        if (key.Length < 32) {
            return StatusCode(500, "La clé JWT doit contenir au moins 32 octets");
        }
        var jwt = GenTokenJWT(key, user.Role, user.Login);
        return Ok(new { Token = jwt });
    }

    private string GenTokenJWT(byte[] keyP, RoleEnum role, string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userId),
            new Claim(ClaimTypes.Role, role.ToString())
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyP), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);
        return jwt;
    }
}
