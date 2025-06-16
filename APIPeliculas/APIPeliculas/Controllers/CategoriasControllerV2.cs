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
    [Route("api/v{version:apiVersion}/categorias")]
    [ApiController]
    //Multiples versiones
    [ApiVersion("2.0")]
    public class CategoriasControllerV2 : ControllerBase
    {
        //Necesitamos acceder al repositorio de Categorias que esta en repository, donde esta toda la logica, por eso ponemos la interfaz
        private readonly ICategoria repoCategoria;
        //Ponemos el IMapper ya que necesitamos acceder al Mapper que creamos al pricipio, para acceder tanto al model como al DTO y viceversa
        private readonly IMapper mapper;
        public CategoriasControllerV2(ICategoria repoCategoria, IMapper mapper)
        {
            this.repoCategoria = repoCategoria;
            this.mapper = mapper;
        }
       
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "valor1", "valor2,valor3" };
        }

       
    }
}
