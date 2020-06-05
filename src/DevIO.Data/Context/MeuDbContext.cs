using DevIO.Business.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DevIO.Data.Context
{
    public class MeuDbContext : DbContext
    {
        public MeuDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // caso foi esquecido de mapear alguma propriedade de uma entidade,
            // o mapeamento (sempr eque for string) será feito sempre com varchar(100)
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(entity => entity.GetProperties()
                .Where(prop => prop.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeuDbContext).Assembly);

            // Config para que quando uma entidade for excluída, seu filhos não serão
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(entity => entity.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            base.OnModelCreating(modelBuilder);
        }
    }
}
