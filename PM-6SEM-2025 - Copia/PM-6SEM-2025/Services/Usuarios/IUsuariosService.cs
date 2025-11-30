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
        Task<ResponseModel<LoginResponseDTO>> Login(LoginRequestDTO loginDTO);

    }
}
