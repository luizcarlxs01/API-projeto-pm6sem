using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM_6SEM_2025.Models
{
    [Table("ProblemasAcessibilidade")]
    public class ProblemasAcessibilidadeModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdProblemaAcessibilidade { get; set; }

        [Required(ErrorMessage = "A descrição do problema é obrigatória")]
        [MaxLength(150)]
        public string Descricao { get; set; }
    }
}
