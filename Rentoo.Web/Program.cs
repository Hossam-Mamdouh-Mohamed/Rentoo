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

#region MVC & Views
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
#endregion

#region Database Context
builder.Services.AddDbContext<RentooDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region Localization
var cultureConfiguration = builder.Configuration.GetSection(nameof(CultureConfiguration)).Get<CultureConfiguration>();
builder.Services.Configure<RequestLocalizationOptions>(opts =>
{
    var supportedCultureCodes = (cultureConfiguration?.Cultures?.Count > 0 ?
        cultureConfiguration.Cultures.Intersect(CultureConfiguration.AvailableCultures) :
        CultureConfiguration.AvailableCultures).ToArray();

    if (!supportedCultureCodes.Any())
        supportedCultureCodes = CultureConfiguration.AvailableCultures;

    var supportedCultures = supportedCultureCodes.Select(c => new CultureInfo(c)).ToList();

    var defaultCultureCode = string.IsNullOrEmpty(cultureConfiguration?.DefaultCulture) ?
        CultureConfiguration.DefaultRequestCulture : cultureConfiguration?.DefaultCulture;

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

#region Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 1;
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
}).AddEntityFrameworkStores<RentooDbContext>();
#endregion

#region Dependency Injection
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
#endregion

var app = builder.Build();

#region Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
#endregion

#region Endpoint Mapping
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
#endregion

app.Run();
