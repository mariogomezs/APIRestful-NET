using System.Net;

namespace APIPeliculas.Modelos
{
    public class RespuestaAPI
    {
        public RespuestaAPI()
        {
            ErrorMessages = new List<string>();

        }
        //Lo usaremos para recibir el codigo de estados
        public HttpStatusCode StatusCode { get; set; }
        // para validar si fue correcta la respuesta
        public bool IsSuccess { get; set; } = true;
        //Para recoger todos los errores
        public List<string> ErrorMessages { get; set; }
        //Resultado
        public object Result { get; set; }
    }
}
