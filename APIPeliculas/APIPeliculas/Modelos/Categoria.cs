using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Modelos
{
    public class Categoria
    {
        //indica que es una clave primaria
        [Key]
        public int Id { get; set; }
        //el campo es obligatorio
        [Required]
        public string Nombre { get; set; }
        [Required]
        public DateTime FechaCreacion { get; set; }
        
    }
}
