using System.ComponentModel.DataAnnotations;

namespace PM_6SEM_2025.DTO.Usuario
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string SenhaHash { get; set; }
    }
}
