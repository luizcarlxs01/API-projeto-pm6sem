using System.ComponentModel.DataAnnotations;

namespace PM_6SEM_2025.DTO.Usuario
{
    public class UsuarioCriacaoDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MaxLength(255)]
        public string SenhaHash { get; set; }

        [Phone(ErrorMessage = "Número de telefone inválido")]
        [MaxLength(20)]
        public string Telefone { get; set; }
    }
}
