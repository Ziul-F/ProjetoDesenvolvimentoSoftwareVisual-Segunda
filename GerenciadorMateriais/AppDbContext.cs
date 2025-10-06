using GerenciadorMateriais.Model;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorMateriais
{
    public class AppDbContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Materiais.db");
        }
    }
}
