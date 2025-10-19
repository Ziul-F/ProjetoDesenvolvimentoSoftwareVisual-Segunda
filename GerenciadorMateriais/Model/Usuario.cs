namespace GerenciadorMateriais.Model
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Login { get; set; }
        public required string Senha { get; set; }
        public required string Perfil { get; set; } // "Admin" ou "Operacional"
    }
}