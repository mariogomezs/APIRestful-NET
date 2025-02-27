using APIPeliculas.Modelos;
using APIPeliculas.Modelos.DTO;
using APIPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace APIPeliculas.Controllers
{
    [Route("api/peliculas")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        //Necesitamos acceder al repositorio de Categorias que esta en repository, donde esta toda la logica, por eso ponemos la interfaz
        private readonly IPelicula repoPelicula;
        //Ponemos el IMapper ya que necesitamos acceder al Mapper que creamos al pricipio, para acceder tanto al model como al DTO y viceversa
        private readonly IMapper mapper;
        public PeliculasController(IPelicula repoPelicula, IMapper mapper)
        {
            this.repoPelicula = repoPelicula;
            this.mapper = mapper;
        }
        //Decimos de que tipo sera el metodo
        [HttpGet]
        //Posibles Respuestas que puede dar
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPeliculas()
        {
            var listaPeliculas = repoPelicula.GetPeliculas();

            var listaPeliculasDTO = new List<PeliculaDTO>();
            foreach (var lista in listaPeliculas)
            {
                listaPeliculasDTO.Add(mapper.Map<PeliculaDTO>(lista));
            }
            return Ok(listaPeliculasDTO);
        }
        [HttpGet("{id:int}", Name = "GetPelicula")]
        //Posibles Respuestas que puede dar
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPelicula(int id)
        {
            var pelicula = repoPelicula.GetPelicula(id);
            //si categoria es igual a null
            if (pelicula == null)
            {
                return NotFound();
            }
            var itemPelicula = mapper.Map<PeliculaDTO>(pelicula);

            return Ok(itemPelicula);
        }

        [HttpPost]
        [ProducesResponseType(201,Type=typeof(PeliculaDTO))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]

        public IActionResult AnadirPelicula([FromBody] CrearPeliculaDTO pelicula)
        {

            //Si el modelo no es valido devolver un badrequest
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // si la categoria es nula devolvemos NotFound
            if (pelicula == null)
            {
                return NotFound(ModelState);
            }
            if (repoPelicula.existePelicula(pelicula.Nombre))
            {
                ModelState.AddModelError("", $"La categoria ya existe");
                return StatusCode(400, ModelState);
            }
            var peliculaMapper = mapper.Map<Pelicula>(pelicula);
            if (!repoPelicula.crearPelicula(peliculaMapper))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro{peliculaMapper.Nombre}");
                return StatusCode(404, ModelState);
            }
            return CreatedAtRoute("GetPelicula", new { id = peliculaMapper.Id }, peliculaMapper);
        }
        //Patch sirve para actualizar pero solo un atributo, por ejemplo actualizar el nombre, para todo seria el verbo PUT
        [HttpPatch("{id:int}", Name = "ActualizarPatchPelicula")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult ActualizarPatchPelicula(int id, [FromBody] PeliculaDTO pelicula)
        {

            //Si el modelo no es valido devolver un badrequest
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // si la categoria es nula devolvemos NotFound
            if (pelicula == null || id != pelicula.Id)
            {
                return NotFound(ModelState);
            }
            if (pelicula == null)
            {
                return NotFound($"No se encontro la categoria con ID {id}");
            }

            var peliculaMapper = mapper.Map<Pelicula>(pelicula);
            if (!repoPelicula.actualizarPelicula(peliculaMapper))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro{peliculaMapper.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        //Patch sirve para actualizar pero solo un atributo, por ejemplo actualizar el nombre, para todo seria el verbo PUT
        [HttpDelete("{id:int}", Name = "borrarPelicula")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult borrarPelicula(int id)
        {
            if (!repoPelicula.existePelicula(id))
            {
                return NotFound();
            }
            var pelicula = repoPelicula.GetPelicula(id);
            if (!repoPelicula.borrarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo ha salido mal borrando el registro{pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        [HttpGet("GetPeliculasEnCategorias/{id:int}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPeliculasEnCategorias(int id)
        {
            var listaPeliculasCategorias=repoPelicula.GetPeliculasENCategoria(id);
            if (listaPeliculasCategorias == null)
            {
                return NotFound();
            }
            var itemPelicula = new List<PeliculaDTO>();
            foreach (var item in listaPeliculasCategorias)
            {
                itemPelicula.Add(mapper.Map<PeliculaDTO>(item));
            }
            return Ok(itemPelicula);
        }
        [HttpGet("Buscar")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Buscar(string nombre)
        {
            try
            {
                var resultado = repoPelicula.BuscarPelicula(nombre);
                // si encuentra cualquier resultado
                if (resultado.Any())
                {
                    return Ok(resultado);
                }
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,"Error recuperando la busqueda de la conexion") ;
            }
        }


    }
}
