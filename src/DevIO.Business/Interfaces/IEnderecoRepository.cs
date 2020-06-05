using DevIO.Business.Models;
using System;
using System.Threading.Tasks;

namespace DevIO.Business.Interfaces
{
    // Interface responsável pela classe de Endereco
    public interface IEnderecoRepository : IRepository<Endereco>
    {
        Task<Endereco> ObterEnderecoPorFornecedor(Guid fornecedorId);
    }
}
