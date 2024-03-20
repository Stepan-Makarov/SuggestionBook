using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuggestionBookBlazorServer.Components.Account;
using SuggestionBookBlazorServer.Data;

namespace SuggestionBookBlazorServer.StartupConfig;

public static class DependencyInjectionExtensions
{
  public static void AddStandardServices(this WebApplicationBuilder builder)
  {
    builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
    builder.Services.AddMemoryCache();
  }

  public static void AddCustomServices(this WebApplicationBuilder builder)
  {
    builder.Services.AddScoped<INoSqlDbConnection, NoSqlDbConnection>();
    builder.Services.AddScoped<IStatusData, MongoStatusData>();
    builder.Services.AddScoped<ICategoryData,MongoCategoryData>();
    builder.Services.AddScoped<IUserData, MongoUserData>();
    builder.Services.AddScoped<ISuggestionData, MongoSuggestionData>();
  }

  public static void AddAuthServices(this WebApplicationBuilder builder)
  {
    builder.Services.AddCascadingAuthenticationState();
    builder.Services.AddScoped<IdentityUserAccessor>();
    builder.Services.AddScoped<IdentityRedirectManager>();
    builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

    builder.Services.AddAuthentication(options =>
    {
      options.DefaultScheme = IdentityConstants.ApplicationScheme;
      options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
        .AddIdentityCookies();

    builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

    builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
  }

  public static void AddDataBaseServices(this WebApplicationBuilder builder)
  {
    var connectionString = builder.Configuration.GetConnectionString("SuggestionIdentity") ?? throw new InvalidOperationException("Connection string 'SuggestionIdentity' not found.");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
  }
}
