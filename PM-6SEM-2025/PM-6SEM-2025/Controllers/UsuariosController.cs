using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PM_6SEM_2025.DTO.Usuario;
using PM_6SEM_2025.Models;
using PM_6SEM_2025.Services.Usuarios;
using System.Security.Claims;

namespace PM_6SEM_2025.Controllers
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento de usuários do sistema.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Por padrão, todas as rotas exigem autenticação
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosService _usuarioInterface;

        public UsuariosController(IUsuariosService usuarioInterface)
        {
            _usuarioInterface = usuarioInterface;
        }

        /// <summary>
        /// Lista todos os usuários cadastrados (somente admin).
        /// </summary>
        [HttpGet("ListarUsuarios")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseModel<List<UsuariosModel>>>> ListaUsuarios()
        {
            var usuarios = await _usuarioInterface.ListarUsuarios();
            return Ok(usuarios);
        }

        /// <summary>
        /// Busca um usuário pelo seu identificador (somente admin).
        /// </summary>
        [HttpGet("BuscarUsuarioPorId/{idUsuario}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseModel<UsuariosModel>>> BuscarUsuarioPorId(int idUsuario)
        {
            var usuario = await _usuarioInterface.BuscarUsuarioPorId(idUsuario);
            return Ok(usuario);
        }

        /// <summary>
        /// Busca o usuário associado a uma denúncia específica (somente admin).
        /// </summary>
        [HttpGet("BuscarUsuarioPorIdDenuncia/{idDenuncia}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseModel<UsuariosModel>>> BuscarUsuarioPorIdDenuncia(int idDenuncia)
        {
            var usuario = await _usuarioInterface.BuscarUsuarioPorIdDenuncia(idDenuncia);
            return Ok(usuario);
        }

        /// <summary>
        /// Cria um novo usuário no sistema (rota pública).
        /// </summary>
        [AllowAnonymous]
        [HttpPost("CriarUsuario")]
        public async Task<ActionResult<ResponseModel<List<UsuariosModel>>>> CriarUsuario(UsuarioCriacaoDTO usuarioCriacaoDTO)
        {
            var usuarios = await _usuarioInterface.CriarUsuario(usuarioCriacaoDTO);
            return Ok(usuarios);
        }

        /// <summary>
        /// Edita os dados de um usuário existente (somente admin).
        /// </summary>
        [HttpPut("EditarUsuario")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseModel<List<UsuariosModel>>>> EditarUsuario(EditarUsuarioDTO editarUsuarioDTO)
        {
            var usuarios = await _usuarioInterface.EditarUsuario(editarUsuarioDTO);
            return Ok(usuarios);
        }

        /// <summary>
        /// Exclui um usuário pelo seu identificador (somente admin).
        /// </summary>
        [HttpDelete("ExcluirUsuario")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseModel<List<UsuariosModel>>>> ExcluirUsuario(int IdUsuario)
        {
            var usuarios = await _usuarioInterface.ExcluirUsuario(IdUsuario);
            return Ok(usuarios);
        }

        /// <summary>
        /// Realiza o login de um usuário e gera o token JWT (rota pública).
        /// </summary>
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<ResponseModel<LoginResponseDTO>>> Login(
            [FromBody] LoginRequestDTO loginDTO)
        {
            var resposta = await _usuarioInterface.Login(loginDTO);

            if (!resposta.Status)
                return BadRequest(resposta);

            return Ok(resposta);
        }

        /// <summary>
        /// Retorna os dados do usuário autenticado (Me).
        /// </summary>
        [HttpGet("Me")]
        public async Task<ActionResult<ResponseModel<UsuariosModel>>> Me()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(idClaim) || !int.TryParse(idClaim, out var idUsuario))
            {
                return Unauthorized(new
                {
                    mensagem = "Usuário não identificado no token."
                });
            }

            var resposta = await _usuarioInterface.ObterUsuarioPorId(idUsuario);
            return Ok(resposta);
        }

        /// <summary>
        /// Altera a senha do usuário autenticado.
        /// </summary>
        [HttpPost("AlterarSenha")]
        public async Task<ActionResult<ResponseModel<bool>>> AlterarSenha([FromBody] AlterarSenhaDTO dto)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(idClaim) || !int.TryParse(idClaim, out var idUsuario))
            {
                return Unauthorized(new
                {
                    mensagem = "Usuário não identificado no token."
                });
            }

            var resposta = await _usuarioInterface.AlterarSenha(idUsuario, dto);

            if (!resposta.Status)
                return BadRequest(resposta);

            return Ok(resposta);
        }

        /// <summary>
        /// Torna o usuário informado em Administrador.
        /// Apenas usuários com perfil Admin podem usar este endpoint.
        /// </summary>
        [HttpPut("TornarAdmin/{idUsuario:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseModel<UsuariosModel>>> TornarAdmin(int idUsuario)
        {
            var resposta = await _usuarioInterface.TornarAdmin(idUsuario);
            if (!resposta.Status)
            {
                return BadRequest(resposta);
            }

            return Ok(resposta);
        }

    }
}
