using Music.Models;
using System.Text.Json;

// Cria um objeto responsável por configurar a aplicação ASP.NET
var builder = WebApplication.CreateBuilder(args); //Estava duplicado

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

//Para usar .Find(), .Add() e .Remove() precisa ser uma lista dinamica .List<Musica>
List<Musica> listamusicas = new List<Musica>();//List<T> do C# foi feita justamente para lidar com coleções de tamanho variável.

//Cadastrar musicas (POST)
app.MapPost("/CadastrarMusica", (JsonElement body ) => //Tava JasonElement e o certo é JsonElement
{
    Random random = new ();
    Musica musica = new Musica();

    musica.Id = random.Next(1000, 9999);//Mudei o Musica. para musica.
    musica.Titulo = body.GetProperty("titulo").GetString()?? "";
    musica.Compositor = body.GetProperty("compositor").GetString()?? "";
    musica.Genero = body.GetProperty("genero").GetString()?? "";
    musica.Artista = body.GetProperty("artista").GetString()?? "";
    musica.Ano = body.GetProperty("ano").GetInt32(); 
    //Mudei o GetInt16 para GetInt32 por conta do tamanho que foi imposto na propriedade do ano no Musicas.cs

    listamusicas.Add(musica); //.Add aumenta a lista e adiciona o item ao final

    return Results.Ok(new{musica});

});


//Listagem (GET)
app.MapGet("/Listar", () => 
{    
    //Como a List<Musicas> guarda apenas as musicas salvas não precisa dos laços para não mostrar os null
    return Results.Ok(new { musicascadastradas = listamusicas }); 

});


//Busca com filtros (GET)
app.MapGet("/BuscarMusica", (string? artista, string? compositor, string? genero, int? ano) =>
{
    var filtro = new List<Musica>();
    
    foreach (var f in listamusicas){ //Laço de repetição para repetir as condiçoes para cada item e guardar na var f
        bool corresponde = true;

        //Se o item não for igual a busca pula
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
    
    return Results.Ok(new {musica = filtro});
});

//Atualizar Musica (PATCH)
app.MapPatch("/AtualizarMusica/{id}/titulo", (int id, string novoTitulo) =>
{
    var m = listamusicas.Find(musica => musica.Id == id);

    if (m == null)
    {
        return Results.NotFound(new
        {
            erro = "Música não encontrada."
        });
    }

    m.Titulo = novoTitulo;

    return Results.Ok(new
    {
        mensagem = "Título atualizado com sucesso.",
        musica = m
    });
});



//Deletar Música
app.MapDelete("/DeletarMusica/{id}", (int id)=>
{
    var M = listamusicas.Find(musica => musica.Id == id); // Estava musica.id

    if(M == null)
    {
        return Results.NotFound(new
        {
			erro = "Musica não encontrada."
        });
    }

    listamusicas.Remove(M);

    return Results.Ok(new
    {
		mensagem = "Musica removida com sucesso."
    });
});

// Inicia o servidor web é iniciado e passa a aguardar requisições HTTP dos clientes
app.Run();