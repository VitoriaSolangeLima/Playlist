using Music.Models;
using System.Text.Json;

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

Musica[] listamusicas = new Musica[100];
int totalmusicas = 0;

// Cadastrar Musicas
app.MapPost("/CadastrarMusica", (JsonElement body ) => 
{
    Random random = new ();
    Musica musica = new Musica();

    musica.id = random.Next(1000, 9999);
    musica.Titulo = body.GetProperty("titulo").GetString()?? "";
    musica.Compositor = body.GetProperty("compositor").GetString()?? "";
    musica.Genero = body.GetProperty("genero").GetString()?? "";
    musica.Artista = body.GetProperty("artista").GetString()?? "";
    musica.Ano = body.GetProperty("ano").GetInt16();

    listamusicas[totalmusicas] = musica;
    totalmusicas++;

    return Results.Ok(new{musica});

});


//Listagem
app.MapGet("/Listar", () => 
{
    Musica[] musicascadastradas = new Musica[totalmusicas];

    for(int i =0; i < totalmusicas; i++){
        musicascadastradas[i] = listamusicas[i];
    }
     return Results.Ok(new{musicascadastradas});

});


//Busca 
app.MapGet("/BuscarMusica", (string? artista, string? compositor, string? genero, int? ano) =>{
    var filtro = new System.Collections.Generic.List<Musica>();
    
    for (int i = 0; i < totalmusicas; i++){
        var f = listamusicas[i];
        bool corresponde = true;

        if (!string.IsNullOrEmpty(artista) && !f.Artista.Contains(artista, StringComparison.OrdinalIgnoreCase)){
            corresponde = false;
        }

        if (!string.IsNullOrEmpty(compositor) && !f.Compositor.Contains(compositor, StringComparison.OrdinalIgnoreCase)){
            corresponde = false;
        }

        if (!string.IsNullOrEmpty(genero) && !f.Genero.Contains(genero, StringComparison.OrdinalIgnoreCase)){
            corresponde = false;
        }

        if (ano.HasValue && f.Ano != ano.Value){
            corresponde = false;
        }

        if (corresponde){
            filtro.Add(f);
        }
    }
    
    return Results.Ok(new {
        musica = filtro 
    });
});

//Atualizar Musica
app.MapPatch("/AtualizarMusica/{id}/titulo", (int id, JsonElement body) =>
{
    Musica? musica = null;

    //Procurar musica pelo id
    for(int i = 0; i < totalmusicas; i++)
    {
        if(listamusicas[i].id == id)
        {
            musica = listamusicas[i];

            if (body.TryGetProperty("titulo", out var titulo))
            {
               musica.Titulo = titulo.GetString();
            }
               listamusicas[i] = musica;
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

    for(int i = 0; i < totalmusicas; i++){
        if(listamusicas[i].id == id)
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

    // Remover
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

// Inicia o servidor web é iniciado e passa a aguardar requisições HTTP dos clientes
app.Run();