using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabAPI.Controllers.v2
{
    [ApiController]
    [Route("v2/[controller]")]
    [Authorize] // Exige autenticação JWT em todos os endpoints
    public class AlunosController : ControllerBase
    {
        static List<Aluno> alunos = new()
        {
            new Aluno(1, "Ana", 20, "ana@gmail.com", DateTime.MinValue),
            new Aluno(2, "Bruno", 30, "bruno@gmail.com", DateTime.MinValue)
        };

        [HttpGet]
        public IActionResult GetAll() => Ok(alunos);

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
            => alunos.FirstOrDefault(a => a.Id == id) is Aluno aluno ? Ok(aluno) : NotFound();

        [HttpPost]
        public IActionResult Post(Aluno aluno)
        {
            aluno.Id = alunos.Any() ? alunos.Max(a => a.Id) + 1 : 1;
            alunos.Add(aluno);
            return CreatedAtAction(nameof(GetById), new { id = aluno.Id }, aluno);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, Aluno aluno)
        {
            var existente = alunos.FirstOrDefault(a => a.Id == id);
            if (existente == null) return NotFound();

            existente.Nome = aluno.Nome;
            existente.Idade = aluno.Idade;
            existente.Email = aluno.Email;
            existente.DataNascimento = aluno.DataNascimento;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var aluno = alunos.FirstOrDefault(a => a.Id == id);
            if (aluno == null) return NotFound();

            alunos.Remove(aluno);
            return NoContent();
        }
    }
}
