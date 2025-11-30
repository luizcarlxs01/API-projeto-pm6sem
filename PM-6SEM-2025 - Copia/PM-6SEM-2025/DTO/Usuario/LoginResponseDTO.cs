namespace PM_6SEM_2025.DTO.Usuario
{
    public class LoginResponseDTO
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }

        public string Token { get; set; }
    }
}
