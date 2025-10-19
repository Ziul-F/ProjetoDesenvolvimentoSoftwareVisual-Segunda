using System.Text.Json.Serialization;

namespace GerenciadorMateriais.Model
{
    public class Movimentacao
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; } = DateTime.Now;
        public string Tipo { get; set; } = string.Empty; // "Entrada" ou "Saída"
        public int QuantidadeMovimentada { get; set; }

        public int ProdutoId { get; set; }
        [JsonIgnore]
        public Produto? Produto { get; set; }

        public int UsuarioId { get; set; }
        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}