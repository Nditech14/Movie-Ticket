using Application.Service.Abstraction;
using Application.Service.Implementation;
using Core;
using Core.Abstraction;
using Core.Entities;
using Core.Implementation;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;

namespace web.Extension
{
    public static class ExtensionService
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton<CosmosClient>(provider =>
            {
                var cosmosDbConfig = configuration.GetSection("CosmosDb");
                var account = cosmosDbConfig["Account"];
                var key = cosmosDbConfig["Key"];
                return new CosmosClient(account, key);
            });

            // Register generic repository and service for all types
            services.AddSingleton(typeof(ICosmosDbRepository<>), typeof(CosmosDbRepository<>));
            services.AddSingleton(typeof(ICosmosDbService<>), typeof(CosmosDbService<>));

            // Configure OpenIdConnect
            services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name", // Map to the claim you added
                    RoleClaimType = "roles"
                };
            });

            // Other services
            services.AddHttpContextAccessor();
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IActorService, ActorService>();
            services.AddScoped<IFileService<FileEntity>, FileService>();
            services.AddScoped<IProducerService, ProducerService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IEmailService, EmailService>();
            // services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICinemaService, CinemaService>();
            services.AddScoped<ICartService, CartService>();

            services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddDebug();
                // Add more logging providers if needed (e.g., EventLog, Azure, etc.)
            });


            return services;
        }
    }
}
