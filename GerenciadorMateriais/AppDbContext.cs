using GerenciadorMateriais.Model;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorMateriais
{
    public class AppDbContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}