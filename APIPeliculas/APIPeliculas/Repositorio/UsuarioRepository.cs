using APIPeliculas.Data;
using APIPeliculas.Modelos;
using APIPeliculas.Modelos.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using APIPeliculas.Data;
using APIPeliculas.Modelos;
using APIPeliculas.Modelos.DTO;
using APIPeliculas.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace APIPeliculas.Repositorio
{
    public class UsuarioRepository : IUsuario
    {
        //con readonly lo usaremos muy parecido a un const,una vez inicializado no podremos cambiar el valor
        //la diferencia es que con readonly si podremos darle un valor en tiempo de ejecucion(al iniciar el constructor)
        private readonly ApplicationDbContext bd;
        private string claveSecreta;
        //para hacer uso de AppUsuarios
        private readonly UserManager<AppUsuarios> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;
        //con este atributo y este constructor podremos acceder a todos las entidades de la BBDD(ApplicationDbContext bd)
        // con IConfig podemos acceder a appsettings.json
        public UsuarioRepository(ApplicationDbContext bd,IConfiguration config,UserManager<AppUsuarios> user,RoleManager<IdentityRole> role, IMapper mapper)
        {
            this.userManager = user;
            this.bd = bd;
            this.claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
            this.roleManager = role;
            this.mapper = mapper;
        }

        public AppUsuarios GetUsuario(string id)
        {
          return bd.AppUsuarios.FirstOrDefault(c=> c.Id==id);
        }

        public ICollection<AppUsuarios> GetUsuarios()
        {
           return  bd.AppUsuarios.OrderBy(c => c.UserName).ToList();
        }

        public bool IsUniqueUser(string usuario)
        {
            var usuarioBD = bd.AppUsuarios.FirstOrDefault(u => u.UserName == usuario);
            if (usuarioBD == null)
            {
                return true;
            }
            return false;
        }

        public async Task<UsuarioLoginRespuestaDTO> Login(UsuarioLoginDTO usarioLogin)
        {
            // al estar encriptada en el registro tambien debemos de encriptarla aqui
           // var password = obtenermd5(usarioLogin.Password);
            var usuario=bd.AppUsuarios.FirstOrDefault(u=>u.UserName.ToLower()==usarioLogin.NombreUsuario.ToLower());

            bool isValid = await userManager.CheckPasswordAsync(usuario, usarioLogin.Password);
            // Validamos si el usuario no existe  con la combinacion de usuario y contraseña
            if (usuario == null || isValid==false)
            {
                return new UsuarioLoginRespuestaDTO()
                {
                    Token="",
                    Usuario=null
                };
               
            }
            //Aqui existe el usuario
            //Creamos el JwtSecurityToken
            var roles=await userManager.GetRolesAsync(usuario);
            var manejadorToken = new JwtSecurityTokenHandler();
            //claveSecreta encriptada
            var key = Encoding.ASCII.GetBytes(claveSecreta);
            //describir las propiedades del token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    //Name representa el nombre del usuario, pasa el nombre de usuario a ClaimTypes
                    new Claim(ClaimTypes.Name,usuario.UserName.ToString())
                    //tambien podemos poner el Role de usuario, ya que viene en ClaimTypes
                    ,new Claim(ClaimTypes.Role,roles.FirstOrDefault())
                }),
                Expires=DateTime.UtcNow.AddDays(7),
                SigningCredentials=new (new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };
            //creamos el Token
            var token = manejadorToken.CreateToken(tokenDescriptor);
            UsuarioLoginRespuestaDTO usuarioLoginRespuestaDTO = new UsuarioLoginRespuestaDTO()
            {
                Token = manejadorToken.WriteToken(token),
                Usuario = mapper.Map<UsuarioDatosDTO>(usuario)

            };
            return usuarioLoginRespuestaDTO;


        }

        public async Task<UsuarioDatosDTO> Registro(UsuarioRegistroDTO usarioRegistro)
        {
            //  var password = obtenermd5(usarioRegistro.Password);
            AppUsuarios usuario = new AppUsuarios()
            {
                UserName = usarioRegistro.NombreUsuario,
                Email = usarioRegistro.NombreUsuario,
                NormalizedEmail = usarioRegistro.NombreUsuario.ToUpper(),
                Nombre = usarioRegistro.Nombre
            };
            var result = await userManager.CreateAsync(usuario, usarioRegistro.Password);
            if (result.Succeeded)
            {
                if (!roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                    await roleManager.CreateAsync(new IdentityRole("Registrado"));
                }
                await userManager.AddToRoleAsync(usuario,"Admin");
                var usuarioRetorno = bd.AppUsuarios.FirstOrDefault(u => u.UserName == usarioRegistro.Nombre);
                return mapper.Map<UsuarioDatosDTO>(usuarioRetorno);
            }

            //      bd.Usuarios.Add(usuario);
            //await  bd.SaveChangesAsync();
            //         usuario.Password = password;
            //º          return usuario;
            return new UsuarioDatosDTO();
        }
        public static string obtenermd5(string valor)
        {
            using (MD5 md5 = MD5.Create()) // Crear instancia de MD5 de manera moderna
            {
                byte[] data = Encoding.UTF8.GetBytes(valor);
                byte[] hash = md5.ComputeHash(data);

                // Convertir el array de bytes a una cadena hexadecimal
                string resp = "";
                foreach (byte b in hash)
                {
                    resp += b.ToString("x2").ToLower(); // Formatear cada byte en hexadecimal
                }
                return resp;
            }
        }
        }
}
