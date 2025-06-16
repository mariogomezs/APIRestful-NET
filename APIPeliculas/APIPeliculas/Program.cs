using APIPeliculas.Data;
using APIPeliculas.Modelos;
using APIPeliculas.PeliculasMapper;
using APIPeliculas.Repositorio;
using APIPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.// añadimos los servicios
builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSQL")));

//Soporte para autentificacion con .NET identify
builder.Services.AddIdentity<AppUsuarios, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

//Cache
builder.Services.AddResponseCaching();

builder.Services.AddControllers(opcion =>
{
    //Cache global para no tenerlo que ponerlo en todas partes.
    opcion.CacheProfiles.Add("PorDefecto30Segundos",new CacheProfile() { Duration=30});
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( options =>
   {
       options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
       {
           Description="AUtentificacion JWT usando el esquema Bearer."+
           "Ingresa la palabra Bearer seguido de un [Espacio] y despues su token en el campo de abajo."+
           "Ejemplo: Bearer tkekrkrkrkr",
           Name="Athorization",
           In=ParameterLocation.Header,
           Scheme= "Bearer"
       });
       options.AddSecurityRequirement(new OpenApiSecurityRequirement()
       {{
           new OpenApiSecurityScheme
           {
               Reference=new OpenApiReference
               {
                   Type=ReferenceType.SecurityScheme,
                   Id="Bearer"
               },
               Scheme ="oath2",
               Name="Bearer",
               In=ParameterLocation.Header
           },
           new List<string>() }
       });
       options.SwaggerDoc("v1",new OpenApiInfo
       {
           Version="v1.0",
           Title="PeliculasApi",
           Description="Api de Peliculas",
           TermsOfService=new Uri("https://render2web.com/promociones"),
           Contact=new OpenApiContact
           {
               Name="render2web",
               Url=new Uri("https://render2web.com/promociones")
           },
           License=new OpenApiLicense
           {
               Name="Licencia Personal",
               Url=new Uri("https://render2web.com/promociones")
           }
       });
       options.SwaggerDoc("v2", new OpenApiInfo
       {
           Version = "v2.0",
           Title = "PeliculasApi v2",
           Description = "Api de Peliculas",
           TermsOfService = new Uri("https://render2web.com/promociones"),
           Contact = new OpenApiContact
           {
               Name = "render2web",
               Url = new Uri("https://render2web.com/promociones")
           },
           License = new OpenApiLicense
           {
               Name = "Licencia Personal",
               Url = new Uri("https://render2web.com/promociones")
           }
       });
   }
   
    );
  

//Soporte para CORS
//Se pueden habilitar: 1-Un dominio, 2-Multiples dominios,
//3-Cualquier dominio(Tener en cuenta seguridad)
// Usamos de ejemplo el dominio: http://localhost:3223, se debe cambiar por el correcto
// Se usa (*) para todos los dominios

//Añadimos el cors y luego añadimos su politica, podemos pasarle a la politica el nombre que queramos
builder.Services.AddCors(p => p.AddPolicy("PoliticaCors",build =>
{
    //Con WithOrigins le pasaremos el dominio que queramos, con AllowAnyMethod daremos acceso a todos los metodos y headers
    build.WithOrigins("http://localhost:3223").AllowAnyMethod().AllowAnyHeader();
}));




//Agregamos los repositorios
builder.Services.AddScoped<ICategoria, CategoriaRepository>();
builder.Services.AddScoped<IPelicula, PeliculaRepository>();
builder.Services.AddScoped<IUsuario, UsuarioRepository>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

//Soporte para el versionamiento
var apiVersioningBuilder = builder.Services.AddApiVersioning(opcion =>
{
    //hace que no sea obligatotorio poner el numero de versionado, si fuera false seria obligatorio
opcion.AssumeDefaultVersionWhenUnspecified = true;
    //establecemos por defecto que version sera
opcion.DefaultApiVersion = new ApiVersion(1, 0);
opcion.ReportApiVersions = true;
//opcion.ApiVersionReader = ApiVersionReader.Combine(
//        new QueryStringApiVersionReader("api-version"));
});

apiVersioningBuilder.AddApiExplorer(
    opciones =>
    {
        opciones.GroupNameFormat = "'v'VVV";
        opciones.SubstituteApiVersionInUrl = true;
    });
//AgregamosAutomapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper));

// COnfiguracion de la autentificacion
builder.Services.AddAuthentication(
    X=> { 
        //estas dos establecen el esquema de autentificacion predeterminada, en este caso JWT
        X.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        X.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
    
    ).AddJwtBearer(x =>
    { x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey=true,
           IssuerSigningKey=new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
           ValidateIssuer= false,
           ValidateAudience=false
        
        };
    
    
    
    }
    
    
    );
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opciones =>
    {
        opciones.SwaggerEndpoint("/swagger/v1/swagger.json","ApiPeliculasV1");
        opciones.SwaggerEndpoint("/swagger/v2/swagger.json", "ApiPeliculasV2");
    });
}

app.UseHttpsRedirection();

//damos el uso del cors

app.UseCors("PoliticaCors");
//Configurar la autentificacion
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
