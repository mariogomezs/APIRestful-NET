using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Modelos.DTO
{
    public class UsuarioRegistroDTO
    {
        [Required(ErrorMessage ="El usuario es obligatorio")]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
