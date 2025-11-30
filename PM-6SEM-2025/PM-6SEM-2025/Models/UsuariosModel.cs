using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM_6SEM_2025.Models
{
    /// <summary>
    /// Representa um usuário do sistema.
    /// </summary>
    public class UsuariosModel
    {
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// Senha armazenada como hash.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string SenhaHash { get; set; }

        [MaxLength(20)]
        public string? Telefone { get; set; }

        /// <summary>
        /// Data de cadastro do usuário.
        /// </summary>
        public DateTime DataCadastro { get; set; }

        /// <summary>
        /// Perfil do usuário (ex: "ADMIN", "Cidadao", "Gestor").
        /// Usado na geração do token JWT.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Perfil { get; set; }

        /// <summary>
        /// Navegação para as denúncias feitas por este usuário.
        /// </summary>
        [InverseProperty(nameof(DenunciasModel.Usuario))]
        public ICollection<DenunciasModel> Denuncias { get; set; } = new List<DenunciasModel>();
    }
}
