using FitVital.DAL;
using FitVital.Domain.Interfaces;
using FitVital.Domain.Services;
using fitVital_API.Domain.Interfaces;
using fitVital_API.Domain.Services;
using Microsoft.EntityFrameworkCore;
using WebAPI.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Esta es la l�nea de code que necesito para configurar la conexi�n a la BD
builder.Services.AddDbContext<DataBaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEntrenadorService,EntrenadorService>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
