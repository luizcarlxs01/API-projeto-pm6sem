using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PM_6SEM_2025.Data;

namespace ProblemasAcessibilidade.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProblemasAcessibilidadeController : ControllerBase
    {
        private readonly PmContext _context;

        public ProblemasAcessibilidadeController(PmContext context)
        {
            _context = context;
        }

        // GET: api/ProblemasAcessibilidade/ListarProblemas
        [HttpGet("ListarProblemas")]
        public async Task<IActionResult> ListarProblemas()
        {
            try
            {
                var problemas = await _context.ProblemasAcessibilidade
                    .Select(p => new
                    {
                        idProblemaAcessibilidade = p.IdProblemaAcessibilidade,
                        descricao = p.Descricao
                    })
                    .ToListAsync();

                return Ok(new
                {
                    dados = problemas,
                    mensagem = "Lista de problemas de acessibilidade carregada com sucesso!",
                    status = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    dados = (object?)null,
                    mensagem = "Erro ao carregar problemas de acessibilidade.",
                    status = false,
                    erroDetalhe = ex.Message
                });
            }
        }
    }
}
