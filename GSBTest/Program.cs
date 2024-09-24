using GSBTest.Models;
using GSBTest.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OfficeOpenXml;//Excel için lisans ayarý 



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(60);//You can set Time   
});

// Cookie Authentication ekleme
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Oturum açýlmamýþsa Login sayfasýna yönlendirme
    options.AccessDeniedPath = "/Account/AccessDenied"; // Yetki yoksa AccessDenied sayfasýna yönlendirme
});








builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// GsbtestContext'i DI konteynerine ekleyin
builder.Services.AddDbContext<GsbtestContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<LogService>(); // Burada servisi DI konteynerine ekliyoruz

// Lisans ayarý (non-commercial kullanýmlar için)
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Diðer ayarlar ve servislerin eklenmesi
builder.Services.AddControllersWithViews();




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

// Authentication ve Authorization middleware'leri ekleniyor
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();



app.Use(async (context, next) =>
{
    var userId = context.Session.GetInt32("UserId");
    if (userId == null && !context.Request.Path.Value.Contains("Account/Login"))
    {
        context.Response.Redirect("/Account/Login");
        return;
    }
    await next.Invoke();
});




app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();



