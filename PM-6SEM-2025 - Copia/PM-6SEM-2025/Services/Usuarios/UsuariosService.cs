using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PM_6SEM_2025.Data;
using PM_6SEM_2025.DTO.Usuario;
using PM_6SEM_2025.Models;
using System.Collections.Generic;
using PM_6SEM_2025.Services.Auth;


namespace PM_6SEM_2025.Services.Usuarios
{
    public class UsuariosService : IUsuariosService
    {
        private readonly PmContext _context;
        public UsuariosService(PmContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<UsuariosModel>> BuscarUsuarioPorId(int IdUsuario)
        {
            ResponseModel<UsuariosModel> resposta = new ResponseModel<UsuariosModel>();
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(usuarioBanco => usuarioBanco.IdUsuario == IdUsuario);
                if(usuario == null) 
                {
                    resposta.Mensagem = "Nenhum registro localizado!";
                    return resposta;
                }

                resposta.Dados = usuario;
                resposta.Mensagem = "Usuario localizado!";
                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                resposta.Status = false;
                return resposta;
            }
        }

        public async Task<ResponseModel<UsuariosModel>> BuscarUsuarioPorIdDenuncia(int IdDenuncia)
        {
            ResponseModel<UsuariosModel> resposta = new ResponseModel<UsuariosModel>();
            try
            {
                var denuncia = await _context.Denuncias
                    .Include(u => u.Usuario)
                    .FirstOrDefaultAsync(DenunciaBanco => DenunciaBanco.IdDenuncia == IdDenuncia);
                if(denuncia == null)
                {
                    resposta.Mensagem = "Nenhum registro localizado!";
                    return resposta;
                }
                if (denuncia.Usuario == null)
                {
                    resposta.Mensagem = "Denúncia localizada, mas sem usuário associado (denúncia anônima).";
                    resposta.Dados = null;
                    return resposta;
                }

                resposta.Dados = denuncia.Usuario;
                resposta.Mensagem = "Usuario localizado!";
                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                resposta.Status = false;
                return resposta;
            }
        }

        public async Task<ResponseModel<List<UsuariosModel>>> CriarUsuario(UsuarioCriacaoDTO usuarioCriacaoDTO)
        {
            ResponseModel<List<UsuariosModel>> resposta = new ResponseModel<List<UsuariosModel>>();
            try
            {
                // Instancia o hasher padrão do ASP.NET Identity
                var passwordHasher = new PasswordHasher<UsuariosModel>();

                var usuario = new UsuariosModel()
                {
                    Nome = usuarioCriacaoDTO.Nome,
                    Email = usuarioCriacaoDTO.Email,
                    Telefone = usuarioCriacaoDTO.Telefone
                };

                // Gera o hash da senha (de forma automática e segura)
                usuario.SenhaHash = passwordHasher.HashPassword(usuario, usuarioCriacaoDTO.SenhaHash);

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                resposta.Dados = await _context.Usuarios.ToListAsync();
                resposta.Mensagem = "Usuario criado com sucesso (senha protegida com hash)!";

                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                resposta.Status = false;
                return resposta;
            }

        }

        public async Task<ResponseModel<List<UsuariosModel>>> EditarUsuario(EditarUsuarioDTO editarUsuarioDTO)
        {
            ResponseModel<List<UsuariosModel>> resposta = new ResponseModel<List<UsuariosModel>>();
            try
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(usuarioBanco => usuarioBanco.IdUsuario == editarUsuarioDTO.IdUsuario);
                if (usuario == null)
                {
                    resposta.Mensagem = "Nenhum usuario localizado!";
                    return resposta;
                }
                if (!string.IsNullOrEmpty(editarUsuarioDTO.Email))
                    usuario.Email = editarUsuarioDTO.Email;

                usuario.Nome = editarUsuarioDTO.Nome;
                usuario.Email = editarUsuarioDTO.Email;
                usuario.Telefone = editarUsuarioDTO.Telefone;

                _context.Update(usuario); 
                await _context.SaveChangesAsync();

                resposta.Dados = await _context.Usuarios.ToListAsync();
                resposta.Mensagem = "Usuario editado com sucesso!";

                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                resposta.Status = false;
                return resposta;
            }
        }

        public async Task<ResponseModel<List<UsuariosModel>>> ExcluirUsuario(int IdUsuario)
        {
            ResponseModel<List<UsuariosModel>> resposta = new ResponseModel<List<UsuariosModel>>();
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(usuarioBanco => usuarioBanco.IdUsuario == IdUsuario);
                if (usuario == null)
                {
                    resposta.Mensagem = "Nenhum usuário localizado!";
                    return resposta;
                }

                _context.Remove(usuario);
                await _context.SaveChangesAsync();

                resposta.Dados = await _context.Usuarios.ToListAsync();
                resposta.Mensagem = "Usuario removido com sucesso!";

                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                resposta.Status = false;
                return resposta;
            }
        }

        public async Task<ResponseModel<List<UsuariosModel>>> ListarUsuarios()
        {
            ResponseModel<List<UsuariosModel>> resposta = new ResponseModel<List<UsuariosModel>>();
            try
            {
                var usuarios = await _context.Usuarios.ToListAsync();

                resposta.Dados = usuarios;
                resposta.Mensagem = "Todos os Usuarios foram coletados!";

                return resposta;
            }
            catch (Exception ex) 
            {
                resposta.Mensagem = ex.Message;
                resposta.Status = false;
                return resposta;
            }
        }

        public async Task<ResponseModel<LoginResponseDTO>> Login(LoginRequestDTO loginDTO)
        {
            var resposta = new ResponseModel<LoginResponseDTO>();

            // 1) Busca usuário pelo e-mail
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == loginDTO.Email);

            if (usuario == null)
            {
                resposta.Status = false;
                resposta.Mensagem = "Usuário ou senha inválidos.";
                return resposta;
            }

            // 2) Verifica a senha usando o MESMO PasswordHasher do cadastro
            var passwordHasher = new PasswordHasher<UsuariosModel>();

            var resultadoVerificacao = passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.SenhaHash,     // hash armazenado no banco
                loginDTO.SenhaHash     // senha digitada no login (texto puro)
            );

            if (resultadoVerificacao == PasswordVerificationResult.Failed)
            {
                resposta.Status = false;
                resposta.Mensagem = "Usuário ou senha inválidos.";
                return resposta;
            }

            // 3) Gera token JWT
            var token = TokenService.GerarToken(usuario);

            resposta.Status = true;
            resposta.Mensagem = "Login realizado com sucesso!";
            resposta.Dados = new LoginResponseDTO
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Token = token
            };

            return resposta;
        }



    }
}
