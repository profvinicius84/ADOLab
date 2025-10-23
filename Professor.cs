public class Professor
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Email { get; set; }

    // Navigation
    public List<Disciplina> Disciplinas { get; set; } = new();
}
