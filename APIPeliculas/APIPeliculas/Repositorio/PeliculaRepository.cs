using APIPeliculas.Data;
using APIPeliculas.Modelos;
using APIPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace APIPeliculas.Repositorio
{
    public class PeliculaRepository : IPelicula
    {
        //con readonly lo usaremos muy parecido a un const,una vez inicializado no podremos cambiar el valor
        //la diferencia es que con readonly si podremos darle un valor en tiempo de ejecucion(al iniciar el constructor)
        private readonly ApplicationDbContext bd;
        //con este atributo y este constructor podremos acceder a todos las entidades de la BBDD
        public PeliculaRepository(ApplicationDbContext bd)
        {

            this.bd = bd;
        }

        public bool actualizarPelicula(Pelicula pelicula)
        {
            pelicula.fechaCreacion = DateTime.Now;
            // arreglar problema del PUT
            var peliculasExistente = bd.Peliculas.Find(pelicula.Id);
            if (peliculasExistente != null)
            {
                bd.Entry(peliculasExistente).CurrentValues.SetValues(pelicula);
            }
            else
            {
                bd.Peliculas.Update(pelicula);
            }

            return guardar();
        }

        public bool borrarPelicula(Pelicula pelicula)
        {
            bd.Peliculas.Remove(pelicula);
            return guardar();
        }

        public IEnumerable<Pelicula> BuscarPelicula(string nombre)
        {
            IQueryable<Pelicula> query = bd.Peliculas;
            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(e => e.Nombre.Contains(nombre) || e.Descripcion.Contains(nombre));
            }
            return query.ToList();
        }

        public bool crearPelicula(Pelicula pelicula)
        {
            pelicula.fechaCreacion = DateTime.Now;
            bd.Peliculas.Add(pelicula);
            return guardar();
        }

        public bool existePelicula(int id)
        {
          return   bd.Peliculas.Any(c => c.Id == id);
        }

        public bool existePelicula(string nombre)
        {
            return bd.Categorias.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
        }

        public Pelicula GetPelicula(int id)
        {
            return bd.Peliculas.FirstOrDefault(c=>c.Id==id);
        }

        public ICollection<Pelicula> GetPeliculas()
        {
            return bd.Peliculas.OrderBy(c => c.Nombre).ToList();
        }

        public ICollection<Pelicula> GetPeliculasENCategoria(int catId)
        {
            return bd.Peliculas.Include(ca => ca.Categoria).Where(ca => ca.categoriaId == catId).ToList();
        }

        public bool guardar()
        {
            return bd.SaveChanges() >= 0 ? true : false;
        }
    }
}

