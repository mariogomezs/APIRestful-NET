using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Modelos.DTO
{
    public class UsuarioDTO
    {
        public string Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
