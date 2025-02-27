using APIPeliculas.Data;
using APIPeliculas.PeliculasMapper;
using APIPeliculas.Repositorio;
using APIPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.// añadimos los servicios
builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSQL")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Agregamos los repositorios
builder.Services.AddScoped<ICategoria, CategoriaRepository>();
builder.Services.AddScoped<IPelicula, PeliculaRepository>();
//AgregamosAutomapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
