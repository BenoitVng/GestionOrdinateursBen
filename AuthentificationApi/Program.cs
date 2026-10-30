
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("GestionOrdinateurs", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GestionOrdinateursApi:BaseUrl"]!);
});
builder.Services.AddHttpClient("Keycloak", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Keycloak:TokenUrl"]!);
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        //front lancé en local (dotnet run) ; 5000 : front servi par nginx sous Docker
        policy.WithOrigins("http://localhost:5103", "http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowBlazorClient");
app.MapControllers();

app.Run();
