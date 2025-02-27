using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Modelos.DTO
{
    public class PeliculaDTO
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Duracion { get; set; }
        public string RutaImagen { get; set; }
        public enum TipoClasificacion { Siete, Trece, Dieciseis, Dieciocho }
        //Implementa a enum de arriba
        public TipoClasificacion clasificacion { get; set; }
        public DateTime fechaCreacion { get; set; }

        //Relacion Categoria solo dejamos el categoriaID
        public int CategoriaID { get; set; }
    }
}
