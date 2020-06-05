using DevIO.Business.Models;
using System;
using System.Threading.Tasks;

namespace DevIO.Business.Interfaces
{
    // Interface responsável pela classe de Fornecedor
    public interface IFornecedorRepository : IRepository<Fornecedor>
    {
        Task<Fornecedor> ObterFornecedorEndereco(Guid id);
        Task<Fornecedor> ObterFornecedorProdutosEndereco(Guid id);
    }
}
