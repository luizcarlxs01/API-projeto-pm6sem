namespace PM_6SEM_2025.DTO.Usuario
{
    /// <summary>
    /// DTO retornado após login bem-sucedido.
    /// Contém dados básicos do usuário e o token JWT.
    /// </summary>
    public class LoginResponseDTO
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// Perfil do usuário (Cidadao, Admin, etc).
        /// </summary>
        public string Perfil { get; set; }

        /// <summary>
        /// Token JWT para autenticação nas demais requisições.
        /// </summary>
        public string Token { get; set; }
    }
}
