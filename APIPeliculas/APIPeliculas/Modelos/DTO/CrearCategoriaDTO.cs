using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Modelos.DTO
{
    public class CrearCategoriaDTO
    {
        //para crear una Categoria solo necesitamos el nombre por lo que solo pasaremos ese atributo
        [Required(ErrorMessage = "El nombre es obligatorio")]
        //el largo del nombre sera de 100, si no mostrara ese error
        [MaxLength(100, ErrorMessage = "El numero maximo de caracteres es de 100!")]
        public string Nombre { get; set; }
    }
}
