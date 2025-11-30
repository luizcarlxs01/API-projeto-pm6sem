using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM_6SEM_2025.Models
{
    /// <summary>
    /// Representa uma denúncia de acessibilidade urbana.
    /// </summary>
    public class DenunciasModel
    {
        public int IdDenuncia { get; set; }

        /// <summary>
        /// FK opcional para o usuário que fez a denúncia.
        /// Pode ser null em caso de denúncia anônima.
        /// </summary>
        public int? IdUsuario { get; set; }

        /// <summary>
        /// Usuário relacionado à denúncia.
        /// Mapeado explicitamente para usar IdUsuario como chave estrangeira.
        /// </summary>
        [ForeignKey(nameof(IdUsuario))]
        [InverseProperty(nameof(UsuariosModel.Denuncias))]
        public UsuariosModel? Usuario { get; set; }

        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string? Complemento { get; set; }
        public string CEP { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string? PontoReferencia { get; set; }

        public bool ExistemObstaculos { get; set; }

        public int? IdProblemaAcessibilidade { get; set; }
        public ProblemasAcessibilidadeModel? ProblemaAcessibilidade { get; set; }

        public string? MotivosPedido { get; set; }
        public string? Descricao { get; set; }

        /// <summary>
        /// Conteúdo da foto armazenado como texto (base64).
        /// No banco, a coluna deve ser NVARCHAR(MAX).
        /// </summary>
        public string? ArquivoFoto { get; set; }

        public string? ProtocoloPrefeitura { get; set; }
        public string Status { get; set; }
        public DateTime DataDenuncia { get; set; }
    }
}
