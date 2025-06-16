using Microsoft.AspNetCore.Identity;

namespace APIPeliculas.Modelos
{
    public class AppUsuarios : IdentityUser
    {
        public string Nombre { get; set; }
       


    }
}
