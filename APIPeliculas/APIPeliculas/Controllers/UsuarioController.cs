using APIPeliculas.Modelos;
using APIPeliculas.Modelos.DTO;
using APIPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace APIPeliculas.Controllers
{
    [Route("api/v{version:apiVersion}/usuarios")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsuarioController : ControllerBase
    {
        //Necesitamos acceder al repositorio de Categorias que esta en repository, donde esta toda la logica, por eso ponemos la interfaz
        private readonly IUsuario repoUsuarios;
        //Ponemos el IMapper ya que necesitamos acceder al Mapper que creamos al pricipio, para acceder tanto al model como al DTO y viceversa
        private readonly IMapper mapper;
        //Para que podemos usarlo en AnadirUsuario
        protected RespuestaAPI respuestaAPI;
        public UsuarioController(IUsuario repoUsuarios, IMapper mapper)
        {
            this.repoUsuarios = repoUsuarios;
            this.mapper = mapper;
            //instanciamos un nuevo objeto
            this.respuestaAPI = new();
        }
        [Authorize(Roles = "Admin")]
        //Decimos de que tipo sera el metodo
        [HttpGet]
        //Posibles Respuestas que puede dar
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsuarios()
        {
            var listaUsuarios = repoUsuarios.GetUsuarios();

            var listaUsuarioDTO = new List<UsuarioDTO>();
            foreach (var lista in listaUsuarios)
            {
                listaUsuarioDTO.Add(mapper.Map<UsuarioDTO>(lista));
            }
            return Ok(listaUsuarioDTO);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:string}", Name = "GetUsuario")]
        //Posibles Respuestas que puede dar
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsuario(string id)
        {
            var usuario = repoUsuarios.GetUsuario(id);
            //si categoria es igual a null
            if (usuario == null)
            {
                return NotFound();
            }
            var itemUsuario = mapper.Map<UsuarioDTO>(usuario);

            return Ok(itemUsuario);
        }
        [AllowAnonymous]
        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]

        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDTO usuarioDTO)
        {
         bool validarNombreUsuarioUnico=   repoUsuarios.IsUniqueUser(usuarioDTO.NombreUsuario);
            //existe el usuario
            if (!validarNombreUsuarioUnico)
            {
                respuestaAPI.StatusCode = HttpStatusCode.BadRequest;
                respuestaAPI.IsSuccess = false;
                respuestaAPI.ErrorMessages.Add("El nombre de usuario ya existe");
                return BadRequest(respuestaAPI);   
            }
            var usuario = await repoUsuarios.Registro(usuarioDTO);
            if (usuario == null)
            {
                respuestaAPI.StatusCode = HttpStatusCode.BadRequest;
                respuestaAPI.IsSuccess = false;
                respuestaAPI.ErrorMessages.Add("Error en el registro");
                return BadRequest(respuestaAPI);
            }
            respuestaAPI.StatusCode = HttpStatusCode.OK;
            respuestaAPI.IsSuccess = true;
            return Ok(respuestaAPI);
        }
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]

        public async Task<IActionResult> Login([FromBody] UsuarioLoginDTO usuarioDTO)
        {
            var respuestaLogin = await repoUsuarios.Login(usuarioDTO);
            // no existe el usuario o no me devuelve el Token
            if (respuestaLogin.Usuario==null|| string.IsNullOrEmpty(respuestaLogin.Token))
            {
                respuestaAPI.StatusCode = HttpStatusCode.BadRequest;
                respuestaAPI.IsSuccess = false;
                respuestaAPI.ErrorMessages.Add("El nombre de usuario o password es incorrecta");
                return BadRequest(respuestaAPI);
            }
           // si existe todo OK
                respuestaAPI.StatusCode = HttpStatusCode.OK;
                respuestaAPI.IsSuccess = true;
                respuestaAPI.Result = respuestaLogin;
            return Ok(respuestaAPI);
         
        }



    }
}
