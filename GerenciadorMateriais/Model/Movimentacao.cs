using System;

namespace GerenciadorMateriais.Model
{
    public class Movimentacao
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; }
        public required string Tipo { get; set; } // "Entrada" ou "Saída"
        public int QuantidadeMovimentada { get; set; }

        public int ProdutoId { get; set; }
        public required Produto Produto { get; set; }

        public int UsuarioId { get; set; }
        public required Usuario Usuario { get; set; }
    }
}