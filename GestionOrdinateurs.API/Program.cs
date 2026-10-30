//question 9 TP4
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using GestionOrdinateurs.Shared;
using GestionOrdinateurs.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// ajoute les services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GestionOrdinateurs",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez votre token JWT"
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});


// crée le service à chaque requête
builder.Services.AddScoped<IOrdinateurService, OrdinateurService>();
builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<OrdinateurService>();
builder.Services.AddScoped<ReparationService>();
//j'ajoute le service journalService
builder.Services.AddScoped<JournalService>();
// ajoute l'accès à la base
builder.Services.AddDbContext<ParcInformatiqueContext>();

builder.Services.AddLogging();


var jwtKey = builder.Configuration["Jwt:Key"] ?? "";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // TP6 : validation des tokens émis par Keycloak (clés publiques récupérées via le réseau Docker)
    options.MetadataAddress = builder.Configuration["Keycloak:MetadataAddress"];
    options.RequireHttpsMetadata = false;
    options.MapInboundClaims = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Keycloak:ValidIssuer"],
        ValidateAudience = false,
        ValidateLifetime = true,
        NameClaimType = "preferred_username",
        RoleClaimType = "roles" // [Authorize(Roles = "Admin")] lit ce claim
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            Console.WriteLine($"Utilisateur utilisant un Token Valide --- {context.Principal?.Identity?.Name ?? "Anonymous"}");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Token Invalide : {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5103", "http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ParcInformatiqueContext>();
    
    db.Database.Migrate();

    if(!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Login="user1", Role=RoleEnum.Admin },
            new User { Login="user2", Role=RoleEnum.Gestionnaire },
            new User { Login="user3", Role=RoleEnum.Gestionnaire }
        );

        db.SaveChanges();
    }
}
// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowBlazorClient"); // ajout CORS pour GestionOrdinateurs.API
app.UseAuthorization();
app.MapControllers();

app.Run();
