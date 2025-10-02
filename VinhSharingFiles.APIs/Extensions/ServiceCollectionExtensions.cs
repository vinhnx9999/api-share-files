using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VinhSharingFiles.Application.Interfaces;
using VinhSharingFiles.Application.Services;
using VinhSharingFiles.Infrastructure.Data;
using VinhSharingFiles.Infrastructure.Repositories;

namespace VinhSharingFiles.APIs.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDefaultAWSOptions(this IServiceCollection services, AWSOptions options)
        {
            services.Add(new ServiceDescriptor(typeof(AWSOptions), options));
            return services;
        }

        public static IServiceCollection AddDefaultAWSOptions(this IServiceCollection collection, Func<IServiceProvider, AWSOptions> implementationFactory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            collection.Add(new ServiceDescriptor(typeof(AWSOptions), implementationFactory, lifetime));
            return collection;
        }

        //public static IServiceCollection AddAWSService<T>(this IServiceCollection collection, ServiceLifetime lifetime = ServiceLifetime.Singleton) where T : IAmazonService
        //{
        //    return collection.AddAWSService<T>(null, lifetime);
        //}

        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFileSharingRepository, FileSharingRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<ICloudService, AmazonS3Service>();
        }

        public static void AddDbContextServices(this IServiceCollection services, IConfiguration configuration)
        {
            var useInMemoryDB = configuration.GetValue<bool>("UseInMemoryDB");
            if (useInMemoryDB)
            {
                // we could have written that logic here but as per clean architecture, we are separating these into their own piece of code
                services.AddInMemoryDatabase();
            }
            else
            {
                //use this for real database on your sql server
                //configuration["Data:DefaultConnection:ConnectionString"]
                //Or configuration.GetConnectionString("DbContext")
                services.AddDbContext<VinhSharingDbContext>(options =>
                {
                    options.UseSqlServer(
                        configuration["Data:DefaultConnection:ConnectionString"],
                        providerOptions => providerOptions.EnableRetryOnFailure()
                    );
                });
            }
        }
    }
}
