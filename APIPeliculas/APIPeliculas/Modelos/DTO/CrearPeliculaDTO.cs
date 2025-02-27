using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Modelos.DTO
{
    public class CrearPeliculaDTO
    {
      
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Duracion { get; set; }
        public string RutaImagen { get; set; }
        //hemos tenido que cambiarle el nombre al enum por el error 500( no puede ser igualq ue el PeliculaDTO)
        public enum CrearTipoClasificacion { Siete, Trece, Dieciseis, Dieciocho }
        //Implementa a enum de arriba
        public CrearTipoClasificacion clasificacion { get; set; }
     

        //Relacion Categoria solo dejamos el categoriaID
        public int CategoriaID { get; set; }
    }
}
