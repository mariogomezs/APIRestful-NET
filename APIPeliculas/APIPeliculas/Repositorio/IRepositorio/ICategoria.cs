using APIPeliculas.Modelos;

namespace APIPeliculas.Repositorio.IRepositorio
{
    public interface ICategoria
    {
        //Es ICollection para que nos traiga toda la lista, ya que es una coleccion
        ICollection<Categoria> GetCategorias();
        //Para una sola categoria, buscamos por su atributo id
        Categoria GetCategoria(int id);
        //Para saber si existe esa categoria, le pasamos el atributo id
        bool existeCategoria(int id);
        //Saber si existe la categoria pero por el nombre
        bool existeCategoria(String  id);
        //crear categoria
        bool crearCategoria(Categoria categoria);
        //actualizar una categoria
        bool actualizarCategoria(Categoria categoria);
        //borrar
        bool borrarCategoria(Categoria categoria);
        //guardar
        bool guardar();

    }
}
