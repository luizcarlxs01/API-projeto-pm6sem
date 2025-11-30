using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM_6SEM_2025.Models
{
    [Table("Denuncias")]
    public class DenunciasModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdDenuncia { get; set; }

        // FK opcional (denúncia anônima = null)
        public int? IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public UsuariosModel Usuario { get; set; }

        // Endereço completo
        [Required(ErrorMessage = "O logradouro é obrigatório")]
        [MaxLength(150)]
        public string Logradouro { get; set; }

        [Required(ErrorMessage = "O número é obrigatório")]
        [MaxLength(10)]
        public string Numero { get; set; }

        [MaxLength(50)]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "O CEP é obrigatório")]
        [MaxLength(10)]
        public string CEP { get; set; }

        [Required(ErrorMessage = "O bairro é obrigatório")]
        [MaxLength(100)]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "A cidade é obrigatória")]
        [MaxLength(100)]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "O estado é obrigatório")]
        [MaxLength(2)]
        public string Estado { get; set; }

        [MaxLength(200)]
        public string? PontoReferencia { get; set; }

        // Acessibilidade
        [Required]
        public bool ExistemObstaculos { get; set; }

        // FK opcional (somente se ExistemObstaculos = true)
        public int? IdProblemaAcessibilidade { get; set; }

        [ForeignKey("IdProblemaAcessibilidade")]
        public ProblemasAcessibilidadeModel ProblemaAcessibilidade { get; set; }

        // Para quando ExistemObstaculos = false (checkbox múltipla)
        [MaxLength(200)]
        public string? MotivosPedido { get; set; } // armazenar como texto separado por vírgulas, ex: "Pisos quebrados, Buracos"

        [MaxLength(500)]
        public string? Descricao { get; set; }

        public byte[]? ArquivoFoto { get; set; }

        [MaxLength(50)]
        public string? ProtocoloPrefeitura { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pendente"; // valores possíveis: Pendente, Em análise, Em andamento, Resolvida, Indeferida

        [Required]
        public DateTime DataDenuncia { get; set; } = DateTime.Now;
    }
}
