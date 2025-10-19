namespace GerenciadorMateriais.Model
{
    public class MovimentacaoRequest
    {
        public int ProdutoId { get; set; }
        public int UsuarioId { get; set; }
        public int QuantidadeMovimentada { get; set; }
    }
}