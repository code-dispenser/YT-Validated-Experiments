using Autofac;
using Autofac.Extensions.DependencyInjection;
using Validation.Api.Configuration;
using Validation.Application.Configuration;
using Validation.Infrastructure.Configuration;

namespace Validation.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            builder.Host.ConfigureContainer<ContainerBuilder>(Container => {

                Container.RegisterModule<ApiAutofacModule>();
                Container.RegisterModule<ApplicationAutofacModule>();
                Container.RegisterModule<InfrastructureAutofacModule>();
            });

            builder.Services.AddHybridCache();
            builder.Services.AddControllers();

            var app = builder.Build();


            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
