using PM_6SEM_2025.Models;

namespace PM_6SEM_2025.Services.Auth
{
    /// <summary>
    /// Serviço responsável pela geração de tokens JWT.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Gera um token JWT para o usuário informado.
        /// </summary>
        /// <param name="usuario">Usuário autenticado.</param>
        /// <returns>String com o token JWT.</returns>
        string GerarToken(UsuariosModel usuario);
    }
}
