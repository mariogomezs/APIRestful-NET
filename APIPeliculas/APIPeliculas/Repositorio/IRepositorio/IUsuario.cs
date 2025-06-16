using APIPeliculas.Modelos;
using APIPeliculas.Modelos.DTO;

namespace APIPeliculas.Repositorio.IRepositorio
{
    public interface IUsuario
    {
        //Es ICollection para que nos traiga toda la lista, ya que es una coleccion
        ICollection<AppUsuarios> GetUsuarios();
        //Para una sola categoria, buscamos por su atributo id
        AppUsuarios GetUsuario(string id);
       
        // saber si ya existe ese nombre de usuario
        bool IsUniqueUser(string usuario);


        Task<UsuarioLoginRespuestaDTO> Login(UsuarioLoginDTO usarioLogin);
        Task<UsuarioDatosDTO> Registro (UsuarioRegistroDTO usarioRegistro);


    }
}
