using Music.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:8000");

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseDefaultFiles();
app.UseStaticFiles();

Musica[] listamusicas = new Musica[100];
int totalmusicas = 0;

// Cadastrar Musicas
app.MapPost("/CadastrarMusica", (JsonElement body) =>
{
    Random random = new();
    Musica musica = new Musica();

    musica.Id = random.Next(1000, 9999);

    if (body.TryGetProperty("titulo", out var pTitulo))
        musica.Titulo = pTitulo.GetString() ?? "";

    if (body.TryGetProperty("compositor", out var pCompositor))
        musica.Compositor = pCompositor.GetString() ?? "";

    if (body.TryGetProperty("genero", out var pGenero))
        musica.Genero = pGenero.GetString() ?? "";

    if (body.TryGetProperty("artista", out var pArtista))
        musica.Artista = pArtista.GetString() ?? "";

    if (body.TryGetProperty("ano", out var pAno) && pAno.ValueKind == JsonValueKind.Number)
        musica.Ano = (short)pAno.GetInt32();

    listamusicas[totalmusicas] = musica;
    totalmusicas++;

    return Results.Ok(new { musica });
});

// Listagem
app.MapGet("/Listar", () =>
{
    Musica[] musicascadastradas = new Musica[totalmusicas];

//Listagem
app.MapGet("/Listar", () =>
{
    Musica[] musicascadastradas = new Musica[totalmusicas];

    for(int i =0; i < totalmusicas; i++)
    {
        musicascadastradas[i] = listamusicas[i];
    }
     return Results.Ok(new{musicascadastradas});

});

//Busca
app.MapGet("/BuscarMusica", (string? artista, string? compositor, string? genero, int? ano) =>{

    var filtro = new System.Collections.Generic.List<Musica>();

    for (int i = 0; i < totalmusicas; i++)
    {
        var f = listamusicas[i];
        bool corresponde = true;

        if (!string.IsNullOrEmpty(artista) && !f.Artista.Contains(artista, StringComparison.OrdinalIgnoreCase))
        {
            corresponde = false;
        }
        if (!string.IsNullOrEmpty(compositor) && !f.Compositor.Contains(compositor, StringComparison.OrdinalIgnoreCase))
        {
            corresponde = false;
        }
        if (!string.IsNullOrEmpty(genero) && !f.Genero.Contains(genero, StringComparison.OrdinalIgnoreCase))
        {
            corresponde = false;
        }
        if (ano.HasValue && f.Ano != ano.Value)
        {
            corresponde = false;
        }
        if (corresponde)
        {
            filtro.Add(f);
        }
    }
    return Results.Ok(new {
        musica = filtro
    });
});

// Atualizar Musica
app.MapPatch("/AtualizarMusica/{id}/titulo", (int id, JsonElement body) =>
{
    Musica? musica = null;

    for (int i = 0; i < totalmusicas; i++)
    {
        if (listamusicas[i].Id == id)
        {
            musica = listamusicas[i];

            if (body.TryGetProperty("titulo", out var titulo))
            {
               musica.Titulo = titulo.GetString() ?? musica.Titulo;
            }
            listamusicas[i] = musica;
            break;
        }
    }
    if (musica == null)
    {
        return Results.NotFound(new 
        { 
            erro = "Música não encontrada."
        });
    }

    return Results.Ok(new 
    { 
        mensagem = "Título atualizado com sucesso.", musica 
    });
});

//Deletar Música
app.MapDelete("/DeletarMusica/{id}", (int id) =>
{
    int index = -1;

    for (int i = 0; i < totalmusicas; i++)
    {
        if (listamusicas[i].Id == id)
        {
            index = i;
            break;
        }
    }
    if (index == -1)
    {
        return Results.NotFound(new
        {
            erro = "Musica não encontrada."
        });
    }

    for (int i = index; i < totalmusicas - 1; i++)
    {
        listamusicas[i] = listamusicas[i + 1];
    }

    totalmusicas--;

    return Results.Ok(new
    {
        mensagem = "Musica removida com sucesso."
    });
});

app.Run();