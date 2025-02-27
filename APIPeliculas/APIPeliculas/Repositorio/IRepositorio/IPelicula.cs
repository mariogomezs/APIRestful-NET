using APIPeliculas.Modelos;

namespace APIPeliculas.Repositorio.IRepositorio
{
    public interface IPelicula
    {
        //Es ICollection para que nos traiga toda la lista, ya que es una coleccion
        ICollection<Pelicula> GetPeliculas();
        //Para una sola categoria, buscamos por su atributo id
        ICollection<Pelicula> GetPeliculasENCategoria(int catId);
        IEnumerable<Pelicula> BuscarPelicula(string nombre);
        Pelicula GetPelicula(int id);
        //Para saber si existe esa categoria, le pasamos el atributo id
        bool existePelicula(int id);
        //Saber si existe la categoria pero por el nombre
        bool existePelicula(string  nombre);
        //crear categoria
        bool crearPelicula(Pelicula pelicula);
        //actualizar una categoria
        bool actualizarPelicula(Pelicula pelicula);
        //borrar
        bool borrarPelicula(Pelicula pelicula);
        //guardar
        bool guardar();

    }
}
