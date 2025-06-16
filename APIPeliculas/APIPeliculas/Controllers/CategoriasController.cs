using APIPeliculas.Modelos;
using APIPeliculas.Modelos.DTO;
using APIPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;

namespace APIPeliculas.Controllers
{
    //Cache 20 segundos duracion( Tambien se puede hacer con los metodos
    [ResponseCache(Duration = 20)]
    [Route("api/v{version:apiVersion}/categorias")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CategoriasController : ControllerBase
    {
        //Necesitamos acceder al repositorio de Categorias que esta en repository, donde esta toda la logica, por eso ponemos la interfaz
        private readonly ICategoria repoCategoria;
        //Ponemos el IMapper ya que necesitamos acceder al Mapper que creamos al pricipio, para acceder tanto al model como al DTO y viceversa
        private readonly IMapper mapper;
        public CategoriasController(ICategoria repoCategoria, IMapper mapper)
        {
            this.repoCategoria = repoCategoria;
            this.mapper = mapper;
        }
        //Decimos de que tipo sera el metodo
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ResponseCache(CacheProfileName = "PorDefecto30Segundos")]
        //SI no quisieramos guardar en cache: [ResponseCache(Location=ResponseCacheLocation.None, NoStore=true)]
        //Posibles Respuestas que puede dar
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //Probando el uso de CORS, hay que poner el nombre que pusimos en el Program(Tambien se podria poner en el controlador
        [EnableCors("PoliticaCors")]
        public IActionResult GetCategorias()
        {
         var listaCategorias=   repoCategoria.GetCategorias();

            var listaCategoriasDTO = new List<CategoriaDTO>();
            foreach( var lista in listaCategorias)
            {
                listaCategoriasDTO.Add(mapper.Map<CategoriaDTO>(lista));
            }
            return Ok(listaCategoriasDTO);
        }

        [HttpGet("{id:int}",Name ="GetCategoria")]
        //Posibles Respuestas que puede dar
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategoria(int id)
        {
            var categoria = repoCategoria.GetCategoria(id);
            //si categoria es igual a null
            if (categoria == null)
            {
                return NotFound();
            }
            var itemcategoria = mapper.Map<CategoriaDTO>(categoria);

            return Ok(itemcategoria);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]

        public IActionResult AnadirCategoria([FromBody] CrearCategoriaDTO categoria)
        {

            //Si el modelo no es valido devolver un badrequest
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // si la categoria es nula devolvemos NotFound
            if (categoria == null)
            {
                return NotFound(ModelState);
            }
            if (repoCategoria.existeCategoria(categoria.Nombre))
            {
                ModelState.AddModelError("", $"La categoria ya existe");
                return StatusCode(400, ModelState);
            }
            var categoriaMapper=mapper.Map<Categoria>(categoria);
            if (!repoCategoria.crearCategoria(categoriaMapper))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro{categoriaMapper.Nombre}");
                return StatusCode(404, ModelState);
            }
            return CreatedAtRoute("GetCategoria", new {id=categoriaMapper.Id},categoriaMapper);
        }
        //Patch sirve para actualizar pero solo un atributo, por ejemplo actualizar el nombre, para todo seria el verbo PUT
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}", Name = "ActualizarPatchCategoria")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult ActualizarPatchCategoria(int  id,[FromBody] CategoriaDTO categoria)
        {

            //Si el modelo no es valido devolver un badrequest
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // si la categoria es nula devolvemos NotFound
            if (categoria == null || id!=categoria.Id)
            {
                return NotFound(ModelState);
            }
            if (categoria == null)
            {
                return NotFound($"No se encontro la categoria con ID {id}");
            }

            var categoriaMapper = mapper.Map<Categoria>(categoria);
            if (!repoCategoria.actualizarCategoria(categoriaMapper))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro{categoriaMapper.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        //Patch sirve para actualizar pero solo un atributo, por ejemplo actualizar el nombre, para todo seria el verbo PUT
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}", Name = "ActualizarPutCategoria")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult ActualizarPutCategoria(int id, [FromBody] CategoriaDTO categoria)
        {

            //Si el modelo no es valido devolver un badrequest
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // si la categoria es nula devolvemos NotFound
            if (categoria == null || id != categoria.Id)
            {
                return NotFound(ModelState);
            }
            var categoriaExistente=repoCategoria.GetCategoria(id);
            if (categoriaExistente == null)
            {
                return NotFound($"No se encontro la categoria con ID {id}");
            }
            var categoriaMapper = mapper.Map<Categoria>(categoria);
            if (!repoCategoria.actualizarCategoria(categoriaMapper))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro{categoriaMapper.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        //Patch sirve para actualizar pero solo un atributo, por ejemplo actualizar el nombre, para todo seria el verbo PUT
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}", Name = "borrarCategoria")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult borrarCategoria(int id)
        {
            if (!repoCategoria.existeCategoria(id))
            {
                return NotFound();
            }
            var categoria = repoCategoria.GetCategoria(id);
            if (!repoCategoria.borrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo ha salido mal borrando el registro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
