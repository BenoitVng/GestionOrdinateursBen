
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using GestionOrdinateurs.Shared;
using System.Text.Json.Serialization;
namespace AuthentificationApi.Controllers;
// TP6 : identifiants envoyés par la page de login pour Keycloak
public class KeycloakLoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// TP6 : réponse du endpoint token de Keycloak
public class KeycloakTokenResponse
{
    [JsonPropertyName("access_token")] public string AccessToken { get; set; } = string.Empty;
    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
    [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; } = string.Empty;
}

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
        // TP6 : login via Keycloak (grant_type=password). L'ancien endpoint "login" est conservé.
    [HttpPost("login-keycloak")]
    public async Task<IActionResult> LoginKeycloak([FromBody] KeycloakLoginRequest demande)
    {
        if (string.IsNullOrWhiteSpace(demande.Username) || string.IsNullOrWhiteSpace(demande.Password))
        {
            return BadRequest("Login et mot de passe obligatoires");
        }
        var client = _httpClientFactory.CreateClient("Keycloak");
        // Paramètres attendus par Keycloak (formulaire x-www-form-urlencoded)
        var formulaire = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _configuration["Keycloak:ClientId"] ?? "",
            ["client_secret"] = _configuration["Keycloak:ClientSecret"] ?? "",
            ["username"] = demande.Username.Trim(),
            ["password"] = demande.Password
        });
        using var response = await client.PostAsync(_configuration["Keycloak:TokenUrl"], formulaire);
        // Keycloak renvoie 400 (invalid_grant) ou 401 pour de mauvais identifiants
        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.BadRequest)

        {
            return Unauthorized("Login ou mot de passe incorrect"); // mauvais identifiants
        }
        if (!response.IsSuccessStatusCode)
        {
            // On renvoie le message de Keycloak pour faciliter le débogage (secret, client, compte incomplet...)
            return StatusCode(502, $"Erreur Keycloak : {await response.Content.ReadAsStringAsync()}");
        }
        var tokenKeycloak = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();
        if (tokenKeycloak == null || string.IsNullOrEmpty(tokenKeycloak.AccessToken))
        {
            return StatusCode(502, "Réponse Keycloak invalide");
        }
        // "Token" : même nom qu'avant, le front n'a pas à changer sa lecture de la réponse
        return Ok(new { Token = tokenKeycloak.AccessToken, tokenKeycloak.ExpiresIn, tokenKeycloak.RefreshToken });
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
