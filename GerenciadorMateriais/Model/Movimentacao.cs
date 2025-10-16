using System;

namespace GerenciadorMateriais.Model
{
    public class Movimentacao
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; }
        public string Tipo { get; set; } // "Entrada" ou "Saída"
        public int QuantidadeMovimentada { get; set; }

        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}