using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Modelos.DTO
{
    //LOS DTO sirven para no exponer directamente los atributos del modelo
    public class CategoriaDTO
    {
       // no necesito poner que es una KEY
        public int Id { get; set; }
        //el campo es obligatorio
        [Required(ErrorMessage ="El nombre es obligatorio")]
        //el largo del nombre sera de 100, si no mostrara ese error
        [MaxLength(100,ErrorMessage ="El numero maximo de caracteres es de 100!")]
        public string Nombre { get; set; }
        [Required]
        public DateTime FechaCreacion { get; set; }
    }
}
