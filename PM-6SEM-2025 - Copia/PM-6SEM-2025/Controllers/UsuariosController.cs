using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PM_6SEM_2025.DTO.Usuario;
using PM_6SEM_2025.Models;
using PM_6SEM_2025.Services.Usuarios;

namespace PM_6SEM_2025.Controllers
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento de usuários do sistema.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Por padrão, tudo aqui exige autenticação
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosService _usuarioInterface;

        /// <summary>
        /// Construtor com injeção de dependência do serviço de usuários.
        /// </summary>
        public UsuariosController(IUsuariosService usuarioInterface)
        {
            _usuarioInterface = usuarioInterface;
        }

        /// <summary>
        /// Lista todos os usuários cadastrados.
        /// </summary>
        [HttpGet("ListarUsuarios")]
        public async Task<ActionResult<ResponseModel<List<UsuariosModel>>>> ListaUsuarios()
        {
            var usuarios = await _usuarioInterface.ListarUsuarios();
            return Ok(usuarios);
        }

        /// <summary>
        /// Busca um usuário pelo seu identificador.
        /// </summary>
        [HttpGet("BuscarUsuarioPorId/{idUsuario}")]
        public async Task<ActionResult<ResponseModel<UsuariosModel>>> BuscarUsuarioPorId(int idUsuario)
        {
            var usuario = await _usuarioInterface.BuscarUsuarioPorId(idUsuario);
            return Ok(usuario);
        }

        /// <summary>
        /// Busca o usuário associado a uma denúncia específica.
        /// </summary>
        [HttpGet("BuscarUsuarioPorIdDenuncia/{idDenuncia}")]
        public async Task<ActionResult<ResponseModel<UsuariosModel>>> BuscarUsuarioPorIdDenuncia(int idDenuncia)
        {
            var usuario = await _usuarioInterface.BuscarUsuarioPorIdDenuncia(idDenuncia);
            return Ok(usuario);
        }

        /// <summary>
        /// Cria um novo usuário no sistema.
        /// ROTA PÚBLICA.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("CriarUsuario")]
        public async Task<ActionResult<ResponseModel<List<UsuariosModel>>>> CriarUsuario(UsuarioCriacaoDTO usuarioCriacaoDTO)
        {
            var usuarios = await _usuarioInterface.CriarUsuario(usuarioCriacaoDTO);
            return Ok(usuarios);
        }

        /// <summary>
        /// Edita os dados de um usuário existente.
        /// </summary>
        [HttpPut("EditarUsuario")]
        public async Task<ActionResult<ResponseModel<List<UsuariosModel>>>> EditarUsuario(EditarUsuarioDTO editarUsuarioDTO)
        {
            var usuarios = await _usuarioInterface.EditarUsuario(editarUsuarioDTO);
            return Ok(usuarios);
        }

        /// <summary>
        /// Exclui um usuário pelo seu identificador.
        /// </summary>
        [HttpDelete("ExcluirUsuario")]
        public async Task<ActionResult<ResponseModel<List<UsuariosModel>>>> ExcluirUsuario(int IdUsuario)
        {
            var usuarios = await _usuarioInterface.ExcluirUsuario(IdUsuario);
            return Ok(usuarios);
        }

        /// <summary>
        /// Realiza o login de um usuário e gera o token JWT.
        /// ROTA PÚBLICA.
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
    }
}
