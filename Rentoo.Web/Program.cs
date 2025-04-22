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
using Rentoo.Domain.Shared;


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
#region culture
var cultureConfiguration = builder.Configuration.GetSection(nameof(CultureConfiguration)).Get<CultureConfiguration>();
builder.Services.Configure<RequestLocalizationOptions>(
    opts =>
    {
        var supportedCultureCodes = (cultureConfiguration?.Cultures?.Count > 0 ?
            cultureConfiguration.Cultures.Intersect(CultureConfiguration.AvailableCultures) :
            CultureConfiguration.AvailableCultures).ToArray();

        if (!supportedCultureCodes.Any())
            supportedCultureCodes = CultureConfiguration.AvailableCultures;
        var supportedCultures = supportedCultureCodes.Select(c => new CultureInfo(c)).ToList();

        // If the default culture is specified use it, otherwise use CultureConfiguration.DefaultRequestCulture ("en")
        var defaultCultureCode = string.IsNullOrEmpty(cultureConfiguration?.DefaultCulture) ?
            CultureConfiguration.DefaultRequestCulture : cultureConfiguration?.DefaultCulture;

        // If the default culture is not among the supported cultures, use the first supported culture as default
        if (!supportedCultureCodes.Contains(defaultCultureCode))
            defaultCultureCode = supportedCultureCodes.FirstOrDefault();

        opts.DefaultRequestCulture = new RequestCulture(defaultCultureCode);
        opts.SupportedCultures = supportedCultures;
        opts.SupportedUICultures = supportedCultures;
        opts.RequestCultureProviders = new List<IRequestCultureProvider>
        {
                        new QueryStringRequestCultureProvider(),
                        new CookieRequestCultureProvider()
        };
    });
#endregion
// Dependency Injection
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();



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