using ClinicaEspacioAbiertoFrontEnd.Middleware;
using ClinicaEspacioAbiertoFrontEnd.Models;
using QuestPDF.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// Agrega servicios al contenedor
builder.Services.AddControllersWithViews(options =>
{
    // Registra el filtro globalmente
    options.Filters.Add(new ClinicaEspacioAbiertoFrontEnd.Middleware.RequireLoginFilter());
});
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache(); // Opcional: almacenar en memoria
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión
    options.Cookie.HttpOnly = true; // Cookies seguras
    options.Cookie.IsEssential = true; // Necesario para el funcionamiento básico
});
builder.Services.AddScoped<IConsultasService, ConsultasService>();

builder.Services.AddHttpClient<IConsultasService, ConsultasService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7231/api/");
});

    builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache(); // Agrega el servicio de memoria cache


builder.Services.AddMvc();
var app = builder.Build();

// Configura el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

QuestPDF.Settings.License = LicenseType.Community;
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseStaticFiles();
app.UseAuthentication(); // Debe estar antes de UseAuthorization
app.UseAuthorization();

app.UseSession(); // Debe estar después de UseRouting y antes de MapControllerRoute

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
