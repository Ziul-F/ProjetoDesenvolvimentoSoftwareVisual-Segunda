namespace GerenciadorMateriais.Model
{
    public class Usuario
    {
        public  int Id { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string Perfil { get; set; } // "Admin" ou "Operacional"
    }
}