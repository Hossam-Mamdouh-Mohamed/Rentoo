using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Rentoo.Application.Interfaces;
using Rentoo.Application.Services;
using Rentoo.Domain.Entities;
using Rentoo.Domain.Interfaces;
using Rentoo.Infrastructure.Data;
using Rentoo.Infrastructure.Repositories;
using Rentoo.Web.Localization;

var builder = WebApplication.CreateBuilder(args);

// MVC & Views
builder.Services.AddControllersWithViews().AddViewLocalization()
    .AddDataAnnotationsLocalization();

// Database
builder.Services.AddDbContext<RentooDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//identity
builder.Services.AddIdentity<User, IdentityRole>(Options =>
{
    Options.Password.RequireDigit = true;
    Options.Password.RequireLowercase = true;
    Options.Password.RequireUppercase = true;
    Options.Password.RequireNonAlphanumeric = true;
    Options.Password.RequiredLength = 8;
    Options.Lockout.MaxFailedAccessAttempts = 3;
    Options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
}).AddEntityFrameworkStores<RentooDbContext>();
// Dependency Injection
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register the localization service
builder.Services.AddScoped<ISharedLocalizationService, SharedLocalizationService>();

// Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("ar"), new CultureInfo("en") };
    options.DefaultRequestCulture = new RequestCulture("ar");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();