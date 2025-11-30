using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PM_6SEM_2025.Data;
using PM_6SEM_2025.DTO.Usuario;
using PM_6SEM_2025.Models;
using PM_6SEM_2025.Services.Auth;

namespace PM_6SEM_2025.Services.Usuarios
{
    /// <summary>
    /// Serviço responsável pelas regras de negócio relacionadas a usuários.
    /// </summary>
    public class UsuariosService : IUsuariosService
    {
        private readonly PmContext _context;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<UsuariosModel> _passwordHasher;

        public UsuariosService(PmContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = new PasswordHasher<UsuariosModel>();
        }

        public async Task<ResponseModel<List<UsuariosModel>>> ListarUsuarios()
        {
            var resposta = new ResponseModel<List<UsuariosModel>>();

            try
            {
                resposta.Dados = await _context.Usuarios.AsNoTracking().ToListAsync();
                resposta.Mensagem = "Lista de usuários carregada com sucesso.";
                resposta.Status = true;
            }
            catch (Exception ex)
            {
                resposta.Status = false;
                resposta.Mensagem = $"Erro ao listar usuários: {ex.Message}";
            }

            return resposta;
        }

        public async Task<ResponseModel<UsuariosModel>> BuscarUsuarioPorId(int IdUsuario)
        {
            var resposta = new ResponseModel<UsuariosModel>();

            try
            {
                var usuario = await _context.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.IdUsuario == IdUsuario);

                if (usuario == null)
                {
                    resposta.Status = false;
                    resposta.Mensagem = "Usuário não encontrado.";
                    return resposta;
                }

                resposta.Dados = usuario;
                resposta.Status = true;
                resposta.Mensagem = "Usuário encontrado com sucesso.";
            }
            catch (Exception ex)
            {
                resposta.Status = false;
                resposta.Mensagem = $"Erro ao buscar usuário: {ex.Message}";
            }

            return resposta;
        }

        public async Task<ResponseModel<UsuariosModel>> BuscarUsuarioPorIdDenuncia(int IdDenuncia)
        {
            var resposta = new ResponseModel<UsuariosModel>();

            try
            {
                var denuncia = await _context.Denuncias
                    .Include(d => d.Usuario)
                    .FirstOrDefaultAsync(d => d.IdDenuncia == IdDenuncia);

                if (denuncia == null || denuncia.Usuario == null)
                {
                    resposta.Status = false;
                    resposta.Mensagem = "Denúncia ou usuário associado não encontrado.";
                    return resposta;
                }

                resposta.Dados = denuncia.Usuario;
                resposta.Status = true;
                resposta.Mensagem = "Usuário encontrado com sucesso.";
            }
            catch (Exception ex)
            {
                resposta.Status = false;
                resposta.Mensagem = $"Erro ao buscar usuário pela denúncia: {ex.Message}";
            }

            return resposta;
        }

        public async Task<ResponseModel<List<UsuariosModel>>> CriarUsuario(UsuarioCriacaoDTO usuarioCriacaoDTO)
        {
            var resposta = new ResponseModel<List<UsuariosModel>>();

            try
            {
                // Verifica se já existe usuário com o mesmo e-mail
                var jaExiste = await _context.Usuarios
                    .AnyAsync(u => u.Email == usuarioCriacaoDTO.Email);

                if (jaExiste)
                {
                    resposta.Status = false;
                    resposta.Mensagem = "Já existe um usuário cadastrado com este e-mail.";
                    return resposta;
                }

                var usuario = new UsuariosModel
                {
                    Nome = usuarioCriacaoDTO.Nome,
                    Email = usuarioCriacaoDTO.Email,
                    Telefone = usuarioCriacaoDTO.Telefone,
                    Perfil = "Cidadao", // Cadastro via front sempre entra como cidadão
                    DataCadastro = DateTime.Now
                };

                // Hash da senha
                usuario.SenhaHash = _passwordHasher.HashPassword(usuario, usuarioCriacaoDTO.SenhaHash);

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                resposta.Dados = await _context.Usuarios.AsNoTracking().ToListAsync();
                resposta.Status = true;
                resposta.Mensagem = "Usuário criado com sucesso (senha protegida com hash).";
            }
            catch (Exception ex)
            {
                resposta.Status = false;
                resposta.Mensagem = $"Erro ao criar usuário: {ex.Message}";
            }

            return resposta;
        }

        public async Task<ResponseModel<List<UsuariosModel>>> EditarUsuario(EditarUsuarioDTO editarUsuarioDTO)
        {
            var resposta = new ResponseModel<List<UsuariosModel>>();

            try
            {
                var usuario = await _context.Usuarios.FindAsync(editarUsuarioDTO.IdUsuario);

                if (usuario == null)
                {
                    resposta.Status = false;
                    resposta.Mensagem = "Usuário não encontrado.";
                    return resposta;
                }

                usuario.Nome = editarUsuarioDTO.Nome;
                usuario.Email = editarUsuarioDTO.Email;
                usuario.Telefone = editarUsuarioDTO.Telefone;

                // Se no futuro quiser permitir editar Perfil, pode ser feito aqui (apenas admin).

                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                resposta.Dados = await _context.Usuarios.AsNoTracking().ToListAsync();
                resposta.Status = true;
                resposta.Mensagem = "Usuário atualizado com sucesso.";
            }
            catch (Exception ex)
            {
                resposta.Status = false;
                resposta.Mensagem = $"Erro ao editar usuário: {ex.Message}";
            }

            return resposta;
        }

