using Music.Models;
using System.Text.Json;
var builder = WebApplication.CreateBuilder(args);

// Cria um objeto responsável por configurar a aplicação ASP.NET
var builder = WebApplication.CreateBuilder(args);

// Define o endereço e a porta em que a aplicação irá escutar requisições HTTP
builder.WebHost.UseUrls("http://localhost:8000");


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// Constrói a aplicação web a partir das configurações definidas no builder
var app = builder.Build();

app.UseCors("AllowAll");

Musica[] listamusicas = new Musicas[100];
int totalmusicas = 0;

// Definição de rotas HTTP do tipo GET 
app.MapPost("/Musica", (JasonElement body ) => 
{
    Random random = new();
    Musica[] musica = new 

    
});

// Inicia o servidor web é iniciado e passa a aguardar requisições HTTP dos clientes
app.Run();

