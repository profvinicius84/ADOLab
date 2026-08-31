using System.Data;
using Microsoft.Data.SqlClient;

public class AlunoRepository : IRepository<Aluno>
{
    public string ConnectionString { get; set; }

    public AlunoRepository(string connectionString)
    {
        ConnectionString = connectionString;
    }

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

        using var cmd = new SqlCommand(ddl, conn);

        cmd.ExecuteNonQuery();
    }


    // CREATE - Inserir aluno
    public int Inserir(
        string nome,
        int idade,
        string email,
        DateTime dataNascimento)
    {
        string sql = @"
            INSERT INTO dbo.Alunos
            (Nome, Idade, Email, DataNascimento)
            OUTPUT INSERTED.Id
            VALUES
            (@Nome, @Idade, @Email, @DataNascimento)";

        using var conn = new SqlConnection(ConnectionString);

        conn.Open();

        using var cmd = new SqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@Nome", nome);
        cmd.Parameters.AddWithValue("@Idade", idade);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@DataNascimento", dataNascimento);

        int id = Convert.ToInt32(cmd.ExecuteScalar());

        return id;
    }


    // READ - Listar alunos
    public List<Aluno> Listar()
    {
        var alunos = new List<Aluno>();

        string sql = @"
            SELECT
                Id,
                Nome,
                Idade,
                Email,
                DataNascimento
            FROM dbo.Alunos
            ORDER BY Id";

        using var conn = new SqlConnection(ConnectionString);

        conn.Open();

        using var cmd = new SqlCommand(sql, conn);

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            var aluno = new Aluno(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetString(3),
                reader.GetDateTime(4)
            );

            alunos.Add(aluno);
        }

        return alunos;
    }


    // UPDATE - Atualizar aluno
    public int Atualizar(
        int id,
        string nome,
        int idade,
        string email,
        DateTime dataNascimento)
    {
        string sql = @"
            UPDATE dbo.Alunos
            SET
                Nome = @Nome,
                Idade = @Idade,
                Email = @Email,
                DataNascimento = @DataNascimento
            WHERE Id = @Id";

        using var conn = new SqlConnection(ConnectionString);

        conn.Open();

        using var cmd = new SqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Nome", nome);
        cmd.Parameters.AddWithValue("@Idade", idade);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@DataNascimento", dataNascimento);

        return cmd.ExecuteNonQuery();
    }


    // DELETE - Excluir aluno
    public int Excluir(int id)
    {
        string sql = @"
            DELETE FROM dbo.Alunos
            WHERE Id = @Id";

        using var conn = new SqlConnection(ConnectionString);

        conn.Open();

        using var cmd = new SqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@Id", id);

        return cmd.ExecuteNonQuery();
    }


    // Buscar aluno por uma propriedade
    public List<Aluno> Buscar(string propriedade, object valor)
    {
        var alunos = new List<Aluno>();

        // Verifica qual coluna o usuário quer pesquisar
        string coluna;

        switch (propriedade.ToLower())
        {
            case "id":
                coluna = "Id";
                break;

            case "nome":
                coluna = "Nome";
                break;

            case "idade":
                coluna = "Idade";
                break;

            case "email":
                coluna = "Email";
                break;

            case "datanascimento":
                coluna = "DataNascimento";
                break;

            default:
                throw new ArgumentException("Propriedade inválida.");
        }

        string sql = $@"
            SELECT
                Id,
                Nome,
                Idade,
                Email,
                DataNascimento
            FROM dbo.Alunos
            WHERE {coluna} = @Valor
            ORDER BY Id";

        using var conn = new SqlConnection(ConnectionString);

        conn.Open();

        using var cmd = new SqlCommand(sql, conn);

        // Converte o valor de acordo com a coluna
        if (coluna == "Id" || coluna == "Idade")
        {
            cmd.Parameters.AddWithValue(
                "@Valor",
                Convert.ToInt32(valor)
            );
        }
        else if (coluna == "DataNascimento")
        {
            cmd.Parameters.AddWithValue(
                "@Valor",
                Convert.ToDateTime(valor)
            );
        }
        else
        {
            cmd.Parameters.AddWithValue(
                "@Valor",
                valor.ToString()
            );
        }

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            var aluno = new Aluno(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetString(3),
                reader.GetDateTime(4)
            );

            alunos.Add(aluno);
        }

        return alunos;
    }
}
