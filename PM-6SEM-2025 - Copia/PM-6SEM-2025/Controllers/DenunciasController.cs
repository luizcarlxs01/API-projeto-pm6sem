using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PM_6SEM_2025.DTO.Denuncia;
using PM_6SEM_2025.Models;
using PM_6SEM_2025.Services.Denuncias;

namespace PM_6SEM_2025.Controllers
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento de denúncias de acessibilidade.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Todas as ações exigem usuário autenticado
    public class DenunciasController : ControllerBase
    {
        private readonly IDenunciasService _denunciasService;

        /// <summary>
        /// Construtor com injeção de dependência do serviço de denúncias.
        /// </summary>
        public DenunciasController(IDenunciasService denunciasService)
        {
            _denunciasService = denunciasService;
        }

        /// <summary>
        /// Lista todas as denúncias cadastradas.
        /// </summary>
        [HttpGet("ListarDenuncias")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseModel<List<DenunciasModel>>>> ListarDenuncias()
        {
            var resposta = await _denunciasService.ListarDenuncias();
            return Ok(resposta);
        }

        /// <summary>
        /// Busca uma denúncia específica pelo seu identificador.
        /// </summary>
        [HttpGet("BuscarDenunciaPorId/{idDenuncia}")]
        public async Task<ActionResult<ResponseModel<DenunciasModel>>> BuscarDenunciaPorId(int idDenuncia)
        {
            var resposta = await _denunciasService.BuscarDenunciaPorId(idDenuncia);
            return Ok(resposta);
        }

        /// <summary>
        /// Lista todas as denúncias de um determinado usuário.
        /// </summary>
        [HttpGet("BuscarDenunciasPorUsuario/{idUsuario}")]
        public async Task<ActionResult<ResponseModel<List<DenunciasModel>>>> BuscarDenunciasPorUsuario(int idUsuario)
        {
            var resposta = await _denunciasService.BuscarDenunciasPorUsuario(idUsuario);
            return Ok(resposta);
        }

        /// <summary>
        /// Cria uma nova denúncia, com suporte a upload de foto.
        /// </summary>
        [HttpPost("CriarDenuncia")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ResponseModel<List<DenunciasModel>>>> CriarDenuncia(
            [FromForm] CriarDenunciaDTO denunciaDTO,
            IFormFile? arquivoFoto)
        {
            try
            {
                byte[]? bytesDaFoto = null;

                if (arquivoFoto != null && arquivoFoto.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await arquivoFoto.CopyToAsync(ms);
                    bytesDaFoto = ms.ToArray();
                }

                denunciaDTO.ArquivoFoto = bytesDaFoto;

                var resposta = await _denunciasService.CriarDenuncia(denunciaDTO);
                return Ok(resposta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensagem = "Erro ao criar denúncia.",
                    erro = ex.Message
                });
            }
        }

        /// <summary>
        /// Atualiza as informações de uma denúncia.
        /// </summary>
        [HttpPut("AtualizarDenuncia")]
        public async Task<ActionResult<ResponseModel<List<DenunciasModel>>>> AtualizarDenuncia(
            [FromBody] AtualizarDenunciaDTO denunciaDTO)
        {
            var resposta = await _denunciasService.AtualizarDenuncia(denunciaDTO);
            return Ok(resposta);
        }

        /// <summary>
        /// Exclui uma denúncia pelo seu identificador.
        /// </summary>
        [HttpDelete("ExcluirDenuncia/{idDenuncia}")]
        public async Task<ActionResult<ResponseModel<List<DenunciasModel>>>> ExcluirDenuncia(int idDenuncia)
        {
            var resposta = await _denunciasService.ExcluirDenuncia(idDenuncia);
            return Ok(resposta);
        }
    }
}
