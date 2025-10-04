using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace VinhSharingFiles.Infrastructure.Data;

public static class AppDbContextFactory
{
    public static void AddInMemoryDatabase(this IServiceCollection services)
    {
        //I want to make DB as in memory so you don't need to setup real database
        services.AddDbContext<VinhSharingDbContext>(options => options.UseInMemoryDatabase("SharingFileDb"));            
    }
}
