
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using YesMovie.Extension.DataLakeExtension;
using YesMovie.MiddleWare;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews(options =>
{
    // Global authorization policy requiring authenticated users for all actions
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddLogging();
// Configure Azure AD B2C authentication and token acquisition
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(new string[] {
        builder.Configuration["AzureAd:Scopes:FileRead"],
        builder.Configuration["AzureAd:Scopes:FileWrite"]
    })
    .AddInMemoryTokenCaches();

// Configure role/claims-based authorization policies
builder.Services.AddAuthorization(options =>
{
    // Default policy for authenticated access
    options.FallbackPolicy = options.DefaultPolicy;

    // Custom policy for file access (e.g., requiring specific scope claims)
    options.AddPolicy("FileAccess", policy =>
    {
        policy.RequireClaim("scp", builder.Configuration["AzureAd:Scopes:FileRead"]);
    });
});

// Register Cosmos DB services using the extension method
//builder.Services.AddCosmosDbServices(builder.Configuration); // Assuming you have this extension method
//builder.Services.AddAutoMapper(typeof(Mapper));
// Enable Razor Pages for UI-related features (login/logout/profile management)
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();
builder.Services.AddDataLakeServices(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    // Error handling for production environments
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserClaimsMiddleware>();

// Map controller routes for MVC pattern
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages routes for login/logout/profile management
app.MapRazorPages();

// Gracefully handle authentication/authorization errors
app.Use(async (context, next) =>
{
    if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
    {
        context.Response.Redirect("/Home/Error");
    }
    else
    {
        await next();
    }
});

app.Run();
