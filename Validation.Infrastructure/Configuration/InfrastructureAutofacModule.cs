using Autofac;
using Validation.Application.Common.Seeds;
using Validation.Infrastructure.Common.Caching;

namespace Validation.Infrastructure.Configuration
{
    public class InfrastructureAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CacheProvider>().AsSelf().SingleInstance();
            builder.RegisterType<CacheRepository>().As<ICacheRepository>().InstancePerDependency();

            base.Load(builder);
        }
    }
}
