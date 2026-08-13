namespace ADOLab;

/// <summary>
/// Verificacao rapida do CRUD contra o banco real: dotnet run -- smoke
/// </summary>
static class SmokeTest
{
    public static void Run(string connString)
    {
        var repo = new AlunoRepository(connString);
        repo.GarantirEsquema();

        int id = repo.Inserir("Teste Ponytail", 20, "teste@ponytail.dev", new DateTime(2005, 3, 14));
        Ok(id > 0);

        var inserido = repo.Listar().Single(a => a.Id == id);
        Ok(inserido.Nome == "Teste Ponytail");
        Ok(inserido.Idade == 20);
        Ok(inserido.DataNascimento == new DateTime(2005, 3, 14));

        Ok(repo.Atualizar(id, "Teste Editado", 21, "novo@ponytail.dev", new DateTime(2004, 1, 2)) == 1);
        var editado = repo.Listar().Single(a => a.Id == id);
        Ok(editado.Nome == "Teste Editado" && editado.Idade == 21);

        Ok(repo.Buscar("Nome", "editado").Any(a => a.Id == id));   // case-insensitive
        Ok(repo.Buscar("Email", "novo@").Any(a => a.Id == id));
        Ok(repo.Buscar("Id", id).Count == 1);
        Ok(repo.Buscar("Nome", "nao-existe-xyz").Count == 0);

        try
        {
            repo.Buscar("Nome; DROP TABLE Alunos", "x");
            throw new Exception("FALHOU: coluna invalida deveria ter sido rejeitada.");
        }
        catch (ArgumentException) { /* esperado */ }

        Ok(repo.Excluir(id) == 1);
        Ok(repo.Excluir(id) == 0);
        Ok(repo.Listar().All(a => a.Id != id));

        Console.WriteLine("Smoke test OK: Inserir, Listar, Atualizar, Excluir, Buscar.");
    }

    static void Ok(bool condicao, [System.Runtime.CompilerServices.CallerLineNumber] int linha = 0)
    {
        if (!condicao) throw new Exception($"Smoke test FALHOU na linha {linha}.");
    }
}
