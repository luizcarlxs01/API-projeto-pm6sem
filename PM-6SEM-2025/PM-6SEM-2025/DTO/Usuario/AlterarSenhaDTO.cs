using System.ComponentModel.DataAnnotations;

namespace PM_6SEM_2025.DTO.Usuario
{
    /// <summary>
    /// DTO utilizado para alteração de senha pelo próprio usuário.
    /// </summary>
    public class AlterarSenhaDTO
    {
        [Required(ErrorMessage = "A senha atual é obrigatória")]
        public string SenhaAtual { get; set; }

        [Required(ErrorMessage = "A nova senha é obrigatória")]
        [MinLength(8, ErrorMessage = "A nova senha deve ter pelo menos 8 caracteres.")]
        public string NovaSenha { get; set; }
    }
}
