namespace Music.Models;
public class Musica
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty; //string.Empty tem a mesma função que o " " e evita que de erro caso a variavel fique vazia
    public string Compositor { get; set; } = string.Empty;
    public string Artista { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public int Ano { get; set; }
}