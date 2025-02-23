using APIPeliculas.Modelos;
using APIPeliculas.Modelos.DTO;
using AutoMapper;

namespace APIPeliculas.PeliculasMapper
{
    //extendemos de profile
    public class PeliculasMapper : Profile
    {
        public PeliculasMapper()
        {
            //Para que el modelo y el DTO se puedan comunicar debemos hacer esto. el reverse es para que se pueda conectar el DTO con el modelo
            CreateMap<Categoria, CategoriaDTO>().ReverseMap();
            CreateMap<Categoria, CrearCategoriaDTO>().ReverseMap();
        }
    }
}
