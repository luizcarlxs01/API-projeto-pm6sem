using System.ComponentModel.DataAnnotations;

namespace PM_6SEM_2025.DTO.Denuncia
{
    public class AtualizarDenunciaDTO
    {
        [Required]
        public int IdDenuncia { get; set; }

        [MaxLength(50)]
        public string? ProtocoloPrefeitura { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }
    }
}
