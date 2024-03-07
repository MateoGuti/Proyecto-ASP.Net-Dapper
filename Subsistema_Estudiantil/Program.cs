using Subsistema_Estudiantil.Servicios;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
//el System.Data.SqlClient se remplazo por using Microsoft.Data.SqlClient; ya que es el nuevo proveedor y asi se evita que 
//se genere error en new SqlConnection el cual dice"no se encuentra el nombre de tipo SqlConnection en el espacio de nombres System.Data.SqlClient"
using Microsoft.Data.SqlClient;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la cadena de conexión para poder utilizar el builder.Services.AddTransient<IDbConnection>((sp) => new SqlConnection(connectionString)); de forma que no salgan 
//Errores 
builder.Configuration.AddJsonFile("appsettings.json");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IRepositorioProfesor,RepositorioProfesor>();
builder.Services.AddTransient<IRepositorioEstudiante, RepositorioEstudiante>();
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();
builder.Services.AddTransient<IRepositorioMaestras, RepositorioMaestras>();
builder.Services.AddTransient<IDbConnection>((sp) => new SqlConnection(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
