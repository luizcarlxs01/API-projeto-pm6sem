using System.ComponentModel.DataAnnotations;

namespace PM_6SEM_2025.DTO.Denuncia
{
    public class CriarDenunciaDTO
    {
        public int? IdUsuario { get; set; }

        [Required(ErrorMessage = "O logradouro é obrigatório")]
        [MaxLength(200)]
        public string Logradouro { get; set; }

        [Required(ErrorMessage = "O número é obrigatório")]
        [MaxLength(10)]
        public string Numero { get; set; }

        [MaxLength(100)]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "O CEP é obrigatório")]
        [MaxLength(15)]
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

        [Required]
        public bool ExistemObstaculos { get; set; }

        public int? IdProblemaAcessibilidade { get; set; }

        [MaxLength(500)]
        public string? MotivosPedido { get; set; }

        [MaxLength(1000)]
        public string? Descricao { get; set; }

        // Compatível com o Model e com o banco
        public byte[]? ArquivoFoto { get; set; }
    }
}
