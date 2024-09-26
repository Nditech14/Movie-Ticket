using Application.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using web.Extension;
using YesMovie.Extension.DataLakeExtension;
using YesMovie.MiddleWare;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddResponseCaching();

builder.Services.Configure<PayStackSettings>(builder.Configuration.GetSection("PayStackSettings"));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

builder.Services.AddControllers();
builder.Services.RegisterApplicationServices(builder.Configuration);
builder.Services.AddDataLakeServices(builder.Configuration);
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Movie|Ticket", Version = "v1" });
    c.SchemaFilter<EnumSchemaFilter>();


    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{builder.Configuration["AzureAdB2C:Instance"]}{builder.Configuration["AzureAdB2C:Domain"]}/oauth2/v2.0/authorize?p={builder.Configuration["AzureAdB2C:SignUpSignInPolicyId"]}"),
                TokenUrl = new Uri($"{builder.Configuration["AzureAdB2C:Instance"]}{builder.Configuration["AzureAdB2C:Domain"]}/oauth2/v2.0/token?p={builder.Configuration["AzureAdB2C:SignUpSignInPolicyId"]}"),
                Scopes = new Dictionary<string, string>
                {
                    { builder.Configuration["AzureAdB2C:Scopes:FileRead"]!, "Read access to files" },
                    { builder.Configuration["AzureAdB2C:Scopes:FileWrite"]!, "Write access to files" }
                }
            }
        }
    });


    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { builder.Configuration["AzureAdB2C:Scopes:FileRead"], builder.Configuration["AzureAdB2C:Scopes:FileWrite"] }
        }
    });
});


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.UseResponseCaching();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie/Ticket V1");
        c.OAuthClientId(builder.Configuration["AzureAdB2C:SwaggerClientId"]);
        c.OAuthAppName("Swagger UI for B2C");
        c.OAuthUsePkce();
        c.OAuth2RedirectUrl(builder.Configuration["AzureAdB2C:RedirectUri"]);
    });
}

app.UseHttpsRedirection();


app.UseSession();


app.UseAuthentication();
app.UseAuthorization();


app.UseMiddleware<UserClaimsMiddleware>();


app.MapControllers();

app.Run();