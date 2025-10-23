using System.Data;
using MySql.Data.MySqlClient; 

/// <summary>
/// Classe de repositório para gerenciar entidades Aluno no banco de dados.
/// </summary>
public class AlunoRepository : IRepository<Aluno>
{
    /// <summary>
    /// Obtém ou define a string de conexão com o banco de dados.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="AlunoRepository"/>.
    /// </summary>
    /// <param name="connectionString">A string de conexão com o banco de dados.</param>
    public AlunoRepository(string connectionString)
    {
        ConnectionString = connectionString;
    }

    /// <summary>
    /// Garante que o esquema do banco de dados para a tabela Aluno exista.
    /// </summary>
    public void GarantirEsquema()
    {
        using var conn = new MySqlConnection(ConnectionString);
        conn.Open();

        const string ddl = @"
        CREATE TABLE IF NOT EXISTS Alunos (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            Nome VARCHAR(100) NOT NULL,
            Idade INT NOT NULL,
            Email VARCHAR(100) NOT NULL,
            DataNascimento DATE NOT NULL
        )";
        
        using var cmd = new MySqlCommand(ddl, conn) { CommandType = CommandType.Text, CommandTimeout = 30 };
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Insere um novo registro de Aluno no banco de dados.
    /// </summary>
    /// <param name="nome">O nome do Aluno.</param>
    /// <param name="idade">A idade do Aluno.</param>
    /// <param name="email">O email do Aluno.</param>
    /// <param name="dataNascimento">A data de nascimento do Aluno.</param>
    /// <returns>O ID do Aluno recém-inserido.</returns>
    public int Inserir(string nome, int idade, string email, DateTime dataNascimento)
    {
        const string sql = @"
        INSERT INTO Alunos (Nome, Idade, Email, DataNascimento)
        VALUES (@Nome, @Idade, @Email, @DataNascimento);
        SELECT LAST_INSERT_ID();";


        using var conn = new MySqlConnection(ConnectionString);
        conn.Open();

        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Nome", nome);
        cmd.Parameters.AddWithValue("@Idade", idade);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@DataNascimento", dataNascimento);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    /// <summary>
    /// Recupera uma lista de todos os registros de Aluno do banco de dados.
    /// </summary>
    /// <returns>Uma lista de entidades Aluno.</returns>
    public List<Aluno> Listar()
    {
        var alunos = new List<Aluno>();
        const string sql = "SELECT Id, Nome, Idade, Email, DataNascimento FROM Alunos";

        using var conn = new MySqlConnection(ConnectionString);
        conn.Open();

        using var cmd = new MySqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) 
        {
            alunos.Add(new Aluno(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetString(3),
                reader.GetDateTime(4)
            ));
        }
        return alunos;
    }

    /// <summary>
    /// Atualiza os dados de um registro de Aluno no banco de dados.
    /// </summary>
    /// <param name="id">O ID do Aluno a ser atualizado.</param>
    /// <param name="nome">O novo nome do Aluno.</param>
    /// <param name="idade">A nova idade do Aluno.</param>
    /// <param name="email">O novo email do Aluno.</param>
    /// <param name="dataNascimento">A nova data de nascimento do Aluno.</param>
    /// <returns>O número de linhas afetadas.</returns>
    public int Atualizar(int id, string nome, int idade, string email, DateTime dataNascimento)
    {
        const string sql = @"
        UPDATE Alunos
        SET Nome = @Nome, Idade = @Idade, Email = @Email, DataNascimento = @DataNascimento
        WHERE Id = @Id";

        using var conn = new MySqlConnection(ConnectionString);
        conn.Open();

        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Nome", nome);
        cmd.Parameters.AddWithValue("@Idade", idade);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@DataNascimento", dataNascimento);

        return cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Exclui um registro de Aluno do banco de dados.
    /// </summary>
    /// <param name="id">O ID do Aluno a ser excluído.</param>
    /// <returns>O número de linhas afetadas.</returns>
    public int Excluir(int id)
    {
        const string sql = "DELETE FROM Alunos WHERE Id = @Id";
        using var conn = new MySqlConnection(ConnectionString);
        conn.Open();

        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);

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
        var alunos = new List<Aluno>();

        string sql = $"SELECT Id, Nome, Idade, Email, DataNascimento FROM Alunos WHERE {propriedade} = @Valor";
        using var conn = new MySqlConnection(ConnectionString);
        conn.Open();

        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Valor", valor);

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            alunos.Add(new Aluno
            (
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetString(3),
                reader.GetDateTime(4)
            ));
        }

        return alunos;
    }
}