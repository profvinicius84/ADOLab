# ADOLab

Projeto acadêmico desenvolvido para praticar **acesso a dados com ADO.NET**, utilizando **SQL Server**.

A solução é composta por uma biblioteca de domínio, uma aplicação Console e uma aplicação Web MVC, utilizando o padrão Repository para realizar as operações de acesso ao banco de dados.

## Desafio

O objetivo da atividade é implementar os métodos CRUD da classe `AlunoRepository`.

Foram implementadas as seguintes operações:

* Inserir alunos;
* Listar alunos;
* Atualizar alunos;
* Excluir alunos;
* Buscar alunos por propriedade e valor.

## CRUD

A classe `AlunoRepository` possui os seguintes métodos:

| Método              | Operação     | Descrição                                        |
| ------------------- | ------------ | ------------------------------------------------ |
| `Inserir()`         | CREATE       | Cadastra um novo aluno no banco de dados         |
| `Listar()`          | READ         | Retorna todos os alunos cadastrados              |
| `Atualizar()`       | UPDATE       | Atualiza os dados de um aluno                    |
| `Excluir()`         | DELETE       | Remove um aluno pelo ID                          |
| `Buscar()`          | READ         | Busca alunos por uma propriedade e valor         |
| `GarantirEsquema()` | Configuração | Cria a tabela `Alunos` caso ela ainda não exista |

## Entidade Aluno

Os alunos possuem os seguintes dados:

* `Id`
* `Nome`
* `Idade`
* `Email`
* `DataNascimento`

## Estrutura do Projeto

```text
ADOLab/
├── ADOLab/              # Domínio e Repository
├── ADOLab.Console/      # Aplicação Console
├── ADOLab.Web/          # Aplicação Web MVC
```

## Tecnologias

* .NET 8
* ASP.NET Core MVC (.NET 10)
* C#
* ADO.NET
* Microsoft.Data.SqlClient
* SQL Server
* HTML
* CSS

## Banco de Dados

O projeto utiliza o banco:

```text
AlunosDB
```

A tabela utilizada pelo projeto é:

```text
dbo.Alunos
```

Sua estrutura é:

```sql
CREATE TABLE dbo.Alunos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Idade INT NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    DataNascimento DATE NOT NULL
);
```

O método `GarantirEsquema()` também pode criar a tabela `Alunos` automaticamente caso ela ainda não exista.

> O banco de dados `AlunosDB` deve existir antes da aplicação tentar realizar a conexão.

## Configuração da Conexão

Configure a connection string `SqlServerConnection` no arquivo `appsettings.json` da aplicação que será executada.

Exemplo:

```json
{
  "ConnectionStrings": {
    "SqlServerConnection": "Server=SEU_SERVIDOR;Database=AlunosDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Altere `SEU_SERVIDOR` de acordo com a instância do SQL Server instalada no computador.

## Executando o Projeto

Primeiro, compile a solução:

```bash
dotnet build ADOLab.sln
```

### Aplicação Console

Para executar a aplicação Console:

```bash
dotnet run --project ADOLab.Console/ADOLab.Console.csproj
```

### Aplicação Web

Para executar a aplicação Web MVC:

```bash
dotnet run --project ADOLab.Web/ADOLab.Web.csproj
```

Também é possível executar pelo Visual Studio definindo `ADOLab.Console` ou `ADOLab.Web` como **Projeto de Inicialização**.

## Funcionalidades

A aplicação permite realizar o gerenciamento dos alunos através das operações:

```text
Cadastrar
   ↓
Listar
   ↓
Buscar
   ↓
Atualizar
   ↓
Excluir
```

Todas as operações são realizadas utilizando **ADO.NET** através das classes:

* `SqlConnection`
* `SqlCommand`
* `SqlDataReader`
* `SqlParameter`

Os comandos SQL utilizam parâmetros para o envio dos dados ao SQL Server.

## Status

* ✅ Métodos CRUD implementados
* ✅ Conexão com SQL Server configurada
* ✅ Banco `AlunosDB` configurado
* ✅ Aplicação Console disponível
* ✅ Aplicação Web MVC disponível
