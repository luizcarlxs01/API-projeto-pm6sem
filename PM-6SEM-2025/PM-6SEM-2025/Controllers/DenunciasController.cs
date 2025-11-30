using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PM_6SEM_2025.DTO.Denuncia;
using PM_6SEM_2025.Models;
using PM_6SEM_2025.Services.Denuncias;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace PM_6SEM_2025.Controllers
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento de denúncias de acessibilidade.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Todas as ações exigem usuário autenticado por padrão
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
        [AllowAnonymous]
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
        /// Cria uma nova denúncia.
        /// Pode ser usada por usuário logado ou de forma anônima.
        /// Recebe JSON no corpo da requisição.
        /// </summary>
        [HttpPost("CriarDenuncia")]
        [AllowAnonymous] // continua permitindo anônimo
        public async Task<ActionResult<ResponseModel<List<DenunciasModel>>>> CriarDenuncia(
            [FromBody] CriarDenunciaDTO denunciaDTO)
        {
            // Se o usuário estiver autenticado, forçamos o IdUsuario a vir do token
            if (User?.Identity?.IsAuthenticated == true)
            {
                // Tenta pegar o Id do usuário pelas claims
                var idClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (int.TryParse(idClaim, out var idUsuarioToken))
                {
                    denunciaDTO.IdUsuario = idUsuarioToken;
                }
            }
            else
            {
                // Se não estiver autenticado, tratamos denúncia anônima
                if (denunciaDTO.IdUsuario == 0)
                {
                    denunciaDTO.IdUsuario = null;
                }
            }

            var resposta = await _denunciasService.CriarDenuncia(denunciaDTO);
            return Ok(resposta);
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
