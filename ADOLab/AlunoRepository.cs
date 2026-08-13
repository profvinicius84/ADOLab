using Npgsql;

/// <summary>
/// Classe de reposit�rio para gerenciar entidades Aluno no banco de dados.
/// </summary>
public class AlunoRepository : IRepository<Aluno>
{
    /// <summary>
    /// Obt�m ou define a string de conex�o com o banco de dados.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Inicializa uma nova inst�ncia da classe <see cref="AlunoRepository"/>.
    /// </summary>
    /// <param name="connectionString">A string de conex�o com o banco de dados.</param>
    public AlunoRepository(string connectionString)
    {
        ConnectionString = connectionString;
    }

    /// <summary>
    /// Garante que o esquema do banco de dados para a tabela Aluno exista.
    /// </summary>
    public void GarantirEsquema()
    {
        const string ddl = @"
        CREATE TABLE IF NOT EXISTS Alunos (
            Id SERIAL PRIMARY KEY,
            Nome VARCHAR(100) NOT NULL,
            Idade INT NOT NULL,
            Email VARCHAR(100) NOT NULL,
            DataNascimento DATE NOT NULL
        );";
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand(ddl, conn) { CommandTimeout = 30 };
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Insere um novo registro de Aluno no banco de dados.
    /// </summary>
    /// <param name="nome">O nome do Aluno.</param>
    /// <param name="idade">A idade do Aluno.</param>
    /// <param name="email">O email do Aluno.</param>
    /// <param name="dataNascimento">A data de nascimento do Aluno.</param>
    /// <returns>O ID do Aluno rec�m-inserido.</returns>
    public int Inserir(string nome, int idade, string email, DateTime dataNascimento)
    {
        const string sql = @"
        INSERT INTO Alunos (Nome, Idade, Email, DataNascimento)
        VALUES (@nome, @idade, @email, @dataNascimento)
        RETURNING Id;";

        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("nome", nome);
        cmd.Parameters.AddWithValue("idade", idade);
        cmd.Parameters.AddWithValue("email", email);
        cmd.Parameters.AddWithValue("dataNascimento", dataNascimento.Date);
        return (int)cmd.ExecuteScalar()!;
    }

    /// <summary>
    /// Recupera uma lista de todos os registros de Aluno do banco de dados.
    /// </summary>
    /// <returns>Uma lista de entidades Aluno.</returns>
    public List<Aluno> Listar()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            "SELECT Id, Nome, Idade, Email, DataNascimento FROM Alunos ORDER BY Id", conn);
        return Ler(cmd);
    }

    /// <summary>
    /// Atualiza os dados de um registro de Aluno no banco de dados.
    /// </summary>
    /// <param name="id">O ID do Aluno a ser atualizado.</param>
    /// <param name="nome">O novo nome do Aluno.</param>
    /// <param name="idade">A nova idade do Aluno.</param>
    /// <param name="email">O novo email do Aluno.</param>
    /// <param name="dataNascimento">A nova data de nascimento do Aluno.</param>
    /// <returns>O n�mero de linhas afetadas.</returns>
    public int Atualizar(int id, string nome, int idade, string email, DateTime dataNascimento)
    {
        const string sql = @"
        UPDATE Alunos
        SET Nome = @nome, Idade = @idade, Email = @email, DataNascimento = @dataNascimento
        WHERE Id = @id;";

        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("nome", nome);
        cmd.Parameters.AddWithValue("idade", idade);
        cmd.Parameters.AddWithValue("email", email);
        cmd.Parameters.AddWithValue("dataNascimento", dataNascimento.Date);
        return cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Exclui um registro de Aluno do banco de dados.
    /// </summary>
    /// <param name="id">O ID do Aluno a ser exclu�do.</param>
    /// <returns>O n�mero de linhas afetadas.</returns>
    public int Excluir(int id)
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand("DELETE FROM Alunos WHERE Id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        return cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Busca registros de Aluno no banco de dados com base em um termo e valor.
    /// </summary>
    /// <param name="propriedade">A propriedade a ser pesquisada (coluna).</param>
    /// <param name="valor">O valor a ser pesquisado.</param>
    /// <returns>Uma lista de entidades Aluno correspondentes.</returns>
    public List<Aluno> Buscar(string propriedade, object valor)
    {
        // Nome de coluna nao pode ser parametrizado: valida contra a lista permitida.
        var coluna = Colunas.FirstOrDefault(c => c.Equals(propriedade, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"Propriedade invalida: '{propriedade}'. Use: {string.Join(", ", Colunas)}.", nameof(propriedade));

        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            $"SELECT Id, Nome, Idade, Email, DataNascimento FROM Alunos WHERE CAST({coluna} AS TEXT) ILIKE @valor ORDER BY Id", conn);
        cmd.Parameters.AddWithValue("valor", $"%{valor}%");
        return Ler(cmd);
    }

    private static readonly string[] Colunas = ["Id", "Nome", "Idade", "Email", "DataNascimento"];

    private static List<Aluno> Ler(NpgsqlCommand cmd)
    {
        var alunos = new List<Aluno>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            alunos.Add(new Aluno(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetString(3),
                reader.GetDateTime(4)));
        return alunos;
    }
}
