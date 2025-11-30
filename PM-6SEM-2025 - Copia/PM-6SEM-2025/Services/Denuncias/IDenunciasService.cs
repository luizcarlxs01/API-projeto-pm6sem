using PM_6SEM_2025.DTO.Denuncia;
using PM_6SEM_2025.Models;

namespace PM_6SEM_2025.Services.Denuncias
{
    public interface IDenunciasService
    {
        Task<ResponseModel<List<DenunciasModel>>> ListarDenuncias();
        Task<ResponseModel<DenunciasModel>> BuscarDenunciaPorId(int idDenuncia);
        Task<ResponseModel<List<DenunciasModel>>> BuscarDenunciasPorUsuario(int idUsuario);
        Task<ResponseModel<List<DenunciasModel>>> CriarDenuncia(CriarDenunciaDTO denunciaDTO);
        Task<ResponseModel<List<DenunciasModel>>> AtualizarDenuncia(AtualizarDenunciaDTO denunciaDTO);
        Task<ResponseModel<List<DenunciasModel>>> ExcluirDenuncia(int idDenuncia);
    }
}
