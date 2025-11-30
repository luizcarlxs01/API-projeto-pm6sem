using PM_6SEM_2025.DTO.Usuario;
using PM_6SEM_2025.Models;

namespace PM_6SEM_2025.Services.Usuarios
{
    public interface IUsuariosService
    {
        Task<ResponseModel<List<UsuariosModel>>> ListarUsuarios();
        Task<ResponseModel<UsuariosModel>> BuscarUsuarioPorId(int IdUsuario);
        Task<ResponseModel<UsuariosModel>> BuscarUsuarioPorIdDenuncia(int IdDenuncia);
        Task<ResponseModel<List<UsuariosModel>>> CriarUsuario(UsuarioCriacaoDTO usuarioCriacaoDTO);
        Task<ResponseModel<List<UsuariosModel>>> EditarUsuario(EditarUsuarioDTO editarUsuarioDTO);
        Task<ResponseModel<List<UsuariosModel>>> ExcluirUsuario(int IdUsuario);

        /// <summary>
        /// Realiza o login do usuário e retorna token + dados básicos.
        /// </summary>
        Task<ResponseModel<LoginResponseDTO>> Login(LoginRequestDTO loginDTO);

        /// <summary>
        /// Retorna os dados do usuário logado (a partir do ID).
        /// </summary>
        Task<ResponseModel<UsuariosModel>> ObterUsuarioPorId(int idUsuario);

        /// <summary>
        /// Altera a senha do usuário logado.
        /// </summary>
        Task<ResponseModel<bool>> AlterarSenha(int idUsuario, AlterarSenhaDTO dto);

        Task<ResponseModel<UsuariosModel>> TornarAdmin(int idUsuario);
    }
}
