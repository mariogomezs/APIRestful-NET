using APIPeliculas.Data;
using APIPeliculas.Modelos;
using APIPeliculas.Repositorio.IRepositorio;
using System.Collections;

namespace APIPeliculas.Repositorio
{
    public class CategoriaRepository : ICategoria
    {
        //con readonly lo usaremos muy parecido a un const,una vez inicializado no podremos cambiar el valor
        //la diferencia es que con readonly si podremos darle un valor en tiempo de ejecucion(al iniciar el constructor)
        private readonly ApplicationDbContext bd;
        //con este atributo y este constructor podremos acceder a todos las entidades de la BBDD
        public CategoriaRepository(ApplicationDbContext bd)
        {

            this.bd = bd;
        }
        public bool actualizarCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            // arreglar problema del PUT
            var categoriaExistete=bd.Categorias.Find(categoria.Id);
            if (categoriaExistete != null)
            {
                bd.Entry(categoriaExistete).CurrentValues.SetValues(categoria);
            }
            else
            {
                bd.Categorias.Update(categoria);
            }

                return guardar();
        }

        public bool borrarCategoria(Categoria categoria)
        {
            bd.Remove(categoria);
            return guardar();
        }

        public bool crearCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            bd.Add(categoria);
            return guardar();
        }

        public bool existeCategoria(int id)
        {
            //EN LINQ, usamos any para saber si ha cumplido una condicion o no, en este caso viendo si los id son iguales
            return bd.Categorias.Any(c => c.Id == id);
        }

        public bool existeCategoria(string nombre)
        {
            //esta vez estamos buscando que los string sean iguales, con trim lo que queremos es quitar los espacios
            bool valor=bd.Categorias.Any(c => c.Nombre.ToLower().Trim()==nombre.ToLower().Trim());
            return valor;
        }

        public Categoria GetCategoria(int id)
        {
            //Con parametros, devuelve la primera coincidencia que tenga igual que el parametro, si no devuelve null, si viene sin parametros, devolvera el primero
            return bd.Categorias.FirstOrDefault(c => c.Id == id);
        }

        public ICollection<Categoria> GetCategorias()
        {
            //priemro esta vez lo vamos a ordenar por nombre y luego lo haremos una lista para tener todos
            return bd.Categorias.OrderBy(c=>c.Nombre).ToList();
        }

        public bool guardar()
        {
            //guardara los cambios siempre que los registros sean mayor a 0, devolvera true si es mayor si no false
            return bd.SaveChanges() >= 0?true:false;
        }
    }
}
