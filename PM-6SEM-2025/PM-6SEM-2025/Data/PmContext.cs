using Microsoft.EntityFrameworkCore;
using PM_6SEM_2025.Models;

namespace PM_6SEM_2025.Data
{
    public class PmContext : DbContext
    {
        public PmContext(DbContextOptions<PmContext> options) : base(options)
        {
        }

        public DbSet<UsuariosModel> Usuarios { get; set; }
        public DbSet<DenunciasModel> Denuncias { get; set; }
        public DbSet<ProblemasAcessibilidadeModel> ProblemasAcessibilidade { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tabela Usuarios
            modelBuilder.Entity<UsuariosModel>().ToTable("Usuarios");
            modelBuilder.Entity<UsuariosModel>(entity =>
            {
                entity.HasKey(u => u.IdUsuario);
                entity.Property(u => u.Nome).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.SenhaHash).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Telefone).HasMaxLength(20);

                // DataCadastro como DateTime
                entity.Property(u => u.DataCadastro)
                      .HasColumnType("datetime2");

                entity.Property(u => u.Perfil)
                      .IsRequired()
                      .HasMaxLength(50);
            });

            // Tabela ProblemasAcessibilidade
            modelBuilder.Entity<ProblemasAcessibilidadeModel>().ToTable("ProblemasAcessibilidade");
            modelBuilder.Entity<ProblemasAcessibilidadeModel>(entity =>
            {
                entity.HasKey(p => p.IdProblemaAcessibilidade);
                entity.Property(p => p.Descricao).IsRequired().HasMaxLength(150);
            });

            // Tabela Denuncias
            modelBuilder.Entity<DenunciasModel>().ToTable("Denuncias");
            modelBuilder.Entity<DenunciasModel>(entity =>
            {
                entity.HasKey(d => d.IdDenuncia);

                entity.Property(d => d.Logradouro).IsRequired().HasMaxLength(200);
                entity.Property(d => d.Numero).HasMaxLength(10);
                entity.Property(d => d.Complemento).HasMaxLength(100);
                entity.Property(d => d.CEP).HasMaxLength(10);
                entity.Property(d => d.Bairro).HasMaxLength(100);
                entity.Property(d => d.Cidade).HasMaxLength(100);
                entity.Property(d => d.Estado).HasMaxLength(50);
                entity.Property(d => d.PontoReferencia).HasMaxLength(200);
                entity.Property(d => d.Descricao).HasMaxLength(1000);
                entity.Property(d => d.Status).HasMaxLength(50);
                entity.Property(d => d.ProtocoloPrefeitura).HasMaxLength(50);

                // Relacionamento opcional com ProblemasAcessibilidade
                entity.HasOne(d => d.ProblemaAcessibilidade)
                      .WithMany()
                      .HasForeignKey(d => d.IdProblemaAcessibilidade)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relacionamento opcional com Usuário
                entity.HasOne(d => d.Usuario)
                      .WithMany(u => u.Denuncias)
                      .HasForeignKey(d => d.IdUsuario)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
