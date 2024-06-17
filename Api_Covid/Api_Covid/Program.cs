using Api_Covid;
using Api_Covid.Datos;
using Api_Covid.Repository;
using Api_Covid.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Creamos Servicio para Conectamos a la base de datos desde nuestro archivo Json
builder.Services.AddDbContext<ApplicationDbContext>(option => 
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
} );

//Creamos servicio para Json
builder.Services.AddControllers().AddNewtonsoftJson();

//Creamos servicio para los map
builder.Services.AddAutoMapper(typeof(MappingConfig));

//Creamos servicio para la implementacion
builder.Services.AddScoped<IEmpleado,EmpleadoRepositorio>();

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
