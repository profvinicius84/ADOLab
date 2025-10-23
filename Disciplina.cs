public class Disciplina
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;

    // FK
    public int ProfessorId { get; set; }
    public Professor? Professor { get; set; }

    // Navigation
    public List<Matricula> Matriculas { get; set; } = new();
}
