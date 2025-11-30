using Microsoft.EntityFrameworkCore;
using PM_6SEM_2025.Data;
using PM_6SEM_2025.DTO.Denuncia;
using PM_6SEM_2025.Models;

namespace PM_6SEM_2025.Services.Denuncias
{
    public class DenunciasService : IDenunciasService
    {
        private readonly PmContext _context;
        public DenunciasService(PmContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<List<DenunciasModel>>> ListarDenuncias()
        {
            ResponseModel<List<DenunciasModel>> resposta = new ResponseModel<List<DenunciasModel>>();
            try
            {
                var denuncias = await _context.Denuncias
                    .Include(u => u.Usuario)
                    .Include(p => p.ProblemaAcessibilidade)
                    .ToListAsync();

                resposta.Dados = denuncias;
                resposta.Mensagem = "Lista de denúncias carregada com sucesso!";
                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Status = false;
                resposta.Mensagem = ex.Message;
                return resposta;
            }
        }

        public async Task<ResponseModel<DenunciasModel>> BuscarDenunciaPorId(int idDenuncia)
        {
            ResponseModel<DenunciasModel> resposta = new ResponseModel<DenunciasModel>();
            try
            {
                var denuncia = await _context.Denuncias
                    .Include(u => u.Usuario)
                    .Include(p => p.ProblemaAcessibilidade)
                    .FirstOrDefaultAsync(d => d.IdDenuncia == idDenuncia);

                if (denuncia == null)
                {
                    resposta.Mensagem = "Nenhuma denúncia encontrada!";
                    return resposta;
                }

                resposta.Dados = denuncia;
                resposta.Mensagem = "Denúncia localizada!";
                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Status = false;
                resposta.Mensagem = ex.Message;
                return resposta;
            }
        }

        public async Task<ResponseModel<List<DenunciasModel>>> BuscarDenunciasPorUsuario(int idUsuario)
        {
            ResponseModel<List<DenunciasModel>> resposta = new ResponseModel<List<DenunciasModel>>();
            try
            {
                var denuncias = await _context.Denuncias
                    .Where(d => d.IdUsuario == idUsuario)
                    .Include(p => p.ProblemaAcessibilidade)
                    .ToListAsync();

                if (denuncias == null || !denuncias.Any())
                {
                    resposta.Mensagem = "Nenhuma denúncia localizada para este usuário.";
                    return resposta;
                }

                resposta.Dados = denuncias;
                resposta.Mensagem = "Denúncias do usuário carregadas com sucesso!";
                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Status = false;
                resposta.Mensagem = ex.Message;
                return resposta;
            }
        }

        public async Task<ResponseModel<List<DenunciasModel>>> CriarDenuncia(CriarDenunciaDTO denunciaDTO)
        {
            ResponseModel<List<DenunciasModel>> resposta = new ResponseModel<List<DenunciasModel>>();
            try
            {
                var denuncia = new DenunciasModel()
                {
                    IdUsuario = denunciaDTO.IdUsuario,
                    Logradouro = denunciaDTO.Logradouro,
                    Numero = denunciaDTO.Numero,
                    Complemento = denunciaDTO.Complemento,
                    CEP = denunciaDTO.CEP,
                    Bairro = denunciaDTO.Bairro,
                    Cidade = denunciaDTO.Cidade,
                    Estado = denunciaDTO.Estado,
                    PontoReferencia = denunciaDTO.PontoReferencia,
                    ExistemObstaculos = denunciaDTO.ExistemObstaculos,
                    IdProblemaAcessibilidade = denunciaDTO.IdProblemaAcessibilidade,
                    MotivosPedido = denunciaDTO.MotivosPedido,
                    Descricao = denunciaDTO.Descricao,
                    ArquivoFoto = denunciaDTO.ArquivoFoto,
                    Status = "Pendente",
                    DataDenuncia = DateTime.Now
                };

                _context.Add(denuncia);
                await _context.SaveChangesAsync();

                resposta.Dados = await _context.Denuncias
                    .Include(u => u.Usuario)
                    .Include(p => p.ProblemaAcessibilidade)
                    .ToListAsync();

                resposta.Mensagem = "Denúncia criada com sucesso!";
                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Status = false;
                resposta.Mensagem = ex.Message;
                return resposta;
            }
        }

        public async Task<ResponseModel<List<DenunciasModel>>> AtualizarDenuncia(AtualizarDenunciaDTO denunciaDTO)
        {
            ResponseModel<List<DenunciasModel>> resposta = new ResponseModel<List<DenunciasModel>>();
            try
            {
                var denuncia = await _context.Denuncias.FirstOrDefaultAsync(d => d.IdDenuncia == denunciaDTO.IdDenuncia);

                if (denuncia == null)
                {
                    resposta.Mensagem = "Denúncia não encontrada!";
                    return resposta;
                }

                if (!string.IsNullOrEmpty(denunciaDTO.ProtocoloPrefeitura))
                    denuncia.ProtocoloPrefeitura = denunciaDTO.ProtocoloPrefeitura;

                if (!string.IsNullOrEmpty(denunciaDTO.Status))
                    denuncia.Status = denunciaDTO.Status;

                _context.Update(denuncia);
                await _context.SaveChangesAsync();

                resposta.Dados = await _context.Denuncias
                    .Include(u => u.Usuario)
                    .Include(p => p.ProblemaAcessibilidade)
                    .ToListAsync();

                resposta.Mensagem = "Denúncia atualizada com sucesso!";
                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Status = false;
                resposta.Mensagem = ex.Message;
                return resposta;
            }
        }

        public async Task<ResponseModel<List<DenunciasModel>>> ExcluirDenuncia(int idDenuncia)
        {
            ResponseModel<List<DenunciasModel>> resposta = new ResponseModel<List<DenunciasModel>>();
            try
            {
                var denuncia = await _context.Denuncias.FirstOrDefaultAsync(d => d.IdDenuncia == idDenuncia);

                if (denuncia == null)
                {
                    resposta.Mensagem = "Denúncia não encontrada!";
                    return resposta;
                }

                _context.Remove(denuncia);
                await _context.SaveChangesAsync();

                resposta.Dados = await _context.Denuncias.ToListAsync();
                resposta.Mensagem = "Denúncia excluída com sucesso!";
                return resposta;
            }
            catch (Exception ex)
            {
                resposta.Status = false;
                resposta.Mensagem = ex.Message;
                return resposta;
            }
        }
    }
}