        public async Task<ResponseModel<List<UsuariosModel>>> ExcluirUsuario(int IdUsuario)
        {
            var resposta = new ResponseModel<List<UsuariosModel>>();

            try
            {
                var usuario = await _context.Usuarios.FindAsync(IdUsuario);

                if (usuario == null)
                {
                    resposta.Status = false;
                    resposta.Mensagem = "Usuário não encontrado.";
                    return resposta;
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                resposta.Dados = await _context.Usuarios.AsNoTracking().ToListAsync();
                resposta.Status = true;
                resposta.Mensagem = "Usuário excluído com sucesso.";
            }
            catch (Exception ex)
            {
                resposta.Status = false;
                resposta.Mensagem = $"Erro ao excluir usuário: {ex.Message}";
            }

            return resposta;
        }

        public async Task<ResponseModel<LoginResponseDTO>> Login(LoginRequestDTO loginDTO)
        {
            var resposta = new ResponseModel<LoginResponseDTO>();

            try
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == loginDTO.Email);

                if (usuario == null)
                {
                    resposta.Status = false;
                    resposta.Mensagem = "Usuário ou senha inválidos.";
                    return resposta;
                }

                var verificationResult = _passwordHasher.VerifyHashedPassword(
                    usuario,
                    usuario.SenhaHash,
                    loginDTO.SenhaHash
                );

                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    resposta.Status = false;
                    resposta.Mensagem = "Usuário ou senha inválidos.";
                    return resposta;
                }

                var token = _tokenService.GerarToken(usuario);

                resposta.Dados = new LoginResponseDTO
                {
                    IdUsuario = usuario.IdUsuario,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Perfil = usuario.Perfil,
                    Token = token
                };

                resposta.Status = true;
                resposta.Mensagem = "Login realizado com sucesso.";
            }
            catch (Exception ex)
            {
                resposta.Status = false;
                resposta.Mensagem = $"Erro ao realizar login: {ex.Message}";
            }

            return resposta;
        }

        public async Task<ResponseModel<UsuariosModel>> ObterUsuarioPorId(int idUsuario)
        {
            return await BuscarUsuarioPorId(idUsuario);
        }

        public async Task<ResponseModel<bool>> AlterarSenha(int idUsuario, AlterarSenhaDTO dto)
        {
            var resposta = new ResponseModel<bool>();

            try
            {
                var usuario = await _context.Usuarios.FindAsync(idUsuario);

                if (usuario == null)
                {
                    resposta.Status = false;
                    resposta.Mensagem = "Usuário não encontrado.";
                    return resposta;
                }

                var verificationResult = _passwordHasher.VerifyHashedPassword(
                    usuario,
                    usuario.SenhaHash,
                    dto.SenhaAtual
                );

                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    resposta.Status = false;
                    resposta.Mensagem = "Senha atual incorreta.";
                    return resposta;
                }

                usuario.SenhaHash = _passwordHasher.HashPassword(usuario, dto.NovaSenha);

                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                resposta.Dados = true;
                resposta.Status = true;
                resposta.Mensagem = "Senha alterada com sucesso.";
            }
            catch (Exception ex)
            {
                resposta.Status = false;
                resposta.Mensagem = $"Erro ao alterar senha: {ex.Message}";
            }

            return resposta;
        }

        /// <summary>
        /// Torna um usuário Administrador (Perfil = "Admin").
        /// Somente deve ser utilizado por usuários com perfil Admin.
        /// </summary>
        public async Task<ResponseModel<UsuariosModel>> TornarAdmin(int idUsuario)
        {
            var resposta = new ResponseModel<UsuariosModel>();

            try
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

                if (usuario == null)
                {
                    resposta.Status = false;
                    resposta.Mensagem = "Usuário não encontrado.";
                    return resposta;
                }

                // Se sua coluna se chama "Perfil", assumimos algo como: "Cidadao" / "Admin"
                if (usuario.Perfil == "Admin")
                {
                    resposta.Status = true;
                    resposta.Mensagem = "Usuário já é administrador.";
                    resposta.Dados = usuario;
                    return resposta;
                }

                usuario.Perfil = "Admin";

                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                resposta.Status = true;
                resposta.Mensagem = "Usuário promovido para administrador com sucesso.";
                resposta.Dados = usuario;

                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Status = false;
                resposta.Mensagem = $"Erro ao tornar usuário Admin: {ex.Message}";
                return resposta;
            }
        }

    }
}
