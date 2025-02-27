using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIPeliculas.Modelos
{
    public class Pelicula
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Duracion { get; set; }
        public string RutaImagen { get; set; }
        public enum TipoClasificacion { Siete,Trece,Dieciseis,Dieciocho }
        //Implementa a enum de arriba
        public TipoClasificacion clasificacion { get; set; }
        public DateTime fechaCreacion { get; set; }

        //Relacion con categoria
        //Creamos un atributo con el que haremos la relacion con Categoria
        public int categoriaId { get; set; }
        //Diremos que esta es la llave foreanea
        [ForeignKey("categoriaId")]
        //la tabla donde es la llave foranea
        public Categoria Categoria { get; set; }
    }
}
