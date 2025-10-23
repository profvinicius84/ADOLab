using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Aluno> Alunos { get; set; } = null!;
    public DbSet<Professor> Professores { get; set; } = null!;
    public DbSet<Disciplina> Disciplinas { get; set; } = null!;
    public DbSet<Matricula> Matriculas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Professor>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Nome).IsRequired().HasMaxLength(100);
            b.Property(p => p.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<Disciplina>(b =>
        {
            b.HasKey(d => d.Id);
            b.Property(d => d.Nome).IsRequired().HasMaxLength(100);
            b.HasOne(d => d.Professor)
             .WithMany(p => p.Disciplinas)
             .HasForeignKey(d => d.ProfessorId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Matricula>(b =>
        {
            b.HasKey(m => m.Id);
            b.HasOne(m => m.Aluno)
             .WithMany(a => a.Matriculas)
             .HasForeignKey(m => m.AlunoId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(m => m.Disciplina)
             .WithMany(d => d.Matriculas)
             .HasForeignKey(m => m.DisciplinaId)
             .OnDelete(DeleteBehavior.Cascade);

            b.Property(m => m.DataMatricula).IsRequired();
        });
    }
}
