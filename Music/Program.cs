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

// Definição de rotas HTTP do tipo POST 
app.MapPost("/CadastrarMusica", (JasonElement body ) => 
{
    Random random = new ();
    Musica musica = new Musica();

    Musica.Id = random.Next(1000, 9999);
    Musica.Titulo = body.GetProperty("titulo").GetString()?? "";
    Musica.Compositor = body.GetProperty("compositor").GetString()?? "";
    Musica.Genero = body.GetProperty("genero").GetString()?? "";
    Musica.Artista = body.GetProperty("artista").GetString()?? "";
    Musica.Ano = body.GetProperty("ano").GetInt16()?? "";

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
    var filtro = new System.Collections.Generic.List<Funcionario>();
    
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
app.Mappatch("/AtualizarMusica/{id}/titulo", (int id, String novoTitulo) =>

//Deletar Música
app.MapDelete("DeletarMusica/{titulo}", (String titulo)=>{
    var M = listamusicas.Find(musica => musica.id == id);
     
    if(M == null)
    {
        return Results.NotFound(new
        {
			erro = "Musica não encontrada."
        });
    }

    M.titulo = novoTitulo;

    return Results.Ok(new
    {
        mensagem = "Título atualizado com sucesso."
    });
});

//Deletar Música
app.MapDelete("/DeletarMusica/{id}", (int id)=>
{
    var M = listamusicas.Find(musica => musica.id == id);

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