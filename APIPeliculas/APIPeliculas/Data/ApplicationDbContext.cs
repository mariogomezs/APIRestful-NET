using APIPeliculas.Modelos;
using Microsoft.EntityFrameworkCore;

namespace APIPeliculas.Data
{
    //Esta clase mapea todos los modelos a la base de datos
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }
        //pasamos aqui todos los modelos que queremos en la BBDD
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
