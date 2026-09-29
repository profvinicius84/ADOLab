using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;

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
        IF OBJECT_ID('dbo.Alunos', 'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Alunos (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                Nome NVARCHAR(100) NOT NULL,
                Idade INT NOT NULL,
                Email NVARCHAR(100) NOT NULL,
                DataNascimento DATE NOT NULL
            );
        END";
        using var conn = new SqlConnection(ConnectionString);
        conn.Open();
        using var cmd = new SqlCommand(ddl, conn) { CommandType = CommandType.Text, CommandTimeout = 30 };
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
            INSERT INTO dbo.Alunos (Nome, Idade, Email, DataNascimento)
            OUTPUT INSERTED.Id
            VALUES (@nome, @idade, @email, @dataNascimento);";

        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(sql, conn);
        AdicionarParametrosAluno(cmd, nome, idade, email, dataNascimento);
        conn.Open();
        return (int)cmd.ExecuteScalar()!;
    }

    /// <summary>
    /// Recupera uma lista de todos os registros de Aluno do banco de dados.
    /// </summary>
    /// <returns>Uma lista de entidades Aluno.</returns>
    public List<Aluno> Listar()
    {
        const string sql = "SELECT Id, Nome, Idade, Email, DataNascimento FROM dbo.Alunos ORDER BY Id;";

        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(sql, conn);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        return LerAlunos(reader);
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
            UPDATE dbo.Alunos
            SET Nome = @nome, Idade = @idade, Email = @email, DataNascimento = @dataNascimento
            WHERE Id = @id;";

        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
        AdicionarParametrosAluno(cmd, nome, idade, email, dataNascimento);
        conn.Open();
        return cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Exclui um registro de Aluno do banco de dados.
    /// </summary>
    /// <param name="id">O ID do Aluno a ser exclu�do.</param>
    /// <returns>O n�mero de linhas afetadas.</returns>
    public int Excluir(int id)
    {
        const string sql = "DELETE FROM dbo.Alunos WHERE Id = @id;";

        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
        conn.Open();
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
        ArgumentNullException.ThrowIfNull(propriedade);
        ArgumentNullException.ThrowIfNull(valor);

        // Identificadores SQL não podem ser parametrizados: aceite somente colunas conhecidas.
        (string coluna, SqlDbType tipo, int tamanho, object valorConvertido) = propriedade.Trim().ToLowerInvariant() switch
        {
            "id" => ("Id", SqlDbType.Int, 0, (object)Convert.ToInt32(valor, CultureInfo.InvariantCulture)),
            "nome" => ("Nome", SqlDbType.NVarChar, 100, Convert.ToString(valor, CultureInfo.InvariantCulture)!),
            "idade" => ("Idade", SqlDbType.Int, 0, Convert.ToInt32(valor, CultureInfo.InvariantCulture)),
            "email" => ("Email", SqlDbType.NVarChar, 100, Convert.ToString(valor, CultureInfo.InvariantCulture)!),
            "datanascimento" => ("DataNascimento", SqlDbType.Date, 0, Convert.ToDateTime(valor, CultureInfo.InvariantCulture)),
            _ => throw new ArgumentException("Propriedade inválida. Use Id, Nome, Idade, Email ou DataNascimento.", nameof(propriedade))
        };

        var sql = $"SELECT Id, Nome, Idade, Email, DataNascimento FROM dbo.Alunos WHERE {coluna} = @valor ORDER BY Id;";
        using var conn = new SqlConnection(ConnectionString);
        using var cmd = new SqlCommand(sql, conn);
        var parametro = tamanho > 0
            ? cmd.Parameters.Add("@valor", tipo, tamanho)
            : cmd.Parameters.Add("@valor", tipo);
        parametro.Value = valorConvertido;
        conn.Open();
        using var reader = cmd.ExecuteReader();
        return LerAlunos(reader);
    }

    private static void AdicionarParametrosAluno(SqlCommand cmd, string nome, int idade, string email, DateTime dataNascimento)
    {
        cmd.Parameters.Add("@nome", SqlDbType.NVarChar, 100).Value = nome;
        cmd.Parameters.Add("@idade", SqlDbType.Int).Value = idade;
        cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = email;
        cmd.Parameters.Add("@dataNascimento", SqlDbType.Date).Value = dataNascimento;
    }

    private static List<Aluno> LerAlunos(SqlDataReader reader)
    {
        var alunos = new List<Aluno>();
        while (reader.Read())
        {
            alunos.Add(new Aluno(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetString(3),
                reader.GetDateTime(4)));
        }
        return alunos;
    }
}
