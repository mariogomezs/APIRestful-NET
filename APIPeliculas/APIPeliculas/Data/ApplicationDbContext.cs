using APIPeliculas.Modelos;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APIPeliculas.Data
{
    //Esta clase mapea todos los modelos a la base de datos
    //Al añadir lo de AppUsuarios ya no hereda de DbCOntext, si no de IdentifyDbCOntext
    public class ApplicationDbContext : IdentityDbContext<AppUsuarios>
    {
        public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }
        //este metodo lo hemos añadido para que no nos falle al añadir el ultimo campo, sin el nos fallaria la autentificacion con Identity
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        //pasamos aqui todos los modelos que queremos en la BBDD
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<AppUsuarios> AppUsuarios { get; set; }
    }
}
